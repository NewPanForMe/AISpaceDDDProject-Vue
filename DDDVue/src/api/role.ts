import { http } from '@/utils/http'
import api from '@/api/index'
import type { PagedResult, RoleDto, CreateRoleRequest, UpdateRoleRequest } from '@/api/index'

// 分页参数接口
export interface PageParams {
  pageNum: number
  pageSize: number
  [key: string]: any
}

// 分配用户角色请求
export interface AssignUserRolesRequest {
  userId: string
  roleIds: string[]
}

// 获取角色列表（分页）
export const getRoles = (params?: PageParams) => {
  return http.get<PagedResult<RoleDto>>(api.Role.GetRolesAsync, { params })
}

// 根据ID获取角色详情
export const getRoleById = (id: string) => {
  return http.get<RoleDto>(api.Role.GetRoleByIdAsync, { params: { id } })
}

// 创建角色
export const createRole = (data: CreateRoleRequest) => {
  return http.post(api.Role.CreateRoleAsync, data)
}

// 更新角色
export const updateRole = (data: UpdateRoleRequest) => {
  return http.put(api.Role.UpdateRoleAsync, data)
}

// 删除角色
export const deleteRole = (id: string) => {
  return http.delete(api.Role.DeleteRoleAsync, { params: { id } })
}

// 启用角色
export const enableRole = (id: string) => {
  return http.post(api.Role.EnableRoleAsync, {}, { params: { id } })
}

// 禁用角色
export const disableRole = (id: string) => {
  return http.post(api.Role.DisableRoleAsync, {}, { params: { id } })
}

// 获取用户的角色ID列表
export const getUserRoleIds = (userId: string) => {
  return http.get<string[]>(api.Role.GetUserRoleIdsAsync, { params: { userId } })
}

// 获取用户的角色详情列表
export const getUserRoles = (userId: string) => {
  return http.get<RoleDto[]>(api.Role.GetUserRolesAsync, { params: { userId } })
}

// 配置用户角色
export const assignUserRoles = (data: AssignUserRolesRequest) => {
  return http.post(api.Role.AssignUserRolesAsync, data)
}

// 获取所有启用的角色列表
export const getEnabledRoles = () => {
  return http.get<RoleDto[]>(api.Role.GetEnabledRolesAsync)
}

// 分配角色用户请求
export interface AssignRoleUsersRequest {
  roleId: string
  userIds: string[]
}

// 获取角色的用户ID列表
export const getRoleUserIds = (roleId: string) => {
  return http.get<string[]>(api.Role.GetRoleUserIdsAsync, { params: { roleId } })
}

// 为角色分配用户
export const assignRoleUsers = (data: AssignRoleUsersRequest) => {
  return http.post(api.Role.AssignRoleUsersAsync, data)
}

// ==================== 系统设置模块 ====================
export interface SettingDto {
  id: string
  key: string
  value: string
  description?: string
  group: string
  createdAt: string
  updatedAt?: string
}

export interface UpdateSettingRequest {
  key: string
  value: string
}

export interface BatchUpdateSettingsRequest {
  settings: UpdateSettingRequest[]
}

// 获取所有设置
export const getAllSettings = () => {
  return http.get<SettingDto[]>(api.Setting.GetAllSettingsAsync)
}

// 根据分组获取设置
export const getSettingsByGroup = (group: string) => {
  return http.get<SettingDto[]>(api.Setting.GetSettingsByGroupAsync, { params: { group } })
}

// 根据键获取设置值
export const getSettingByKey = (key: string) => {
  return http.get<SettingDto>(api.Setting.GetSettingByKeyAsync, { params: { key } })
}

// 更新单个设置
export const updateSetting = (data: UpdateSettingRequest) => {
  return http.post(api.Setting.UpdateSettingAsync, data)
}

// 批量更新设置
export const batchUpdateSettings = (data: BatchUpdateSettingsRequest) => {
  return http.post(api.Setting.BatchUpdateSettingsAsync, data)
}

// ==================== 权限模块 ====================
export interface PermissionDto {
  id: string
  code: string
  name: string
  description?: string
  module: string
  menuId?: string
  sortOrder: number
  status: number
  createdAt: string
  updatedAt?: string
}

export interface CreatePermissionRequest {
  code: string
  name: string
  description?: string
  module: string
  menuId?: string
  sortOrder: number
}

export interface UpdatePermissionRequest {
  id: string
  name?: string
  description?: string
  sortOrder?: number
}

export interface AssignRolePermissionsRequest {
  roleId: string
  permissionIds: string[]
}

// 获取权限列表（分页）
export const getPermissions = (params?: PageParams) => {
  return http.get<PagedResult<PermissionDto>>(api.Permission.GetPermissionsAsync, { params })
}

// 获取所有启用的权限列表
export const getAllEnabledPermissions = () => {
  return http.get<PermissionDto[]>(api.Permission.GetAllEnabledPermissionsAsync)
}

// 根据模块获取权限列表
export const getPermissionsByModule = (module: string) => {
  return http.get<PermissionDto[]>(api.Permission.GetPermissionsByModuleAsync, { params: { module } })
}

// 根据ID获取权限详情
export const getPermissionById = (id: string) => {
  return http.get<PermissionDto>(api.Permission.GetPermissionByIdAsync, { params: { id } })
}

// 创建权限
export const createPermission = (data: CreatePermissionRequest) => {
  return http.post(api.Permission.CreatePermissionAsync, data)
}

// 更新权限
export const updatePermission = (data: UpdatePermissionRequest) => {
  return http.put(api.Permission.UpdatePermissionAsync, data)
}

// 删除权限
export const deletePermission = (id: string) => {
  return http.delete(api.Permission.DeletePermissionAsync, { params: { id } })
}

// 启用权限
export const enablePermission = (id: string) => {
  return http.post(api.Permission.EnablePermissionAsync, {}, { params: { id } })
}

// 禁用权限
export const disablePermission = (id: string) => {
  return http.post(api.Permission.DisablePermissionAsync, {}, { params: { id } })
}

// 获取角色的权限ID列表
export const getRolePermissionIds = (roleId: string) => {
  return http.get<string[]>(api.Permission.GetRolePermissionIdsAsync, { params: { roleId } })
}

// 为角色分配权限
export const assignRolePermissions = (data: AssignRolePermissionsRequest) => {
  return http.post(api.Permission.AssignRolePermissionsAsync, data)
}

// 获取用户的权限列表
export const getUserPermissions = (userId: string) => {
  return http.get<PermissionDto[]>(api.Permission.GetUserPermissionsAsync, { params: { userId } })
}

// 获取用户的权限编码列表
export const getUserPermissionCodes = async (userId: string) => {
  const response = await http.get<PermissionDto[]>(api.Permission.GetUserPermissionsAsync, { params: { userId } })
  if (response.success && response.data) {
    return {
      ...response,
      data: response.data.map(p => p.code)
    }
  }
  return response
}

// 检查用户是否有指定权限
export const hasPermission = (userId: string, permissionCode: string) => {
  return http.get<boolean>(api.Permission.HasPermissionAsync, { params: { userId, permissionCode } })
}