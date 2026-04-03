<template>
  <div class="menu-container">
    <div class="menu-content">
      <el-card class="menu-card">
        <template #header>
          <div class="card-header">
            <el-button v-if="hasBtn('menu:add')" class="button" type="primary" @click="addMenu">添加菜单</el-button>
          </div>
        </template>
        <el-table :data="menuList" style="width: 100%" row-key="id"
          :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
          :header-cell-style="{ background: '#f5f7fa', color: '#333' }" height="calc(100% - 120px)">
          <el-table-column prop="name" label="菜单名称" min-width="150" />
          <el-table-column prop="path" label="路由路径" min-width="150" />
          <el-table-column prop="component" label="组件路径" min-width="180" />
          <el-table-column prop="icon" label="图标" width="120">
            <template #default="{ row }">
              <div class="icon-cell" v-if="row.icon">
                <el-icon v-if="row.icon">
                  <component :is="row.icon" />
                </el-icon>
                <span class="icon-name">{{ row.icon }}</span>
              </div>
              <span v-else class="no-icon">无</span>
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
          <el-table-column label="操作" width="240" fixed="right">
            <template #default="{ row }">
              <div class="table-actions">
                <el-button v-if="hasBtn('menu:edit')" size="small" @click="editMenu(row)">编辑</el-button>
                <el-button v-if="hasBtn('menu:delete')" size="small" type="danger" @click="deleteMenu(row.id)">删除</el-button>
                <el-button v-if="hasBtn('menu:add_child')" size="small" type="primary" @click="addChildMenu(row)">子菜单</el-button>
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

    <!-- 菜单编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="600px" :destroy-on-close="true">
      <el-form :model="menuForm" :rules="menuRules" ref="menuFormRef" label-width="100px">
        <el-form-item label="菜单名称" prop="name">
          <el-input v-model="menuForm.name" placeholder="请输入菜单名称" />
        </el-form-item>
        <el-form-item label="上级菜单" prop="parentId">
          <el-tree-select v-model="menuForm.parentId" :data="menuTreeData"
            :props="{ label: 'name', value: 'id', children: 'children' }" :check-strictly="true" placeholder="请选择上级菜单"
            clearable />
        </el-form-item>
        <el-form-item label="路由路径" prop="path">
          <el-input v-model="menuForm.path" placeholder="请输入路由路径" />
        </el-form-item>
        <el-form-item label="组件路径" prop="component">
          <el-input v-model="menuForm.component" placeholder="请输入组件路径" />
        </el-form-item>
        <el-form-item label="图标">
          <el-select v-model="menuForm.icon" placeholder="请选择图标" clearable style="width: 100%">
            <template #prefix>
              <el-icon v-if="menuForm.icon" :size="20">
                <component :is="menuForm.icon" />
              </el-icon>
            </template>
            <el-option v-for="icon in allIcons" :key="icon" :value="icon" :label="icon">
              <div class="icon-option">
                <el-icon :size="32">
                  <component :is="icon" />
                </el-icon>
                <span class="icon-name">{{ icon }}</span>
              </div>
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="排序">
          <el-input-number v-model="menuForm.sortOrder" :min="0" />
        </el-form-item>
        <el-form-item label="状态">
          <el-switch v-model="menuForm.status" :active-value="1" :inactive-value="0" inline-prompt active-text="启用"
            inactive-text="禁用" active-color="#13ce66" inactive-color="#ff4949" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitMenuForm">确定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessageBox } from 'element-plus'
import * as menuApi from '@/api/menu'
import type { PageParams, MenuTree } from '@/api/menu'
import * as Icons from '@element-plus/icons-vue'
import { removeItem, setItem, getItem, StorageKeys } from '@/utils/storage'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'
import { useButtons } from '@/utils/buttons'

// 刷新后通知的 sessionStorage key
const REFRESH_NOTIFICATION_KEY = 'menu_refresh_notification'

// 解构导入 API 函数
const { getPagedMenuTree, addMenu: addMenuApi, updateMenu: updateMenuApi, deleteMenu: deleteMenuApi } = menuApi

// 按钮管理
const { hasBtn } = useButtons('settings-menu')

// 菜单数据类型（使用 API 定义的 MenuTree 类型）
type Menu = MenuTree

// 菜单表单类型
interface MenuForm {
  id?: string | number
  name: string
  path: string
  component?: string
  icon?: string
  parentId?: string | number
  sortOrder?: number
  status?: number
}

// 响应式数据
const menuList = ref<Menu[]>([])
const dialogVisible = ref(false)
const dialogTitle = ref('添加菜单')
const menuFormRef = ref()
const menuForm = ref<MenuForm>({
  name: '',
  path: '',
  component: '',
  icon: '',
  parentId: undefined,
  sortOrder: 0,
  status: 1
})
const menuTreeData = ref<Menu[]>([])
const pagination = ref({
  pageNum: 1,
  pageSize: 10,
  total: 0
})

// 所有 Element Plus 图标列表
const allIcons = Object.keys(Icons).filter(iconName => {
  // 过滤掉非图标组件
  return !iconName.startsWith('_') && !iconName.startsWith('install') && iconName !== 'default'
})

// 菜单表单验证规则
const menuRules = {
  name: [
    { required: true, message: '请输入菜单名称', trigger: 'blur' }
  ],
  path: [
    { required: true, message: '请输入路由路径', trigger: 'blur' }
  ]
}

// 加载菜单数据
const loadMenuData = async (forceRefresh: boolean = false) => {
  try {
    const params: PageParams = {
      pageNum: pagination.value.pageNum,
      pageSize: pagination.value.pageSize
    }

    // 生成缓存键（包含分页参数）
    const cacheKey = `${StorageKeys.List}_menu_${params.pageNum}_${params.pageSize}`

    // 如果不是强制刷新，优先从缓存获取
    if (!forceRefresh) {
      const cachedData = getItem<{ list: Menu[], total: number }>(cacheKey)
      if (cachedData) {
        menuList.value = cachedData.list || []
        pagination.value.total = cachedData.total || 0
        buildTreeData()
        return
      }
    }

    // 缓存不存在或强制刷新，从 API 获取
    const response = await getPagedMenuTree(params)
    if (response.data) {
      menuList.value = response.data.list || []
      pagination.value.total = response.data.total || 0

      // 存入缓存
      setItem(cacheKey, {
        list: menuList.value,
        total: pagination.value.total
      })
    }
    buildTreeData()
  } catch (error) {
    console.error('加载菜单数据失败:', error)
    showErrorNotification({ title: '错误', message: '加载菜单数据失败' })
  }
}

// 清空菜单列表缓存
const clearMenuCache = () => {
  // 清除所有以 'list_menu' 开头的缓存
  const keysToRemove: string[] = []
  for (let i = 0; i < localStorage.length; i++) {
    const key = localStorage.key(i)
    if (key && key.startsWith(`${StorageKeys.List}_menu`)) {
      keysToRemove.push(key)
    }
  }
  keysToRemove.forEach(key => localStorage.removeItem(key))
}

// 构建树形菜单数据
const buildTreeData = () => {
  menuTreeData.value = JSON.parse(JSON.stringify(menuList.value))
}

// 添加菜单
const addMenu = () => {
  dialogTitle.value = '添加菜单'
  resetMenuForm()
  dialogVisible.value = true
}

// 编辑菜单
const editMenu = (row: Menu) => {
  dialogTitle.value = '编辑菜单'
  menuForm.value = { ...row }
  dialogVisible.value = true
}

// 添加子菜单
const addChildMenu = (parentRow: Menu) => {
  dialogTitle.value = '添加子菜单'
  resetMenuForm()
  menuForm.value.parentId = parentRow.id
  dialogVisible.value = true
}

// 删除菜单
const deleteMenu = async (id: number) => {
  try {
    await ElMessageBox.confirm('确认删除此菜单吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    // 调用API删除菜单
    await deleteMenuApi(id.toString())
    clearMenuCache() // 清空菜单缓存

    // 存储通知信息，刷新后显示
    sessionStorage.setItem(REFRESH_NOTIFICATION_KEY, JSON.stringify({
      title: '成功',
      message: '删除成功'
    }))

    await refreshSidebarMenu() // 刷新侧边栏菜单
  } catch (error) {
    console.log('取消删除或删除失败')
  }
}

// 重置菜单表单
const resetMenuForm = () => {
  menuForm.value = {
    name: '',
    path: '',
    component: '',
    icon: '',
    parentId: undefined,
    sortOrder: 0,
    status: 1
  }
}

// 提交菜单表单
const submitMenuForm = async () => {
  try {
    await menuFormRef.value.validate()

    let notificationMessage = ''

    if (dialogTitle.value === '添加菜单' || dialogTitle.value === '添加子菜单') {
      // 调用API添加菜单
      await addMenuApi(menuForm.value)
      notificationMessage = '添加成功'
    } else {
      // 调用API更新菜单
      if (menuForm.value.id) {
        await updateMenuApi(menuForm.value.id.toString(), menuForm.value)
        notificationMessage = '编辑成功'
      }
    }

    dialogVisible.value = false
    clearMenuCache() // 清空菜单缓存

    // 存储通知信息，刷新后显示
    sessionStorage.setItem(REFRESH_NOTIFICATION_KEY, JSON.stringify({
      title: '成功',
      message: notificationMessage
    }))

    await refreshSidebarMenu() // 刷新侧边栏菜单
  } catch (error) {
    console.log('验证失败或保存失败')
  }
}

// 刷新侧边栏菜单
const refreshSidebarMenu = async () => {
  try {
    removeItem(StorageKeys.SidebarMenu)
    window.location.reload()
  } catch (error) {
    console.error('刷新侧边栏菜单失败:', error)
  }
}

// 处理分页大小变化
const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size
  loadMenuData()
}

// 处理分页页码变化
const handleCurrentChange = (page: number) => {
  pagination.value.pageNum = page
  loadMenuData()
}

// 检查并显示刷新后的通知
const checkAndShowRefreshNotification = () => {
  const notificationData = sessionStorage.getItem(REFRESH_NOTIFICATION_KEY)
  if (notificationData) {
    try {
      const { title, message } = JSON.parse(notificationData)
      showSuccessNotification({ title, message })
      sessionStorage.removeItem(REFRESH_NOTIFICATION_KEY)
    } catch (e) {
      console.error('解析通知数据失败:', e)
    }
  }
}

onMounted(() => {
  loadMenuData()
  checkAndShowRefreshNotification()
})
</script>

<style scoped>
.menu-container {
  padding: 0;
  height: 100%;
}

.menu-content {
  margin: 0;
  height: 100%;
}

.menu-card {
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
  padding: 0 8px;
}

.dialog-footer {
  text-align: right;
}

/* 图标单元格样式 */
.icon-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

.icon-name {
  font-size: 12px;
  color: #666;
}

.no-icon {
  color: #999;
  font-style: italic;
}

/* 图标选择器样式 */
.icon-selector {
  position: relative;
  width: 100%;
}

.icon-selector :deep(.el-input__wrapper) {
  overflow: visible !important;
}

.icon-selector :deep(.icon-trigger) {
  cursor: pointer;
  transition: transform 0.3s;
}

.icon-selector :deep(.icon-trigger:hover) {
  transform: scale(1.1);
}

/* el-select 图标选项样式 */
.icon-option {
  display: flex;
  align-items: center;
  gap: 10px;
}

.icon-name {
  font-size: 14px;
  color: #606266;
}

/* el-select 图标选项样式 */
:deep(.el-select .el-select__prefix) {
  display: flex;
  align-items: center;
}

:deep(.el-select .el-select__prefix .el-icon) {
  display: flex;
}

/* el-select 下拉菜单样式 */
:deep(.el-select-dropdown__option) {
  padding: 10px 20px;
}

:deep(.el-select-dropdown__option:hover) {
  background-color: #f5f7fa;
}

:deep(.el-select-dropdown__option.selected) {
  background-color: #f5f7fa;
  font-weight: bold;
}
</style>
