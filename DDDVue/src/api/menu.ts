import { http } from '@/utils/http'
import api from '@/api/index'
import type { PagedResult } from '@/api/index'

// 分页参数接口
export interface PageParams {
  pageNum: number
  pageSize: number
  [key: string]: any
}

// 菜单树节点接口
export interface MenuTree {
  id?: string | number
  name: string
  path: string
  component?: string
  icon?: string
  parentId?: string | number
  sortOrder?: number
  status?: number
  children?: MenuTree[]
}

// 获取菜单树（所有菜单）- 返回树形结构，无需再次转换
export const getMenuTree = () => {
  return http.get<MenuTree[]>(api.Menu.GetSidebarMenusAsync)
}


// 获取分页菜单树（用于大数据量）
export const getPagedMenuTree = (params?: PageParams) => {
  return http.get<PagedResult<MenuTree[]>>(api.Menu.GetPagedTreeMenusAsync, { params })
}

// 获取扁平菜单列表（用于需要手动构建树形结构的场景）
export const getMenuListFlat = (params?: PageParams) => {
  return http.get<PagedResult<MenuTree[]>>(api.Menu.GetMenusAsync, { params })
}

// 获取菜单列表（分页）
export const getMenuList = (params?: PageParams) => {
  return http.get<PagedResult<any>>(api.Menu.GetMenusAsync, { params })
}

// 获取树形菜单（用于侧边栏，已构建好层级结构）
export const getSidebarMenuTree = () => {
  return http.get<MenuTree[]>(api.Menu.GetSidebarMenusAsync)
}

// 根据ID获取菜单详情
export const getMenuById = (id: string) => {
  return http.get(api.Menu.GetMenuByIdAsync, { params: { id } })
}

// 添加菜单
export const addMenu = (data: any) => {
  return http.post(api.Menu.CreateMenuAsync, data)
}

// 更新菜单
export const updateMenu = (id: string, data: any) => {
  return http.put(api.Menu.UpdateMenuAsync, { ...data, id })
}

// 删除菜单
export const deleteMenu = (id: string) => {
  return http.delete(api.Menu.DeleteMenuAsync, { params: { id } })
}

// 启用菜单
export const enableMenu = (id: string) => {
  return http.post(api.Menu.EnableMenuAsync, {}, { params: { id } })
}

// 禁用菜单
export const disableMenu = (id: string) => {
  return http.post(api.Menu.DisableMenuAsync, {}, { params: { id } })
}
