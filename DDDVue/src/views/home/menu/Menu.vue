<template>
  <div class="menu-container">
    <h2>菜单管理</h2>
    <div class="menu-content">
      <el-card class="menu-card">
        <template #header>
          <div class="card-header">
            <span>菜单列表</span>
            <el-button class="button" type="primary" @click="addMenu">添加菜单</el-button>
          </div>
        </template>
        <el-table
          :data="menuList"
          style="width: 100%"
          row-key="id"
          :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
        >
          <el-table-column prop="name" label="菜单名称" width="180" />
          <el-table-column prop="path" label="路由路径" width="180" />
          <el-table-column prop="component" label="组件路径" width="200" />
          <el-table-column prop="icon" label="图标" width="120">
            <template #default="{ row }">
              <el-icon v-if="row.icon">
                <component :is="row.icon" />
              </el-icon>
              <span v-else>{{ row.icon }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="sortOrder" label="排序" width="80" />
          <el-table-column prop="status" label="状态" width="100">
            <template #default="{ row }">
              <el-tag :type="row.status === 1 ? 'success' : 'danger'">
                {{ row.status === 1 ? '启用' : '禁用' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="220">
            <template #default="{ row }">
              <el-button size="small" @click="editMenu(row)">编辑</el-button>
              <el-button size="small" type="danger" @click="deleteMenu(row.id)">删除</el-button>
              <el-button size="small" type="primary" @click="addChildMenu(row)">添加子菜单</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-card>
    </div>

    <!-- 菜单编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="600px">
      <el-form :model="menuForm" :rules="menuRules" ref="menuFormRef" label-width="100px">
        <el-form-item label="菜单名称" prop="name">
          <el-input v-model="menuForm.name" placeholder="请输入菜单名称" />
        </el-form-item>
        <el-form-item label="上级菜单" prop="parentId">
          <el-tree-select
            v-model="menuForm.parentId"
            :data="menuTreeData"
            :props="{ label: 'name', value: 'id', children: 'children' }"
            :check-strictly="true"
            placeholder="请选择上级菜单"
            clearable
          />
        </el-form-item>
        <el-form-item label="路由路径" prop="path">
          <el-input v-model="menuForm.path" placeholder="请输入路由路径" />
        </el-form-item>
        <el-form-item label="组件路径" prop="component">
          <el-input v-model="menuForm.component" placeholder="请输入组件路径" />
        </el-form-item>
        <el-form-item label="图标">
          <el-input v-model="menuForm.icon" placeholder="请输入图标名称" />
        </el-form-item>
        <el-form-item label="排序">
          <el-input-number v-model="menuForm.sortOrder" :min="0" />
        </el-form-item>
        <el-form-item label="状态">
          <el-switch
            v-model="menuForm.status"
            :active-value="1"
            :inactive-value="0"
            inline-prompt
            active-text="启用"
            inactive-text="禁用"
          />
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
import { ElMessage, ElMessageBox } from 'element-plus'
import menuApi from '@/api/menu'

// 菜单数据类型
interface Menu {
  id: number
  name: string
  path: string
  component: string
  icon: string
  parentId: number | null
  sortOrder: number
  status: number
  children?: Menu[]
}

// 菜单表单类型
interface MenuForm {
  id?: number
  name: string
  path: string
  component: string
  icon: string
  parentId: number | null
  sortOrder: number
  status: number
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
  parentId: null,
  sortOrder: 0,
  status: 1
})
const menuTreeData = ref<Menu[]>([])

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
const loadMenuData = async () => {
  try {
    const response = await menuApi.getMenuTree()
    menuList.value = response.data || []
    buildTreeData()
  } catch (error) {
    console.error('加载菜单数据失败:', error)
    // 使用模拟数据作为备选
    menuList.value = [
      {
        id: 1,
        name: '首页',
        path: '/home/dashboard',
        component: 'Dashboard/Dashboard.vue',
        icon: 'Monitor',
        parentId: null,
        sortOrder: 1,
        status: 1
      },
      {
        id: 2,
        name: '产品管理',
        path: '/home/products',
        component: 'Products/Products.vue',
        icon: 'ShoppingBag',
        parentId: null,
        sortOrder: 2,
        status: 1,
        children: [
          {
            id: 21,
            name: '产品列表',
            path: '/home/products/list',
            component: 'Products/Products.vue',
            icon: 'List',
            parentId: 2,
            sortOrder: 1,
            status: 1
          },
          {
            id: 22,
            name: '产品分类',
            path: '/home/products/category',
            component: 'Products/Category.vue',
            icon: 'FolderOpened',
            parentId: 2,
            sortOrder: 2,
            status: 1
          }
        ]
      },
      {
        id: 3,
        name: '用户管理',
        path: '/home/users',
        component: 'Users/Users.vue',
        icon: 'User',
        parentId: null,
        sortOrder: 3,
        status: 1
      },
      {
        id: 4,
        name: '系统设置',
        path: '/home/settings',
        component: 'Settings/Settings.vue',
        icon: 'Setting',
        parentId: null,
        sortOrder: 4,
        status: 1
      }
    ]
    buildTreeData()
  }
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
    await menuApi.deleteMenu(id)
    await loadMenuData() // 重新加载数据
    ElMessage.success('删除成功')
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
    parentId: null,
    sortOrder: 0,
    status: 1
  }
}

// 提交菜单表单
const submitMenuForm = async () => {
  try {
    await menuFormRef.value.validate()
    
    if (dialogTitle.value === '添加菜单' || dialogTitle.value === '添加子菜单') {
      // 调用API添加菜单
      await menuApi.addMenu(menuForm.value)
      ElMessage.success('添加成功')
    } else {
      // 调用API更新菜单
      if (menuForm.value.id) {
        await menuApi.updateMenu(menuForm.value.id, menuForm.value)
        ElMessage.success('编辑成功')
      }
    }
    
    dialogVisible.value = false
    await loadMenuData() // 重新加载数据
  } catch (error) {
    console.log('验证失败或保存失败')
  }
}

onMounted(() => {
  loadMenuData()
})
</script>

<style scoped>
.menu-container {
  padding: 20px;
}

.menu-content {
  margin-top: 20px;
}

.menu-card {
  min-height: 400px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.text {
  font-size: 14px;
}

.item {
  margin-bottom: 18px;
}

.dialog-footer {
  text-align: right;
}
</style>