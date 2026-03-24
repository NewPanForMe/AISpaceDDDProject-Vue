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

  // 菜单相关
  SidebarMenu: 'sidebarMenu',

  // 列表数据缓存
  // 其他列表缓存在此处添加
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
        localStorage.removeItem(StorageKeys.Token)
        localStorage.removeItem(StorageKeys.UserInfo)
        console.log('已清除 Login 分类的缓存')
        break

      case 'Menu':
        // 清除菜单相关数据
        localStorage.removeItem(StorageKeys.SidebarMenu)
        console.log('已清除 Menu 分类的缓存')
        break

      case 'List':
        // 清除列表数据缓存（预留扩展）
        console.log('已清除 List 分类的缓存')
        break

      case 'All':
        // 清除所有数据
        localStorage.clear()
        console.log('已清除所有 localStorage 缓存')
        break

      default:
        console.warn(`未知的分类: ${category}`)
    }
  } catch (e) {
    console.error(`清除分类缓存失败: ${category}`, e)
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
      // 列表缓存的键名可以在这里扩展
      return []

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
    } else {
      category = 'Other'
    }

    stats[category] = (stats[category] || 0) + 1
  }

  return stats
}
