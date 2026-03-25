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