<template>
  <div class="role-container">
    <div class="role-content">
      <el-card class="role-card">
        <template #header>
          <div class="card-header">
            <el-button v-if="hasBtn('role:add')" class="button" type="primary" @click="addRole">添加角色</el-button>
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
          <el-table-column label="操作" width="480" fixed="right">
            <template #default="{ row }">
              <div class="table-actions">
                <el-button v-if="hasBtn('role:edit')" size="small" @click="editRole(row)">编辑</el-button>
                <el-button v-if="hasBtn('role:assign_menu')" size="small" type="primary" @click="assignMenus(row)">分配模块</el-button>
                <el-button v-if="hasBtn('role:assign_permission')" size="small" type="warning" @click="assignPermissions(row)">分配权限</el-button>
                <el-button v-if="hasBtn('role:assign_user')" size="small" type="success" @click="assignUsers(row)">分配用户</el-button>
              
                <el-button v-if="hasAnyBtn(['role:enable', 'role:disable'])" size="small" :type="row.status === 1 ? 'warning' : 'success'" @click="toggleRoleStatus(row)">
                  {{ row.status === 1 ? '禁用' : '启用' }}
                </el-button>
                <el-button v-if="hasBtn('role:delete')" size="small" type="danger" @click="deleteRole(row)">删除</el-button>
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

    <!-- 分配模块对话框 -->
    <el-dialog v-model="menuDialogVisible" title="分配模块" width="600px" :destroy-on-close="true">
      <el-form label-width="80px">
        <el-form-item label="角色名称">
          <el-input v-model="menuForm.roleName" disabled />
        </el-form-item>
        <el-form-item label="选择模块">
          <el-tree
            ref="menuTreeRef"
            :data="allMenus"
            :props="{ label: 'name', children: 'children' }"
            show-checkbox
            node-key="id"
            default-expand-all
            :check-strictly="false"
            v-loading="menuLoading"
            style="max-height: 400px; overflow-y: auto;"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="menuDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitMenuForm" :loading="menuLoading">确定</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 分配权限对话框 -->
    <el-dialog v-model="permissionDialogVisible" title="分配权限" width="800px" :destroy-on-close="true">
      <el-form label-width="80px">
        <el-form-item label="角色名称">
          <el-input v-model="permissionForm.roleName" disabled />
        </el-form-item>
        <el-form-item label="快捷操作">
          <el-button size="small" type="primary" @click="selectAllPermissions">全选</el-button>
          <el-button size="small" @click="deselectAllPermissions">取消全选</el-button>
        </el-form-item>
        <el-form-item label="选择权限">
          <div v-loading="permissionLoading" class="permission-container">
            <div v-for="group in menuGroups" :key="group.menuId" class="permission-group">
              <div class="module-header">
                <el-checkbox
                  :model-value="isMenuAllSelected(group.menuId)"
                  :indeterminate="isMenuIndeterminate(group.menuId)"
                  @change="(val: boolean) => toggleMenuButtons(group.menuId, val)"
                >
                  {{ group.menuName }}
                </el-checkbox>
              </div>
              <el-checkbox-group v-model="permissionForm.selectedButtonIds" class="permission-checkbox-group">
                <el-checkbox
                  v-for="button in getButtonsByMenu(group.menuId)"
                  :key="button.id"
                  :label="button.id"
                  :value="button.id"
                >
                  {{ button.name }}
                </el-checkbox>
              </el-checkbox-group>
            </div>
          </div>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="permissionDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitPermissionForm" :loading="permissionLoading">确定</el-button>
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
import * as menuRoleApi from '@/api/menuRole'
import * as menuApi from '@/api/menu'
import * as permissionApi from '@/api/role'
import { http } from '@/utils/http'
import api from '@/api/index'
import type { RoleDto, CreateRoleRequest, UpdateRoleRequest, UserDto, PermissionDto } from '@/api/index'
import type { MenuTree } from '@/api/menu'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'
import { getItem, setItem, StorageKeys } from '@/utils/storage'
import { useButtons } from '@/utils/buttons'

// 解构导入 API 函数
const { getRoles, createRole: createRoleApi, updateRole: updateRoleApi, deleteRole: deleteRoleApi, enableRole, disableRole, getRoleUserIds, assignRoleUsers } = roleApi
const { getUsers } = userApi
const { getRolePermissionIds, assignRolePermissions } = permissionApi

// 按钮管理
const { hasBtn, hasAnyBtn } = useButtons('users-role')

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

// 分配模块相关数据
const menuDialogVisible = ref(false)
const menuLoading = ref(false)
const menuTreeRef = ref()
const allMenus = ref<MenuTree[]>([])
const menuForm = ref<{
  roleId: string
  roleName: string
}>({
  roleId: '',
  roleName: ''
})

// 分配权限相关数据
const permissionDialogVisible = ref(false)
const permissionLoading = ref(false)
const allButtons = ref<ButtonDto[]>([])
const permissionForm = ref<{
  roleId: string
  roleName: string
  selectedButtonIds: string[]
}>({
  roleId: '',
  roleName: '',
  selectedButtonIds: []
})

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
}

// 菜单分组列表（动态从按钮数据获取）
const menuGroups = ref<{ menuId: string; menuName: string }[]>([])

// 根据菜单获取按钮列表
const getButtonsByMenu = (menuId: string) => {
  return allButtons.value.filter(b => b.menuId === menuId)
}

// 全选所有按钮
const selectAllPermissions = () => {
  permissionForm.value.selectedButtonIds = allButtons.value.map(b => b.id)
}

// 取消全选
const deselectAllPermissions = () => {
  permissionForm.value.selectedButtonIds = []
}

// 判断菜单是否全选
const isMenuAllSelected = (menuId: string) => {
  const menuButtons = getButtonsByMenu(menuId)
  if (menuButtons.length === 0) return false
  return menuButtons.every(b => permissionForm.value.selectedButtonIds.includes(b.id))
}

// 判断菜单是否部分选中（半选状态）
const isMenuIndeterminate = (menuId: string) => {
  const menuButtons = getButtonsByMenu(menuId)
  if (menuButtons.length === 0) return false
  const selectedCount = menuButtons.filter(b => permissionForm.value.selectedButtonIds.includes(b.id)).length
  return selectedCount > 0 && selectedCount < menuButtons.length
}

// 切换菜单按钮选中状态
const toggleMenuButtons = (menuId: string, checked: boolean) => {
  const menuButtonIds = getButtonsByMenu(menuId).map(b => b.id)
  if (checked) {
    // 添加该菜单所有按钮
    const newIds = new Set([...permissionForm.value.selectedButtonIds, ...menuButtonIds])
    permissionForm.value.selectedButtonIds = Array.from(newIds)
  } else {
    // 移除该菜单所有按钮
    permissionForm.value.selectedButtonIds = permissionForm.value.selectedButtonIds.filter(
      id => !menuButtonIds.includes(id)
    )
  }
}

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

    // 生成缓存键（包含分页参数）
    const cacheKey = `${StorageKeys.List}_role_${params.pageNum}_${params.pageSize}`

    // 优先从缓存获取
    const cachedData = getItem<{ list: RoleDto[], total: number }>(cacheKey)
    if (cachedData) {
      roleList.value = cachedData.list || []
      pagination.value.total = cachedData.total || 0
      return
    }

    // 缓存不存在，从 API 获取
    const response = await getRoles(params)
    if (response.data) {
      roleList.value = response.data.list || []
      pagination.value.total = response.data.total || 0

      // 存入缓存
      setItem(cacheKey, {
        list: response.data.list,
        total: response.data.total
      })
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
    // 先检查角色是否有关联的用户
    const userResponse = await getRoleUserIds(row.id)
    if (userResponse.data && userResponse.data.length > 0) {
      await ElMessageBox.alert(
        `该角色已分配给 ${userResponse.data.length} 个用户，请先解除用户关联后再删除。`,
        '无法删除',
        {
          confirmButtonText: '知道了',
          type: 'warning'
        }
      )
      return
    }

    // 检查角色是否有关联的菜单
    const menuResponse = await menuRoleApi.getRoleMenuIds(row.id)
    if (menuResponse.data && menuResponse.data.length > 0) {
      await ElMessageBox.alert(
        `该角色已分配 ${menuResponse.data.length} 个菜单权限，请先解除菜单关联后再删除。`,
        '无法删除',
        {
          confirmButtonText: '知道了',
          type: 'warning'
        }
      )
      return
    }

    // 检查角色是否有关联的权限
    const permissionResponse = await getRolePermissionIds(row.id)
    if (permissionResponse.data && permissionResponse.data.length > 0) {
      await ElMessageBox.alert(
        `该角色已分配 ${permissionResponse.data.length} 个权限，请先解除权限关联后再删除。`,
        '无法删除',
        {
          confirmButtonText: '知道了',
          type: 'warning'
        }
      )
      return
    }

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
    await loadRoleData()  // 刷新数据
  } catch (error) {
    console.error('分配用户失败:', error)
    showErrorNotification({ title: '错误', message: '分配用户失败' })
  } finally {
    userLoading.value = false
  }
}

// 加载所有菜单
const loadAllMenus = async () => {
  try {
    // 使用分页接口获取所有菜单（不过滤权限）
    const response = await menuApi.getPagedMenuTree({ pageNum: 1, pageSize: 1000 })
    if (response.data && response.data.list) {
      allMenus.value = response.data.list
    }
  } catch (error) {
    console.error('加载菜单列表失败:', error)
  }
}

// 分配模块
const assignMenus = async (row: RoleDto) => {
  menuForm.value = {
    roleId: row.id,
    roleName: row.name
  }

  menuLoading.value = true
  menuDialogVisible.value = true

  try {
    // 加载所有菜单
    await loadAllMenus()

    // 获取角色已有的菜单
    const response = await menuRoleApi.getRoleMenuIds(row.id)
    if (response.data && menuTreeRef.value) {
      // 设置已选中的菜单
      menuTreeRef.value.setCheckedKeys(response.data, false)
    }
  } catch (error) {
    console.error('加载角色菜单失败:', error)
    showErrorNotification({ title: '错误', message: '加载角色菜单失败' })
  } finally {
    menuLoading.value = false
  }
}

// 提交模块分配
const submitMenuForm = async () => {
  try {
    menuLoading.value = true

    // 获取选中的菜单ID（包括半选中的父节点）
    const checkedKeys = menuTreeRef.value.getCheckedKeys(false)

    await menuRoleApi.assignRoleMenus({
      roleId: menuForm.value.roleId,
      menuIds: checkedKeys
    })

    showSuccessNotification({ title: '成功', message: '分配模块成功' })
    menuDialogVisible.value = false
    await loadRoleData()  // 刷新数据
  } catch (error) {
    console.error('分配模块失败:', error)
    showErrorNotification({ title: '错误', message: '分配模块失败' })
  } finally {
    menuLoading.value = false
  }
}

// 加载所有按钮
const loadAllButtons = async () => {
  try {
    // 获取所有按钮（不分页）
    const response = await http.get<{ list: ButtonDto[]; total: number }>(
      api.Button.GetButtonsAsync,
      { params: { pageNum: 1, pageSize: 1000 } }
    )

    if (response.data) {
      allButtons.value = response.data.list || []

      // 动态提取菜单分组（去重）
      const menuMap = new Map<string, string>()
      allButtons.value.forEach(b => {
        if (b.menuName && !menuMap.has(b.menuId)) {
          menuMap.set(b.menuId, b.menuName)
        }
      })

      // 转换为数组并排序
      menuGroups.value = Array.from(menuMap.entries()).map(([menuId, menuName]) => ({
        menuId,
        menuName
      }))
    }
  } catch (error) {
    console.error('加载按钮列表失败:', error)
  }
}

// 分配权限
const assignPermissions = async (row: RoleDto) => {
  permissionForm.value = {
    roleId: row.id,
    roleName: row.name,
    selectedButtonIds: []
  }

  permissionLoading.value = true
  permissionDialogVisible.value = true

  try {
    // 加载所有按钮
    await loadAllButtons()

    // 获取角色已有的权限ID列表
    const rolePermissionResponse = await getRolePermissionIds(row.id)
    if (!rolePermissionResponse.data || rolePermissionResponse.data.length === 0) {
      // 没有权限，直接返回
      return
    }

    // 获取所有权限列表
    const allPermissionsResponse = await http.get<PermissionDto[]>(
      api.Permission.GetAllEnabledPermissionsAsync
    )

    if (!allPermissionsResponse.data) {
      return
    }

    // 根据权限ID找到对应的权限编码
    const rolePermissionIds = rolePermissionResponse.data
    const permissionCodes = allPermissionsResponse.data
      .filter((p: PermissionDto) => rolePermissionIds.includes(p.id))
      .map((p: PermissionDto) => p.code)

    // 根据权限编码匹配按钮
    const matchedButtonIds = allButtons.value
      .filter(b => b.permissionCode && permissionCodes.includes(b.permissionCode))
      .map(b => b.id)

    permissionForm.value.selectedButtonIds = matchedButtonIds
  } catch (error) {
    console.error('加载角色权限失败:', error)
    showErrorNotification({ title: '错误', message: '加载角色权限失败' })
  } finally {
    permissionLoading.value = false
  }
}

// 提交权限分配
const submitPermissionForm = async () => {
  try {
    permissionLoading.value = true

    // 获取选中按钮的权限编码
    const selectedPermissionCodes = allButtons.value
      .filter(b => permissionForm.value.selectedButtonIds.includes(b.id) && b.permissionCode)
      .map(b => b.permissionCode!)

    // 获取所有权限，找到对应的权限ID
    const allPermissionsResponse = await http.get<PermissionDto[]>(
      api.Permission.GetAllEnabledPermissionsAsync
    )

    if (!allPermissionsResponse.data) {
      showErrorNotification({ title: '错误', message: '获取权限列表失败' })
      return
    }

    // 根据权限编码匹配权限ID
    const permissionIds = allPermissionsResponse.data
      .filter((p: PermissionDto) => selectedPermissionCodes.includes(p.code))
      .map((p: PermissionDto) => p.id)

    await assignRolePermissions({
      roleId: permissionForm.value.roleId,
      permissionIds: permissionIds
    })

    showSuccessNotification({ title: '成功', message: '分配权限成功' })
    permissionDialogVisible.value = false
    await loadRoleData()  // 刷新数据
  } catch (error) {
    console.error('分配权限失败:', error)
    showErrorNotification({ title: '错误', message: '分配权限失败' })
  } finally {
    permissionLoading.value = false
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

/* 权限分配样式 */
.permission-container {
  max-height: 400px;
  overflow-y: auto;
}

.permission-group {
  margin-bottom: 16px;
  padding: 12px;
  background: #f5f7fa;
  border-radius: 4px;
}

.module-header {
  font-weight: 600;
  color: #303133;
  margin-bottom: 12px;
  padding-bottom: 8px;
  border-bottom: 1px solid #e4e7ed;
}

.module-header .el-checkbox {
  font-weight: 600;
}

.permission-checkbox-group {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  padding-left: 24px;
}

.permission-checkbox-group .el-checkbox {
  margin-right: 0;
}
</style>