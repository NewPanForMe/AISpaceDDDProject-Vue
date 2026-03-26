import { http } from '@/utils/http'
import api from '@/api/index'
import type { MenuTree } from '@/api/menu'

// 为角色分配菜单请求
export interface AssignRoleMenusRequest {
  roleId: string
  menuIds: string[]
}

// 为菜单分配角色请求
export interface AssignMenuRolesRequest {
  menuId: string
  roleIds: string[]
}

// 获取角色的菜单ID列表
export const getRoleMenuIds = (roleId: string) => {
  return http.get<string[]>(api.MenuRole.GetRoleMenuIdsAsync, { params: { roleId } })
}

// 获取菜单的角色ID列表
export const getMenuRoleIds = (menuId: string) => {
  return http.get<string[]>(api.MenuRole.GetMenuRoleIdsAsync, { params: { menuId } })
}

// 为角色分配菜单
export const assignRoleMenus = (data: AssignRoleMenusRequest) => {
  return http.post(api.MenuRole.AssignRoleMenusAsync, data)
}

// 为菜单分配角色
export const assignMenuRoles = (data: AssignMenuRolesRequest) => {
  return http.post(api.MenuRole.AssignMenuRolesAsync, data)
}

// 获取角色的菜单列表（树形结构）
export const getRoleMenus = (roleId: string) => {
  return http.get<MenuTree[]>(api.MenuRole.GetRoleMenusAsync, { params: { roleId } })
}

// 获取用户的所有菜单（通过角色关联）
export const getUserMenusByRoles = (userId: string) => {
  return http.get<MenuTree[]>(api.MenuRole.GetUserMenusByRolesAsync, { params: { userId } })
}

// 清除角色的所有菜单权限
export const clearRoleMenus = (roleId: string) => {
  return http.delete(api.MenuRole.ClearRoleMenusAsync, { params: { roleId } })
}

// 清除菜单的所有角色关联
export const clearMenuRoles = (menuId: string) => {
  return http.delete(api.MenuRole.ClearMenuRolesAsync, { params: { menuId } })
}