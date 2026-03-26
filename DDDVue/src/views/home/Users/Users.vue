<template>
  <div class="users-container">
    <div class="users-content">
      <el-card class="users-card">
        <template #header>
          <div class="card-header">
            <el-button v-if="hasPermission(PermissionCodes.USER_ADD)" class="button" type="primary" @click="addUser">添加用户</el-button>
          </div>
        </template>
        <el-table :data="userList" style="width: 100%" :header-cell-style="{ background: '#f5f7fa', color: '#333' }"
          height="calc(100% - 120px)">
          <el-table-column prop="userName" label="用户名" min-width="120" />
          <el-table-column prop="realName" label="真实姓名" min-width="120">
            <template #default="{ row }">
              {{ row.realName || '-' }}
            </template>
          </el-table-column>
          <el-table-column prop="email" label="邮箱" min-width="180" />
          <el-table-column prop="phoneNumber" label="手机号码" min-width="130">
            <template #default="{ row }">
              {{ row.phoneNumber || '-' }}
            </template>
          </el-table-column>
          <el-table-column prop="status" label="状态" width="100">
            <template #default="{ row }">
              <el-tag :type="row.status === 1 ? 'success' : 'danger'" effect="dark">
                {{ row.status === 1 ? '启用' : '禁用' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="lastLoginTime" label="最后登录时间" min-width="170">
            <template #default="{ row }">
              {{ row.lastLoginTime ? formatDateTime(row.lastLoginTime) : '-' }}
            </template>
          </el-table-column>
          <el-table-column prop="createdAt" label="创建时间" min-width="170">
            <template #default="{ row }">
              {{ formatDateTime(row.createdAt) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="400" fixed="right">
            <template #default="{ row }">
              <div class="table-actions">
                <el-button v-if="hasPermission(PermissionCodes.USER_EDIT)" size="small" @click="editUser(row)">编辑</el-button>
                <el-button v-if="hasPermission(PermissionCodes.USER_RESET_PASSWORD)" size="small" type="info" @click="openResetPwdDialog(row)">重置密码</el-button>
                <el-button v-if="hasPermission(PermissionCodes.USER_ASSIGN_ROLE)" size="small" type="primary" @click="configRole(row)">配置角色</el-button>
                <el-button v-if="hasAnyPermission([PermissionCodes.USER_ENABLE, PermissionCodes.USER_DISABLE])" size="small" :type="row.status === 1 ? 'warning' : 'success'" @click="toggleUserStatus(row)">
                  {{ row.status === 1 ? '禁用' : '启用' }}
                </el-button>
                <el-button v-if="hasPermission(PermissionCodes.USER_DELETE)" size="small" type="danger" @click="deleteUser(row)">删除</el-button>
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

    <!-- 用户编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="600px" :destroy-on-close="true">
      <el-form :model="userForm" :rules="userRules" ref="userFormRef" label-width="100px">
        <el-form-item label="用户名" prop="userName" v-if="dialogTitle === '添加用户'">
          <el-input v-model="userForm.userName" placeholder="请输入用户名" />
        </el-form-item>
        <el-form-item label="用户名" v-else>
          <el-input v-model="userForm.userName" disabled />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="userForm.email" placeholder="请输入邮箱" />
        </el-form-item>
        <el-form-item label="密码" prop="password" v-if="dialogTitle === '添加用户'">
          <el-input v-model="userForm.password" type="password" placeholder="请输入密码" show-password />
        </el-form-item>
        <el-form-item label="真实姓名">
          <el-input v-model="userForm.realName" placeholder="请输入真实姓名" />
        </el-form-item>
        <el-form-item label="手机号码" prop="phoneNumber">
          <el-input v-model="userForm.phoneNumber" placeholder="请输入手机号码" />
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="userForm.remark" type="textarea" :rows="3" placeholder="请输入备注" />
        </el-form-item>
        <el-form-item label="状态" v-if="dialogTitle === '编辑用户'">
          <el-switch v-model="userForm.status" :active-value="1" :inactive-value="0" inline-prompt active-text="启用"
            inactive-text="禁用" active-color="#13ce66" inactive-color="#ff4949" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitUserForm">确定</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 重置密码对话框 -->
    <el-dialog v-model="resetPwdDialogVisible" title="重置密码" width="400px" :destroy-on-close="true">
      <el-form :model="resetPwdForm" :rules="resetPwdRules" ref="resetPwdFormRef" label-width="100px">
        <el-form-item label="用户名">
          <el-input v-model="resetPwdForm.userName" disabled />
        </el-form-item>
        <el-form-item label="新密码" prop="newPassword">
          <el-input v-model="resetPwdForm.newPassword" type="password" placeholder="请输入新密码" show-password />
        </el-form-item>
        <el-form-item label="确认密码" prop="confirmPassword">
          <el-input v-model="resetPwdForm.confirmPassword" type="password" placeholder="请再次输入新密码" show-password />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="resetPwdDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitResetPwd">确定</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 配置角色对话框 -->
    <el-dialog v-model="roleDialogVisible" title="配置角色" width="500px" :destroy-on-close="true">
      <el-form label-width="100px">
        <el-form-item label="用户名">
          <el-input v-model="roleForm.userName" disabled />
        </el-form-item>
        <el-form-item label="角色">
          <el-checkbox-group v-model="roleForm.selectedRoleIds" v-loading="roleLoading">
            <el-checkbox v-for="role in allRoles" :key="role.id" :label="role.id" :value="role.id">
              {{ role.name }}
            </el-checkbox>
          </el-checkbox-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="roleDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitRoleForm" :loading="roleLoading">确定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessageBox } from 'element-plus'
import * as userApi from '@/api/user'
import * as roleApi from '@/api/role'
import type { UserDto, CreateUserRequest, UpdateUserRequest, RoleDto } from '@/api/index'
import { aesEncrypt } from '@/utils/crypto'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'
import { getItem, setItem, StorageKeys, hasPermission, hasAnyPermission, PermissionCodes } from '@/utils/storage'

// 解构导入 API 函数
const { getUsers, createUser: createUserApi, updateUser: updateUserApi, deleteUser: deleteUserApi, enableUser, disableUser, resetPassword } = userApi
const { getUserRoleIds, assignUserRoles, getEnabledRoles } = roleApi

// 用户表单类型
interface UserForm {
  id?: string
  userName: string
  email: string
  password: string
  realName?: string
  phoneNumber?: string
  remark?: string
  status: number
}

// 重置密码表单类型
interface ResetPwdForm {
  id: string
  userName: string
  newPassword: string
  confirmPassword: string
}

// 分页数据
interface Pagination {
  pageNum: number
  pageSize: number
  total: number
}

// 响应式数据
const userList = ref<UserDto[]>([])
const dialogVisible = ref(false)
const dialogTitle = ref('添加用户')
const userFormRef = ref()
const userForm = ref<UserForm>({
  userName: '',
  email: '',
  password: '',
  realName: '',
  phoneNumber: '',
  remark: '',
  status: 1
})

const resetPwdDialogVisible = ref(false)
const resetPwdFormRef = ref()
const resetPwdForm = ref<ResetPwdForm>({
  id: '',
  userName: '',
  newPassword: '',
  confirmPassword: ''
})

const pagination = ref<Pagination>({
  pageNum: 1,
  pageSize: 10,
  total: 0
})

// 角色配置相关数据
const roleDialogVisible = ref(false)
const roleLoading = ref(false)
const allRoles = ref<RoleDto[]>([])
const roleForm = ref<{
  userId: string
  userName: string
  selectedRoleIds: string[]
}>({
  userId: '',
  userName: '',
  selectedRoleIds: []
})

// 用户表单验证规则
const userRules = {
  userName: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, max: 50, message: '用户名长度在 3 到 50 个字符', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, max: 50, message: '密码长度在 6 到 50 个字符', trigger: 'blur' },
    {
      pattern: /^(?=.*[a-zA-Z])(?=.*\d).+$/,
      message: '密码必须包含字母和数字',
      trigger: 'blur'
    }
  ],
  phoneNumber: [
    {
      pattern: /^1[3-9]\d{9}$/,
      message: '请输入正确的手机号码',
      trigger: 'blur'
    }
  ]
}

// 重置密码表单验证规则
const resetPwdRules = {
  newPassword: [
    { required: true, message: '请输入新密码', trigger: 'blur' },
    { min: 6, max: 50, message: '密码长度在 6 到 50 个字符', trigger: 'blur' },
    {
      pattern: /^(?=.*[a-zA-Z])(?=.*\d).+$/,
      message: '密码必须包含字母和数字',
      trigger: 'blur'
    }
  ],
  confirmPassword: [
    { required: true, message: '请再次输入新密码', trigger: 'blur' },
    {
      validator: (_rule: any, value: string, callback: any) => {
        if (value !== resetPwdForm.value.newPassword) {
          callback(new Error('两次输入的密码不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
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

// 加载用户数据
const loadUserData = async () => {
  try {
    const params = {
      pageNum: pagination.value.pageNum,
      pageSize: pagination.value.pageSize
    }

    // 生成缓存键（包含分页参数）
    const cacheKey = `${StorageKeys.List}_user_${params.pageNum}_${params.pageSize}`

    // 优先从缓存获取
    const cachedData = getItem<{ list: UserDto[], total: number }>(cacheKey)
    if (cachedData) {
      userList.value = cachedData.list || []
      pagination.value.total = cachedData.total || 0
      return
    }

    // 缓存不存在，从 API 获取
    const response = await getUsers(params)
    if (response.data) {
      userList.value = response.data.list || []
      pagination.value.total = response.data.total || 0

      // 存入缓存
      setItem(cacheKey, {
        list: response.data.list,
        total: response.data.total
      })
    }
  } catch (error) {
    console.error('加载用户数据失败:', error)
    showErrorNotification({ title: '错误', message: '加载用户数据失败' })
  }
}

// 添加用户
const addUser = () => {
  dialogTitle.value = '添加用户'
  resetUserForm()
  dialogVisible.value = true
}

// 编辑用户
const editUser = (row: UserDto) => {
  dialogTitle.value = '编辑用户'
  userForm.value = {
    id: row.id,
    userName: row.userName,
    email: row.email,
    password: '',
    realName: row.realName || '',
    phoneNumber: row.phoneNumber || '',
    remark: row.remark || '',
    status: row.status
  }
  dialogVisible.value = true
}

// 重置用户表单
const resetUserForm = () => {
  userForm.value = {
    userName: '',
    email: '',
    password: '',
    realName: '',
    phoneNumber: '',
    remark: '',
    status: 1
  }
}

// 提交用户表单
const submitUserForm = async () => {
  try {
    await userFormRef.value.validate()

    if (dialogTitle.value === '添加用户') {
      // 加密密码
      const createData: CreateUserRequest = {
        userName: userForm.value.userName,
        email: userForm.value.email,
        password: aesEncrypt(userForm.value.password),
        realName: userForm.value.realName,
        phoneNumber: userForm.value.phoneNumber,
        remark: userForm.value.remark
      }
      await createUserApi(createData)
      showSuccessNotification({ title: '成功', message: '添加成功' })
    } else {
      // 更新用户
      const updateData: UpdateUserRequest = {
        id: userForm.value.id!,
        email: userForm.value.email,
        realName: userForm.value.realName,
        phoneNumber: userForm.value.phoneNumber,
        remark: userForm.value.remark,
        status: userForm.value.status
      }
      await updateUserApi(updateData)
      showSuccessNotification({ title: '成功', message: '编辑成功' })
    }

    dialogVisible.value = false
    await loadUserData()
  } catch (error) {
    console.log('验证失败或保存失败:', error)
  }
}

// 切换用户状态
const toggleUserStatus = async (row: UserDto) => {
  try {
    const action = row.status === 1 ? '禁用' : '启用'
    await ElMessageBox.confirm(`确认${action}用户 "${row.userName}" 吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    if (row.status === 1) {
      await disableUser(row.id)
    } else {
      await enableUser(row.id)
    }

    showSuccessNotification({ title: '成功', message: `${action}成功` })
    await loadUserData()
  } catch (error) {
    console.log('取消操作或操作失败')
  }
}

// 删除用户
const deleteUser = async (row: UserDto) => {
  try {
    await ElMessageBox.confirm(`确认删除用户 "${row.userName}" 吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    await deleteUserApi(row.id)
    showSuccessNotification({ title: '成功', message: '删除成功' })
    await loadUserData()
  } catch (error) {
    console.log('取消删除或删除失败')
  }
}

// 打开重置密码对话框
const openResetPwdDialog = (row: UserDto) => {
  resetPwdForm.value = {
    id: row.id,
    userName: row.userName,
    newPassword: '',
    confirmPassword: ''
  }
  resetPwdDialogVisible.value = true
}

// 提交重置密码
const submitResetPwd = async () => {
  try {
    await resetPwdFormRef.value.validate()

    await resetPassword({
      id: resetPwdForm.value.id,
      newPassword: aesEncrypt(resetPwdForm.value.newPassword)
    })

    showSuccessNotification({ title: '成功', message: '密码重置成功' })
    resetPwdDialogVisible.value = false
  } catch (error) {
    console.log('验证失败或重置失败:', error)
  }
}

// 处理分页大小变化
const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size
  loadUserData()
}

// 处理分页页码变化
const handleCurrentChange = (page: number) => {
  pagination.value.pageNum = page
  loadUserData()
}

// 加载所有启用的角色
const loadAllRoles = async () => {
  try {
    const response = await getEnabledRoles()
    if (response.data) {
      allRoles.value = response.data
    }
  } catch (error) {
    console.error('加载角色列表失败:', error)
  }
}

// 配置角色
const configRole = async (row: UserDto) => {
  roleForm.value = {
    userId: row.id,
    userName: row.userName,
    selectedRoleIds: []
  }

  roleLoading.value = true
  roleDialogVisible.value = true

  try {
    // 加载角色列表
    await loadAllRoles()

    // 获取用户当前的角色
    const response = await getUserRoleIds(row.id)
    if (response.data) {
      roleForm.value.selectedRoleIds = response.data
    }
  } catch (error) {
    console.error('加载用户角色失败:', error)
    showErrorNotification({ title: '错误', message: '加载用户角色失败' })
  } finally {
    roleLoading.value = false
  }
}

// 提交角色配置
const submitRoleForm = async () => {
  try {
    roleLoading.value = true

    await assignUserRoles({
      userId: roleForm.value.userId,
      roleIds: roleForm.value.selectedRoleIds
    })

    showSuccessNotification({ title: '成功', message: '角色配置成功' })
    roleDialogVisible.value = false
    await loadUserData()  // 刷新数据
  } catch (error) {
    console.error('配置角色失败:', error)
    showErrorNotification({ title: '错误', message: '配置角色失败' })
  } finally {
    roleLoading.value = false
  }
}

onMounted(() => {
  loadUserData()
})
</script>

<style scoped>
.users-container {
  padding: 0;
  height: 100%;
}

.users-content {
  margin: 0;
  height: 100%;
}

.users-card {
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