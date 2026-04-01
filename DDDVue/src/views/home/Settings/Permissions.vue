<template>
  <div class="permissions-container">
    <div class="permissions-content">
      <el-card class="permissions-card">
        <template #header>
          <div class="card-header">
            <div class="header-left">
              <el-select v-model="filterModule" placeholder="按模块筛选" clearable style="width: 150px" @change="handleFilterChange">
                <el-option v-for="module in moduleOptions" :key="module" :label="module" :value="module" />
              </el-select>
            </div>
            <el-button v-if="hasBtn('permission:add')" class="button" type="primary" @click="addPermission">添加权限</el-button>
          </div>
        </template>
        <el-table :data="permissionList" style="width: 100%" :header-cell-style="{ background: '#f5f7fa', color: '#333' }"
          height="calc(100% - 120px)">
          <el-table-column prop="code" label="权限编码" min-width="150" />
          <el-table-column prop="name" label="权限名称" min-width="120" />
          <el-table-column prop="module" label="所属模块" width="100">
            <template #default="{ row }">
              <el-tag type="info" effect="plain">{{ row.module }}</el-tag>
            </template>
          </el-table-column>
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
          <el-table-column label="操作" width="200" fixed="right">
            <template #default="{ row }">
              <div class="table-actions">
                <el-button v-if="hasBtn('permission:edit')" size="small" @click="editPermission(row)">编辑</el-button>
                <el-button v-if="hasBtn('permission:enable') && row.status !== 1" size="small" type="success" @click="togglePermissionStatus(row)">启用</el-button>
                <el-button v-if="hasBtn('permission:disable') && row.status === 1" size="small" type="warning" @click="togglePermissionStatus(row)">禁用</el-button>
                <el-button v-if="hasBtn('permission:delete')" size="small" type="danger" @click="deletePermissionItem(row)">删除</el-button>
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

    <!-- 权限编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px" :destroy-on-close="true">
      <el-form :model="permissionForm" :rules="permissionRules" ref="permissionFormRef" label-width="100px">
        <el-form-item label="权限编码" prop="code">
          <el-input v-model="permissionForm.code" placeholder="如：user:add" :disabled="dialogTitle === '编辑权限'" />
        </el-form-item>
        <el-form-item label="权限名称" prop="name">
          <el-input v-model="permissionForm.name" placeholder="如：添加用户" />
        </el-form-item>
        <el-form-item label="所属模块" prop="module">
          <el-select v-model="permissionForm.module" placeholder="请选择模块" style="width: 100%" :disabled="dialogTitle === '编辑权限'">
            <el-option v-for="module in moduleOptions" :key="module" :label="module" :value="module" />
          </el-select>
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="permissionForm.description" type="textarea" :rows="2" placeholder="请输入权限描述" />
        </el-form-item>
        <el-form-item label="排序号">
          <el-input-number v-model="permissionForm.sortOrder" :min="0" :max="999" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitPermissionForm">确定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessageBox } from 'element-plus'
import * as permissionApi from '@/api/role'
import type { PermissionDto, CreatePermissionRequest, UpdatePermissionRequest } from '@/api/role'
import type { PageParams } from '@/api/role'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'
import { getItem, setItem, StorageKeys } from '@/utils/storage'
import { useButtons } from '@/utils/buttons'

// 解构导入 API 函数
const {
  getPermissions,
  createPermission: createPermissionApi,
  updatePermission: updatePermissionApi,
  deletePermission: deletePermissionApi,
  enablePermission,
  disablePermission
} = permissionApi

// 按钮管理
const { hasBtn } = useButtons('settings-permissions')

// 权限表单类型
interface PermissionForm {
  id?: string
  code: string
  name: string
  module: string
  description?: string
  sortOrder: number
}

// 分页数据
interface Pagination {
  pageNum: number
  pageSize: number
  total: number
}

// 响应式数据
const permissionList = ref<PermissionDto[]>([])
const filterModule = ref('')
const dialogVisible = ref(false)
const dialogTitle = ref('添加权限')
const permissionFormRef = ref()
const permissionForm = ref<PermissionForm>({
  code: '',
  name: '',
  module: '',
  description: '',
  sortOrder: 0
})

const pagination = ref<Pagination>({
  pageNum: 1,
  pageSize: 10,
  total: 0
})

// 模块选项（动态获取）
const moduleOptions = ref<string[]>([])

// 加载模块选项
const loadModuleOptions = async () => {
  try {
    const response = await permissionApi.getAllEnabledPermissions()
    if (response.data) {
      // 从权限数据中提取模块列表（去重并排序）
      const modules = [...new Set(response.data.map(p => p.module))].sort()
      moduleOptions.value = modules
    }
  } catch (error) {
    console.error('加载模块选项失败:', error)
  }
}

// 权限表单验证规则
const permissionRules = {
  code: [
    { required: true, message: '请输入权限编码', trigger: 'blur' },
    { min: 2, max: 100, message: '权限编码长度在 2 到 100 个字符', trigger: 'blur' },
    { pattern: /^[a-z_]+:[a-z_]+$/, message: '权限编码格式：模块:操作（如 user:add）', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入权限名称', trigger: 'blur' },
    { min: 2, max: 50, message: '权限名称长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  module: [
    { required: true, message: '请选择所属模块', trigger: 'change' }
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

// 加载权限数据
const loadPermissionData = async (forceRefresh: boolean = false) => {
  try {
    const params: PageParams = {
      pageNum: pagination.value.pageNum,
      pageSize: pagination.value.pageSize
    }

    // 如果有模块筛选，添加到参数中
    if (filterModule.value) {
      params.module = filterModule.value
    }

    // 生成缓存键（包含分页参数和筛选条件）
    const cacheKey = `${StorageKeys.List}_permission_${params.pageNum}_${params.pageSize}_${filterModule.value || 'all'}`

    // 如果不是强制刷新，优先从缓存获取
    if (!forceRefresh) {
      const cachedData = getItem<{ list: PermissionDto[], total: number }>(cacheKey)
      if (cachedData) {
        permissionList.value = cachedData.list || []
        pagination.value.total = cachedData.total || 0
        return
      }
    }

    // 缓存不存在或强制刷新，从 API 获取
    const response = await getPermissions(params)
    if (response.data) {
      permissionList.value = response.data.list || []
      pagination.value.total = response.data.total || 0

      // 存入缓存
      setItem(cacheKey, {
        list: response.data.list,
        total: response.data.total
      })
    }
  } catch (error) {
    console.error('加载权限数据失败:', error)
    showErrorNotification({ title: '错误', message: '加载权限数据失败' })
  }
}

// 处理筛选变化
const handleFilterChange = () => {
  pagination.value.pageNum = 1
  loadPermissionData(true)  // 筛选变化时强制刷新
}

// 添加权限
const addPermission = () => {
  dialogTitle.value = '添加权限'
  resetPermissionForm()
  dialogVisible.value = true
}

// 编辑权限
const editPermission = (row: PermissionDto) => {
  dialogTitle.value = '编辑权限'
  permissionForm.value = {
    id: row.id,
    code: row.code,
    name: row.name,
    module: row.module,
    description: row.description || '',
    sortOrder: row.sortOrder
  }
  dialogVisible.value = true
}

// 重置权限表单
const resetPermissionForm = () => {
  permissionForm.value = {
    code: '',
    name: '',
    module: '',
    description: '',
    sortOrder: 0
  }
}

// 提交权限表单
const submitPermissionForm = async () => {
  try {
    await permissionFormRef.value.validate()

    if (dialogTitle.value === '添加权限') {
      const createData: CreatePermissionRequest = {
        code: permissionForm.value.code,
        name: permissionForm.value.name,
        module: permissionForm.value.module,
        description: permissionForm.value.description,
        sortOrder: permissionForm.value.sortOrder
      }
      await createPermissionApi(createData)
      showSuccessNotification({ title: '成功', message: '添加成功' })
    } else {
      const updateData: UpdatePermissionRequest = {
        id: permissionForm.value.id!,
        name: permissionForm.value.name,
        description: permissionForm.value.description,
        sortOrder: permissionForm.value.sortOrder
      }
      await updatePermissionApi(updateData)
      showSuccessNotification({ title: '成功', message: '编辑成功' })
    }

    dialogVisible.value = false
    await loadPermissionData(true)  // 强制刷新缓存
  } catch (error) {
    console.log('验证失败或保存失败:', error)
  }
}

// 切换权限状态
const togglePermissionStatus = async (row: PermissionDto) => {
  try {
    const action = row.status === 1 ? '禁用' : '启用'
    await ElMessageBox.confirm(`确认${action}权限 "${row.name}" 吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    if (row.status === 1) {
      await disablePermission(row.id)
    } else {
      await enablePermission(row.id)
    }

    showSuccessNotification({ title: '成功', message: `${action}成功` })
    await loadPermissionData(true)  // 强制刷新缓存
  } catch (error) {
    console.log('取消操作或操作失败')
  }
}

// 删除权限
const deletePermissionItem = async (row: PermissionDto) => {
  try {
    await ElMessageBox.confirm(`确认删除权限 "${row.name}" 吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    await deletePermissionApi(row.id)
    showSuccessNotification({ title: '成功', message: '删除成功' })
    await loadPermissionData(true)  // 强制刷新缓存
  } catch (error) {
    console.log('取消删除或删除失败')
  }
}

// 处理分页大小变化
const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size
  loadPermissionData()
}

// 处理分页页码变化
const handleCurrentChange = (page: number) => {
  pagination.value.pageNum = page
  loadPermissionData()
}

onMounted(() => {
  loadModuleOptions()
  loadPermissionData()
})
</script>

<style scoped>
.permissions-container {
  padding: 0;
  height: 100%;
}

.permissions-content {
  margin: 0;
  height: 100%;
}

.permissions-card {
  height: 100%;
  border: none;
  border-radius: 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-left {
  display: flex;
  gap: 12px;
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
