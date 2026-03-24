// 菜单项接口
export interface MenuItem {
  id?: string | number
  path: string
  name: string
  icon?: string
  parentId?: string | number
  children?: MenuItem[]
}

import { getItem, setItem, StorageKeys } from './storage'

// 从 localStorage 获取菜单数据
export const getRoutesFromStorage = (): MenuItem[] => {
  return getItem<MenuItem[]>(StorageKeys.SidebarMenu) || []
}

// 将菜单数据保存到 localStorage
export const saveRoutesToStorage = (menus: MenuItem[]): void => {
  setItem(StorageKeys.SidebarMenu, menus)
}
