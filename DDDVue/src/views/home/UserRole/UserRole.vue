<template>
  <div class="role-container">
    <div class="role-content">
      <el-card class="role-card">
        <template #header>
          <div class="card-header">
            <el-button class="button" type="primary" @click="addRole">添加角色</el-button>
          </div>
        </template>
        <el-table :data="roleList" style="width: 100%" :header-cell-style="{ background: '#f5f7fa', color: '#333' }"
          height="calc(100% - 120px)">
          <el-table-column prop="name" label="角色名称" min-width="120" />
          <el-table-column prop="code" label="角色编码" min-width="120" />
          <el-table-column prop="description" label="描述" min-width="180">
            <template #default="{ row }">
              {{ row.description || '-' }}
            </template>
          </el-table-column>
          <el-table-column prop="sortOrder" label="排序" width="80" />
          <el-table-column prop="status" label="状态" width="100">
            <template #default="{ row }">
              <el-tag :type="row.status === 1 ? 'success' : 'danger'" effect="dark">
                {{ row.status === 1 ? '启用' : '禁用' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="createdAt" label="创建时间" min-width="170">
            <template #default="{ row }">
              {{ formatDateTime(row.createdAt) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="280" fixed="right">
            <template #default="{ row }">
              <div class="table-actions">
                <el-button size="small" @click="editRole(row)">编辑</el-button>
                <el-button size="small" type="primary" @click="assignUsers(row)">分配用户</el-button>
                <el-button size="small" :type="row.status === 1 ? 'warning' : 'success'" @click="toggleRoleStatus(row)">
                  {{ row.status === 1 ? '禁用' : '启用' }}
                </el-button>
                <el-button size="small" type="danger" @click="deleteRole(row)">删除</el-button>
              </div>
            </template>
          </el-table-column>
        </el-table>
        <el-pagination v-model:current-page="pagination.pageNum" v-model:page-size="pagination.pageSize"
          :total="pagination.total" :page-sizes="[10, 20, 50, 100]" layout="total, sizes, prev, pager, next, jumper"
          background style="margin-top: 16px; justify-content: flex-end" @size-change="handleSizeChange"
          @current-change="handleCurrentChange" />
      </el-card>
    </div>

    <!-- 角色编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px" :destroy-on-close="true">
      <el-form :model="roleForm" :rules="roleRules" ref="roleFormRef" label-width="100px">
        <el-form-item label="角色名称" prop="name">
          <el-input v-model="roleForm.name" placeholder="请输入角色名称" />
        </el-form-item>
        <el-form-item label="角色编码" prop="code">
          <el-input v-model="roleForm.code" placeholder="请输入角色编码" :disabled="dialogTitle === '编辑角色'" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="roleForm.description" type="textarea" :rows="3" placeholder="请输入角色描述" />
        </el-form-item>
        <el-form-item label="排序号">
          <el-input-number v-model="roleForm.sortOrder" :min="0" :max="999" />
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="roleForm.remark" type="textarea" :rows="2" placeholder="请输入备注" />
        </el-form-item>
        <el-form-item label="状态" v-if="dialogTitle === '编辑角色'">
          <el-switch v-model="roleForm.status" :active-value="1" :inactive-value="0" inline-prompt active-text="启用"
            inactive-text="禁用" active-color="#13ce66" inactive-color="#ff4949" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitRoleForm">确定</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 分配用户对话框 -->
    <el-dialog v-model="userDialogVisible" title="分配用户" width="800px" :destroy-on-close="true">
      <el-form label-width="80px">
        <el-form-item label="角色名称">
          <el-input v-model="userForm.roleName" disabled />
        </el-form-item>
        <el-form-item label="选择用户">
          <el-transfer
            v-model="userForm.selectedUserIds"
            :data="allUsers"
            :titles="['可选用户', '已选用户']"
            :props="{
              key: 'id',
              label: 'realName'
            }"
            filterable
            filter-placeholder="搜索用户"
            v-loading="userLoading"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="userDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitUserForm" :loading="userLoading">确定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessageBox } from 'element-plus'
import * as roleApi from '@/api/role'
import * as userApi from '@/api/user'
import type { RoleDto, CreateRoleRequest, UpdateRoleRequest, UserDto } from '@/api/index'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'

// 解构导入 API 函数
const { getRoles, createRole: createRoleApi, updateRole: updateRoleApi, deleteRole: deleteRoleApi, enableRole, disableRole, getRoleUserIds, assignRoleUsers } = roleApi
const { getUsers } = userApi

// 角色表单类型
interface RoleForm {
  id?: string
  name: string
  code: string
  description?: string
  sortOrder: number
  remark?: string
  status: number
}

// 分页数据
interface Pagination {
  pageNum: number
  pageSize: number
  total: number
}

// 响应式数据
const roleList = ref<RoleDto[]>([])
const dialogVisible = ref(false)
const dialogTitle = ref('添加角色')
const roleFormRef = ref()
const roleForm = ref<RoleForm>({
  name: '',
  code: '',
  description: '',
  sortOrder: 0,
  remark: '',
  status: 1
})

const pagination = ref<Pagination>({
  pageNum: 1,
  pageSize: 10,
  total: 0
})

// 分配用户相关数据
const userDialogVisible = ref(false)
const userLoading = ref(false)
const allUsers = ref<UserDto[]>([])
const userForm = ref<{
  roleId: string
  roleName: string
  selectedUserIds: string[]
}>({
  roleId: '',
  roleName: '',
  selectedUserIds: []
})

// 角色表单验证规则
const roleRules = {
  name: [
    { required: true, message: '请输入角色名称', trigger: 'blur' },
    { min: 2, max: 50, message: '角色名称长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  code: [
    { required: true, message: '请输入角色编码', trigger: 'blur' },
    { min: 2, max: 50, message: '角色编码长度在 2 到 50 个字符', trigger: 'blur' },
    { pattern: /^[A-Z_]+$/, message: '角色编码只能包含大写字母和下划线', trigger: 'blur' }
  ]
}

// 格式化日期时间
const formatDateTime = (dateStr: string) => {
  if (!dateStr) return '-'
  const date = new Date(dateStr)
  return date.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

// 加载角色数据
const loadRoleData = async () => {
  try {
    const params = {
      pageNum: pagination.value.pageNum,
      pageSize: pagination.value.pageSize
    }
    const response = await getRoles(params)
    if (response.data) {
      roleList.value = response.data.list || []
      pagination.value.total = response.data.total || 0
    }
  } catch (error) {
    console.error('加载角色数据失败:', error)
    showErrorNotification({ title: '错误', message: '加载角色数据失败' })
  }
}

// 加载所有用户
const loadAllUsers = async () => {
  try {
    const response = await getUsers({ pageNum: 1, pageSize: 1000 })
    if (response.data) {
      allUsers.value = response.data.list || []
    }
  } catch (error) {
    console.error('加载用户列表失败:', error)
  }
}

// 添加角色
const addRole = () => {
  dialogTitle.value = '添加角色'
  resetRoleForm()
  dialogVisible.value = true
}

// 编辑角色
const editRole = (row: RoleDto) => {
  dialogTitle.value = '编辑角色'
  roleForm.value = {
    id: row.id,
    name: row.name,
    code: row.code,
    description: row.description || '',
    sortOrder: row.sortOrder,
    remark: row.remark || '',
    status: row.status
  }
  dialogVisible.value = true
}

// 重置角色表单
const resetRoleForm = () => {
  roleForm.value = {
    name: '',
    code: '',
    description: '',
    sortOrder: 0,
    remark: '',
    status: 1
  }
}

// 提交角色表单
const submitRoleForm = async () => {
  try {
    await roleFormRef.value.validate()

    if (dialogTitle.value === '添加角色') {
      const createData: CreateRoleRequest = {
        name: roleForm.value.name,
        code: roleForm.value.code,
        description: roleForm.value.description,
        sortOrder: roleForm.value.sortOrder,
        remark: roleForm.value.remark
      }
      await createRoleApi(createData)
      showSuccessNotification({ title: '成功', message: '添加成功' })
    } else {
      const updateData: UpdateRoleRequest = {
        id: roleForm.value.id!,
        name: roleForm.value.name,
        code: roleForm.value.code,
        description: roleForm.value.description,
        sortOrder: roleForm.value.sortOrder,
        remark: roleForm.value.remark,
        status: roleForm.value.status
      }
      await updateRoleApi(updateData)
      showSuccessNotification({ title: '成功', message: '编辑成功' })
    }

    dialogVisible.value = false
    await loadRoleData()
  } catch (error) {
    console.log('验证失败或保存失败:', error)
  }
}

// 切换角色状态
const toggleRoleStatus = async (row: RoleDto) => {
  try {
    const action = row.status === 1 ? '禁用' : '启用'
    await ElMessageBox.confirm(`确认${action}角色 "${row.name}" 吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    if (row.status === 1) {
      await disableRole(row.id)
    } else {
      await enableRole(row.id)
    }

    showSuccessNotification({ title: '成功', message: `${action}成功` })
    await loadRoleData()
  } catch (error) {
    console.log('取消操作或操作失败')
  }
}

// 删除角色
const deleteRole = async (row: RoleDto) => {
  try {
    await ElMessageBox.confirm(`确认删除角色 "${row.name}" 吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    await deleteRoleApi(row.id)
    showSuccessNotification({ title: '成功', message: '删除成功' })
    await loadRoleData()
  } catch (error) {
    console.log('取消删除或删除失败')
  }
}

// 分配用户
const assignUsers = async (row: RoleDto) => {
  userForm.value = {
    roleId: row.id,
    roleName: row.name,
    selectedUserIds: []
  }

  userLoading.value = true
  userDialogVisible.value = true

  try {
    // 加载所有用户
    await loadAllUsers()

    // 获取角色已有的用户
    const response = await getRoleUserIds(row.id)
    if (response.data) {
      userForm.value.selectedUserIds = response.data
    }
  } catch (error) {
    console.error('加载角色用户失败:', error)
    showErrorNotification({ title: '错误', message: '加载角色用户失败' })
  } finally {
    userLoading.value = false
  }
}

// 提交用户分配
const submitUserForm = async () => {
  try {
    userLoading.value = true

    await assignRoleUsers({
      roleId: userForm.value.roleId,
      userIds: userForm.value.selectedUserIds
    })

    showSuccessNotification({ title: '成功', message: '分配用户成功' })
    userDialogVisible.value = false
  } catch (error) {
    console.error('分配用户失败:', error)
    showErrorNotification({ title: '错误', message: '分配用户失败' })
  } finally {
    userLoading.value = false
  }
}

// 处理分页大小变化
const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size
  loadRoleData()
}

// 处理分页页码变化
const handleCurrentChange = (page: number) => {
  pagination.value.pageNum = page
  loadRoleData()
}

onMounted(() => {
  loadRoleData()
})
</script>

<style scoped>
.role-container {
  padding: 0;
  height: 100%;
}

.role-content {
  margin: 0;
  height: 100%;
}

.role-card {
  height: 100%;
  border: none;
  border-radius: 0;
}

.card-header {
  display: flex;
  justify-content: flex-end;
  align-items: center;
}

.table-actions {
  display: flex;
  gap: 8px;
  align-items: center;
  flex-wrap: nowrap;
}

.dialog-footer {
  text-align: right;
}
</style>