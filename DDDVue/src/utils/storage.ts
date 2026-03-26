/**
 * localStorage 分类管理工具
 * 
 * 分类说明：
 * - Login: 登录相关数据 (token, userInfo)
 * - Menu: 菜单相关数据 (sidebarMenu)
 * - List: 列表数据缓存 (其他列表缓存)
 */

// 定义 localStorage 键名常量
export const StorageKeys = {
  // 登录相关
  Token: 'token',
  UserInfo: 'userInfo',
  Permissions: 'permissions', // 用户权限列表

  // 菜单相关
  SidebarMenu: 'sidebarMenu',

  // 列表数据缓存
  List: 'list', // 列表数据缓存
} as const

// 定义分类类型
export type StorageCategory = 'Login' | 'Menu' | 'List' | 'All' | 'Other'

/**
 * 获取存储的数据
 * @param key localStorage 键名
 * @returns 存储的数据，如果不存在或解析失败返回 null
 */
export const getItem = <T = any>(key: string): T | null => {
  try {
    const stored = localStorage.getItem(key)
    if (stored) {
      return JSON.parse(stored)
    }
  } catch (e) {
    console.error(`获取 localStorage 数据失败: ${key}`, e)
  }
  return null
}

/**
 * 设置存储数据
 * @param key localStorage 键名
 * @param value 要存储的数据
 */
export const setItem = (key: string, value: any): void => {
  try {
    localStorage.setItem(key, JSON.stringify(value))
  } catch (e) {
    console.error(`设置 localStorage 数据失败: ${key}`, e)
  }
}

/**
 * 移除指定键的存储数据
 * @param key localStorage 键名
 */
export const removeItem = (key: string): void => {
  try {
    localStorage.removeItem(key)
  } catch (e) {
    console.error(`移除 localStorage 数据失败: ${key}`, e)
  }
}

/**
 * 清除指定分类的存储数据
 * @param category 要清除的分类
 */
export const clearByCategory = (category: StorageCategory): void => {
  try {
    switch (category) {
      case 'Login':
        // 清除登录相关数据
        clearAllCache();
        console.log('已清除 Login 分类的缓存')
        break

      case 'Menu':
        // 清除菜单相关数据
        localStorage.removeItem(StorageKeys.SidebarMenu)
        console.log('已清除 Menu 分类的缓存')
        break

      case 'List':
        // 清除列表数据缓存（所有以 'list' 开头的键）
        const keysToRemove: string[] = []
        for (let i = 0; i < localStorage.length; i++) {
          const key = localStorage.key(i)
          if (key && key.startsWith(StorageKeys.List)) {
            keysToRemove.push(key)
          }
        }
        keysToRemove.forEach(key => localStorage.removeItem(key))
        console.log('已清除 List 分类的缓存')
        break

      case 'All':
        // 清除所有数据
        clearAllCache()
        break

      default:
        console.warn(`未知的分类: ${category}`)
    }
  } catch (e) {
    console.error(`清除分类缓存失败: ${category}`, e)
  }
}

/**
 * 清除所有缓存数据
 * 用于退出登录或需要完全重置应用状态时调用
 */
export const clearAllCache = (): void => {
  try {
    localStorage.clear()
    console.log('已清除所有 localStorage 缓存')
  } catch (e) {
    console.error('清除所有缓存失败', e)
  }
}

/**
 * 获取指定分类的所有键名
 * @param category 分类
 * @returns 该分类的所有键名数组
 */
export const getKeysByCategory = (category: StorageCategory): string[] => {
  switch (category) {
    case 'Login':
      return [StorageKeys.Token, StorageKeys.UserInfo]

    case 'Menu':
      return [StorageKeys.SidebarMenu]

    case 'List':
      // 列表缓存的键名（所有以 'list' 开头的键）
      const listKeys: string[] = []
      for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i)
        if (key && key.startsWith(StorageKeys.List)) {
          listKeys.push(key)
        }
      }
      return listKeys

    case 'All':
      // 返回所有键名
      const keys: string[] = []
      for (let i = 0; i < localStorage.length; i++) {
        keys.push(localStorage.key(i) || '')
      }
      return keys

    default:
      return []
  }
}

/**
 * 检查指定分类是否有数据
 * @param category 分类
 * @returns 是否有数据
 */
export const hasDataByCategory = (category: StorageCategory): boolean => {
  const keys = getKeysByCategory(category)
  return keys.some(key => localStorage.getItem(key) !== null)
}

/**
 * 获取所有分类的数据统计
 * @returns 包含各分类数据统计的对象
 */
export const getStorageStats = (): Record<string, number> => {
  const stats: Record<string, number> = {}

  for (let i = 0; i < localStorage.length; i++) {
    const key = localStorage.key(i) || ''
    let category = 'Unknown'

    // 根据键名判断分类
    if (key === StorageKeys.Token || key === StorageKeys.UserInfo) {
      category = 'Login'
    } else if (key === StorageKeys.SidebarMenu) {
      category = 'Menu'
    } else if (key.startsWith(StorageKeys.List)) {
      category = 'List'
    } else {
      category = 'Other'
    }

    stats[category] = (stats[category] || 0) + 1
  }

  return stats
}

// ==================== 权限管理相关函数 ====================

/**
 * 获取当前用户的权限列表
 * @returns 权限编码数组
 */
export const getUserPermissions = (): string[] => {
  const permissions = getItem<string[]>(StorageKeys.Permissions)
  return permissions || []
}

/**
 * 设置用户权限列表
 * @param permissions 权限编码数组
 */
export const setUserPermissions = (permissions: string[]): void => {
  setItem(StorageKeys.Permissions, permissions)
}

/**
 * 清除用户权限列表
 */
export const clearUserPermissions = (): void => {
  removeItem(StorageKeys.Permissions)
}

/**
 * 检查用户是否有指定权限
 * @param permissionCode 权限编码（如：user:add）
 * @returns 是否有该权限
 */
export const hasPermission = (permissionCode: string): boolean => {
  const permissions = getUserPermissions()
  return permissions.includes(permissionCode)
}

/**
 * 检查用户是否有任意一个指定权限
 * @param permissionCodes 权限编码数组
 * @returns 是否有任意一个权限
 */
export const hasAnyPermission = (permissionCodes: string[]): boolean => {
  const permissions = getUserPermissions()
  return permissionCodes.some(code => permissions.includes(code))
}

/**
 * 检查用户是否有所有指定权限
 * @param permissionCodes 权限编码数组
 * @returns 是否有所有权限
 */
export const hasAllPermissions = (permissionCodes: string[]): boolean => {
  const permissions = getUserPermissions()
  return permissionCodes.every(code => permissions.includes(code))
}

/**
 * 权限编码常量
 * 与后端 Permission 种子数据保持一致
 */
export const PermissionCodes = {
  // 菜单管理
  MENU_ADD: 'menu:add',
  MENU_EDIT: 'menu:edit',
  MENU_DELETE: 'menu:delete',
  MENU_ADD_CHILD: 'menu:add_child',

  // 用户管理
  USER_ADD: 'user:add',
  USER_EDIT: 'user:edit',
  USER_DELETE: 'user:delete',
  USER_RESET_PASSWORD: 'user:reset_password',
  USER_ASSIGN_ROLE: 'user:assign_role',
  USER_ENABLE: 'user:enable',
  USER_DISABLE: 'user:disable',

  // 角色管理
  ROLE_ADD: 'role:add',
  ROLE_EDIT: 'role:edit',
  ROLE_DELETE: 'role:delete',
  ROLE_ASSIGN_MENU: 'role:assign_menu',
  ROLE_ASSIGN_USER: 'role:assign_user',
  ROLE_ASSIGN_PERMISSION: 'role:assign_permission',
  ROLE_ENABLE: 'role:enable',
  ROLE_DISABLE: 'role:disable',

  // 系统设置
  SETTING_SAVE_JWT: 'setting:save_jwt',
  SETTING_SAVE_SYSTEM: 'setting:save_system',

  // 缓存管理
  CACHE_CLEAR_LOGIN: 'cache:clear_login',
  CACHE_CLEAR_MENU: 'cache:clear_menu',
  CACHE_CLEAR_LIST: 'cache:clear_list',
  CACHE_CLEAR_ALL: 'cache:clear_all'
} as const

/**
 * 创建权限检查函数
 * 用于 Vue 组件中快速检查权限
 * @returns 权限检查函数
 */
export const usePermission = () => {
  return {
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    getUserPermissions,
    setUserPermissions,
    clearUserPermissions,
    PermissionCodes
  }
}
