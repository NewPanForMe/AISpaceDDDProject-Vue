<template>
  <div class="button-container">
    <div class="button-content">
      <el-card class="button-card">
        <template #header>
          <div class="card-header">
            <div class="header-left">
              <el-select v-model="filterMenuId" placeholder="按菜单筛选" clearable style="width: 200px" @change="handleFilterChange">
                <el-option v-for="menu in menuOptions" :key="menu.id" :label="menu.name" :value="menu.id" />
              </el-select>
            </div>
            <el-button v-if="hasBtn('button:add')" type="primary" @click="addButton">添加按钮</el-button>
          </div>
        </template>
        <el-table :data="buttonList" style="width: 100%" :header-cell-style="{ background: '#f5f7fa', color: '#333' }"
          height="calc(100% - 120px)">
          <el-table-column prop="name" label="按钮名称" min-width="120" />
          <el-table-column prop="code" label="按钮编码" min-width="150" />
          <el-table-column prop="menuName" label="所属菜单" min-width="120">
            <template #default="{ row }">
              {{ row.menuName || '-' }}
            </template>
          </el-table-column>
          <el-table-column prop="permissionCode" label="权限编码" min-width="150">
            <template #default="{ row }">
              {{ row.permissionCode || '-' }}
            </template>
          </el-table-column>
          <el-table-column prop="icon" label="图标" width="100">
            <template #default="{ row }">
              {{ row.icon || '-' }}
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
          <el-table-column prop="description" label="描述" min-width="150">
            <template #default="{ row }">
              {{ row.description || '-' }}
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
                <el-button v-if="hasBtn('button:edit')" size="small" @click="editButton(row)">编辑</el-button>
                <el-button v-if="hasBtn('button:enable') && row.status !== 1" size="small" type="success" @click="toggleButtonStatus(row)">启用</el-button>
                <el-button v-if="hasBtn('button:disable') && row.status === 1" size="small" type="warning" @click="toggleButtonStatus(row)">禁用</el-button>
                <el-button v-if="hasBtn('button:delete')" size="small" type="danger" @click="deleteButtonItem(row)">删除</el-button>
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

    <!-- 按钮编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="550px" :destroy-on-close="true">
      <el-form :model="buttonForm" :rules="buttonRules" ref="buttonFormRef" label-width="100px">
        <el-form-item label="按钮名称" prop="name">
          <el-input v-model="buttonForm.name" placeholder="如：添加用户" />
        </el-form-item>
        <el-form-item label="按钮编码" prop="code">
          <el-input v-model="buttonForm.code" placeholder="如：user:add" :disabled="dialogTitle === '编辑按钮'" />
        </el-form-item>
        <el-form-item label="所属菜单" prop="menuId">
          <el-select v-model="buttonForm.menuId" placeholder="请选择菜单" style="width: 100%" :disabled="dialogTitle === '编辑按钮'">
            <el-option v-for="menu in menuOptions" :key="menu.id" :label="menu.name" :value="menu.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="权限编码">
          <el-input v-model="buttonForm.permissionCode" placeholder="如：user:add（与权限管理关联）" />
        </el-form-item>
        <el-form-item label="图标">
          <el-input v-model="buttonForm.icon" placeholder="如：Plus" />
        </el-form-item>
        <el-form-item label="排序号">
          <el-input-number v-model="buttonForm.sortOrder" :min="0" :max="999" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="buttonForm.description" type="textarea" :rows="2" placeholder="请输入按钮描述" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitButtonForm">确定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessageBox } from 'element-plus'
import { http } from '@/utils/http'
import api from '@/api/index'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'
import { getItem, setItem, StorageKeys } from '@/utils/storage'
import { useButtons } from '@/utils/buttons'

// 按钮管理
const { hasBtn, hasAnyBtn } = useButtons('settings-button')

// 按钮数据类型
interface ButtonDto {
  id: string
  name: string
  code: string
  menuId: string
  menuName?: string
  permissionCode?: string
  icon?: string
  sortOrder: number
  status: number
  description?: string
  createdAt: string
  updatedAt?: string
}

// 菜单数据类型
interface MenuDto {
  id: string
  name: string
  path: string
}

// 分页数据
interface Pagination {
  pageNum: number
  pageSize: number
  total: number
}

// 按钮表单类型
interface ButtonForm {
  id?: string
  name: string
  code: string
  menuId: string
  permissionCode?: string
  icon?: string
  sortOrder: number
  description?: string
}

// 响应式数据
const buttonList = ref<ButtonDto[]>([])
const menuOptions = ref<MenuDto[]>([])
const filterMenuId = ref<string>('')
const dialogVisible = ref(false)
const dialogTitle = ref('添加按钮')
const buttonFormRef = ref()
const buttonForm = ref<ButtonForm>({
  name: '',
  code: '',
  menuId: '',
  permissionCode: '',
  icon: '',
  sortOrder: 0,
  description: ''
})

const pagination = ref<Pagination>({
  pageNum: 1,
  pageSize: 10,
  total: 0
})

// 按钮表单验证规则
const buttonRules = {
  name: [
    { required: true, message: '请输入按钮名称', trigger: 'blur' },
    { min: 2, max: 50, message: '按钮名称长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  code: [
    { required: true, message: '请输入按钮编码', trigger: 'blur' },
    { min: 2, max: 100, message: '按钮编码长度在 2 到 100 个字符', trigger: 'blur' },
    { pattern: /^[a-z_]+:[a-z_]+$/, message: '按钮编码格式：模块:操作（如 user:add）', trigger: 'blur' }
  ],
  menuId: [
    { required: true, message: '请选择所属菜单', trigger: 'change' }
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

// 加载菜单选项
const loadMenuOptions = async () => {
  try {
    const response = await http.get<{ success: boolean; data?: MenuDto[] }>(api.Menu.GetTreeMenusAsync)
    if (response.success && response.data) {
      // 扁平化菜单树
      const flattenMenus = (menus: MenuDto[]): MenuDto[] => {
        const result: MenuDto[] = []
        menus.forEach(menu => {
          result.push({ id: menu.id, name: menu.name, path: menu.path })
        })
        return result
      }
      menuOptions.value = flattenMenus(response.data)
    }
  } catch (error) {
    console.error('加载菜单选项失败:', error)
  }
}

// 加载按钮数据
const loadButtonData = async (forceRefresh: boolean = false) => {
  try {
    const params: Record<string, any> = {
      pageNum: pagination.value.pageNum,
      pageSize: pagination.value.pageSize
    }

    // 如果有菜单筛选
    if (filterMenuId.value) {
      params.menuId = filterMenuId.value
    }

    // 生成缓存键
    const cacheKey = `${StorageKeys.List}_button_${params.pageNum}_${params.pageSize}_${filterMenuId.value || 'all'}`

    // 如果不是强制刷新，优先从缓存获取
    if (!forceRefresh) {
      const cachedData = getItem<{ list: ButtonDto[], total: number }>(cacheKey)
      if (cachedData) {
        buttonList.value = cachedData.list || []
        pagination.value.total = cachedData.total || 0
        return
      }
    }

    // 从 API 获取
    const response = await http.get<{ success: boolean; data?: { list: ButtonDto[]; total: number } }>(
      api.Button.GetButtonsAsync,
      { params }
    )

    if (response.success && response.data) {
      buttonList.value = response.data.list || []
      pagination.value.total = response.data.total || 0

      // 存入缓存
      setItem(cacheKey, {
        list: response.data.list,
        total: response.data.total
      })
    }
  } catch (error) {
    console.error('加载按钮数据失败:', error)
    showErrorNotification({ title: '错误', message: '加载按钮数据失败' })
  }
}

// 处理筛选变化
const handleFilterChange = () => {
  pagination.value.pageNum = 1
  loadButtonData(true)
}

// 添加按钮
const addButton = () => {
  dialogTitle.value = '添加按钮'
  resetButtonForm()
  dialogVisible.value = true
}

// 编辑按钮
const editButton = (row: ButtonDto) => {
  dialogTitle.value = '编辑按钮'
  buttonForm.value = {
    id: row.id,
    name: row.name,
    code: row.code,
    menuId: row.menuId,
    permissionCode: row.permissionCode || '',
    icon: row.icon || '',
    sortOrder: row.sortOrder,
    description: row.description || ''
  }
  dialogVisible.value = true
}

// 重置按钮表单
const resetButtonForm = () => {
  buttonForm.value = {
    name: '',
    code: '',
    menuId: '',
    permissionCode: '',
    icon: '',
    sortOrder: 0,
    description: ''
  }
}

// 提交按钮表单
const submitButtonForm = async () => {
  try {
    await buttonFormRef.value.validate()

    if (dialogTitle.value === '添加按钮') {
      const response = await http.post(api.Button.CreateButtonAsync, {
        name: buttonForm.value.name,
        code: buttonForm.value.code,
        menuId: buttonForm.value.menuId,
        permissionCode: buttonForm.value.permissionCode || undefined,
        icon: buttonForm.value.icon || undefined,
        sortOrder: buttonForm.value.sortOrder,
        description: buttonForm.value.description || undefined
      })
      if (response.success) {
        showSuccessNotification({ title: '成功', message: '添加成功' })
      } else {
        showErrorNotification({ title: '失败', message: response.message || '添加失败' })
        return
      }
    } else {
      const response = await http.put(api.Button.UpdateButtonAsync, {
        id: buttonForm.value.id,
        name: buttonForm.value.name,
        permissionCode: buttonForm.value.permissionCode || undefined,
        icon: buttonForm.value.icon || undefined,
        sortOrder: buttonForm.value.sortOrder,
        description: buttonForm.value.description || undefined
      })
      if (response.success) {
        showSuccessNotification({ title: '成功', message: '编辑成功' })
      } else {
        showErrorNotification({ title: '失败', message: response.message || '编辑失败' })
        return
      }
    }

    dialogVisible.value = false
    await loadButtonData(true)
  } catch (error) {
    console.log('验证失败或保存失败:', error)
  }
}

// 切换按钮状态
const toggleButtonStatus = async (row: ButtonDto) => {
  try {
    const action = row.status === 1 ? '禁用' : '启用'
    await ElMessageBox.confirm(`确认${action}按钮 "${row.name}" 吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    const url = row.status === 1 ? api.Button.DisableButtonAsync : api.Button.EnableButtonAsync
    const response = await http.post(url, null, { params: { id: row.id } })

    if (response.success) {
      showSuccessNotification({ title: '成功', message: `${action}成功` })
      await loadButtonData(true)
    } else {
      showErrorNotification({ title: '失败', message: response.message || `${action}失败` })
    }
  } catch (error) {
    console.log('取消操作或操作失败')
  }
}

// 删除按钮
const deleteButtonItem = async (row: ButtonDto) => {
  try {
    await ElMessageBox.confirm(`确认删除按钮 "${row.name}" 吗？`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    const response = await http.delete(api.Button.DeleteButtonAsync, { params: { id: row.id } })

    if (response.success) {
      showSuccessNotification({ title: '成功', message: '删除成功' })
      await loadButtonData(true)
    } else {
      showErrorNotification({ title: '失败', message: response.message || '删除失败' })
    }
  } catch (error) {
    console.log('取消删除或删除失败')
  }
}

// 处理分页大小变化
const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size
  loadButtonData()
}

// 处理分页页码变化
const handleCurrentChange = (page: number) => {
  pagination.value.pageNum = page
  loadButtonData()
}

onMounted(() => {
  loadMenuOptions()
  loadButtonData()
})
</script>

<style scoped>
.button-container {
  padding: 0;
  height: 100%;
}

.button-content {
  margin: 0;
  height: 100%;
}

.button-card {
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