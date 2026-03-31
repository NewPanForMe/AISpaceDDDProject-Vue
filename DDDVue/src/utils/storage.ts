/**
 * localStorage 分类管理工具
 * 
 * 分类说明：
 * - Auth: 认证相关 (token)
 * - User: 用户信息 (userInfo, permissions)
 * - Menu: 菜单相关 (sidebarMenu)
 * - List: 列表数据缓存 (list_*)
 * - Setting: 设置相关 (settings_*)
 */

// 定义 localStorage 键名常量
export const StorageKeys = {
  // 认证相关
  Token: 'token',

  // 用户信息
  UserInfo: 'userInfo',
  Permissions: 'permissions',

  // 菜单相关
  SidebarMenu: 'sidebarMenu',

  // 列表数据缓存
  List: 'list',

  // 设置相关
  Setting: 'settings',
} as const

// 用户信息类型（与后端 UserDto 对应）
export interface UserDto {
  id?: string
  userName?: string
  email?: string
  phoneNumber?: string
  realName?: string
  avatar?: string
  status?: number
  lastLoginTime?: string
  lastLoginIp?: string
  remark?: string
  createdAt?: string
  updatedAt?: string
}

// 定义分类类型（更细化的分类）
export type StorageCategory = 'Auth' | 'User' | 'Menu' | 'List' | 'Setting' | 'All'

// 分类配置信息
export const CategoryConfig = {
  Auth: {
    name: '登录认证',
    description: '用户登录令牌，清除后需要重新登录',
    icon: 'Key',
    color: '#E6A23C',
    keys: [StorageKeys.Token],
    permission: 'cache:clear_auth'
  },
  User: {
    name: '用户信息',
    description: '用户基本信息和权限数据',
    icon: 'User',
    color: '#409EFF',
    keys: [StorageKeys.UserInfo, StorageKeys.Permissions],
    permission: 'cache:clear_user'
  },
  Menu: {
    name: '菜单数据',
    description: '侧边栏菜单和导航数据',
    icon: 'Menu',
    color: '#67C23A',
    keys: [StorageKeys.SidebarMenu],
    permission: 'cache:clear_menu'
  },
  List: {
    name: '列表缓存',
    description: '各模块列表页的数据缓存',
    icon: 'DataLine',
    color: '#909399',
    keys: [], // 动态获取以 'list' 开头的键
    permission: 'cache:clear_list'
  },
  Setting: {
    name: '系统设置',
    description: '用户偏好设置和系统配置',
    icon: 'Setting',
    color: '#F56C6C',
    keys: [], // 动态获取以 'settings' 开头的键
    permission: 'cache:clear_setting'
  },
  All: {
    name: '全部缓存',
    description: '清除所有本地存储数据',
    icon: 'Delete',
    color: '#F56C6C',
    keys: [],
    permission: 'cache:clear_all'
  }
} as const

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
 * 获取指定分类的所有键名
 * @param category 分类
 * @returns 该分类的所有键名数组
 */
export const getKeysByCategory = (category: StorageCategory): string[] => {
  const keys: string[] = []
  
  switch (category) {
    case 'Auth':
      keys.push(StorageKeys.Token)
      break

    case 'User':
      keys.push(StorageKeys.UserInfo, StorageKeys.Permissions)
      break

    case 'Menu':
      keys.push(StorageKeys.SidebarMenu)
      break

    case 'List':
      // 列表缓存的键名（所有以 'list' 开头的键）
      for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i)
        if (key && key.startsWith(StorageKeys.List)) {
          keys.push(key)
        }
      }
      break

    case 'Setting':
      // 设置相关的键名（所有以 'settings' 开头的键）
      for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i)
        if (key && key.startsWith(StorageKeys.Setting)) {
          keys.push(key)
        }
      }
      break

    case 'All':
      // 返回所有键名
      for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i)
        if (key) keys.push(key)
      }
      break
  }

  return keys
}

/**
 * 清除指定分类的存储数据
 * @param category 要清除的分类
 */
export const clearByCategory = (category: StorageCategory): void => {
  try {
    const keys = getKeysByCategory(category)
    keys.forEach(key => localStorage.removeItem(key))
    console.log(`已清除 ${category} 分类的缓存，共 ${keys.length} 项`)
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
  const stats: Record<string, number> = {
    Auth: 0,
    User: 0,
    Menu: 0,
    List: 0,
    Setting: 0
  }

  for (let i = 0; i < localStorage.length; i++) {
    const key = localStorage.key(i) || ''
    
    if (key === StorageKeys.Token) {
      stats.Auth++
    } else if (key === StorageKeys.UserInfo || key === StorageKeys.Permissions) {
      stats.User++
    } else if (key === StorageKeys.SidebarMenu) {
      stats.Menu++
    } else if (key.startsWith(StorageKeys.List)) {
      stats.List++
    } else if (key.startsWith(StorageKeys.Setting)) {
      stats.Setting++
    }
  }

  return stats
}

/**
 * 获取总缓存数量
 * @returns 缓存总项数
 */
export const getTotalCacheCount = (): number => {
  return localStorage.length
}

/**
 * 获取各分类详细信息
 * @returns 各分类的详细信息数组
 */
export const getCategoryDetails = () => {
  const stats = getStorageStats()
  return Object.entries(CategoryConfig)
    .filter(([key]) => key !== 'All')
    .map(([key, config]) => ({
      key: key as StorageCategory,
      name: config.name,
      description: config.description,
      icon: config.icon,
      color: config.color,
      count: stats[key] || 0,
      permission: config.permission
    }))
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
  ROLE_ASSIGN_BUTTON: 'role:assign_button',
  ROLE_ASSIGN_PERMISSION: 'role:assign_permission',
  ROLE_ENABLE: 'role:enable',
  ROLE_DISABLE: 'role:disable',

  // 系统设置
  SETTING_SAVE_JWT: 'setting:save_jwt',
  SETTING_SAVE_SYSTEM: 'setting:save_system',

  // 权限管理
  PERMISSION_ADD: 'permission:add',
  PERMISSION_EDIT: 'permission:edit',
  PERMISSION_DELETE: 'permission:delete',
  PERMISSION_ENABLE: 'permission:enable',
  PERMISSION_DISABLE: 'permission:disable',

  // 缓存管理
  CACHE_CLEAR_AUTH: 'cache:clear_auth',
  CACHE_CLEAR_USER: 'cache:clear_user',
  CACHE_CLEAR_MENU: 'cache:clear_menu',
  CACHE_CLEAR_LIST: 'cache:clear_list',
  CACHE_CLEAR_SETTING: 'cache:clear_setting',
  CACHE_CLEAR_ALL: 'cache:clear_all',

  // 按钮管理
  BUTTON_ADD: 'button:add',
  BUTTON_EDIT: 'button:edit',
  BUTTON_DELETE: 'button:delete',
  BUTTON_ENABLE: 'button:enable',
  BUTTON_DISABLE: 'button:disable'
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