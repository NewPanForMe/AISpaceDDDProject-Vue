/**
 * 按钮管理工具
 * 用于获取和管理页面按钮
 */

import { ref, onMounted } from 'vue'
import { http } from './http'
import api from '@/api/index'
import { getItem, setItem, StorageKeys, getUserPermissions } from './storage'

// 按钮数据类型
export interface ButtonDto {
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
  createdAt: string
  updatedAt?: string
}

// 按钮缓存键前缀
const BUTTON_CACHE_PREFIX = 'buttons_'

/**
 * 根据菜单路径获取按钮列表
 * @param menuPath 菜单路径（如 'users', 'settings-menu'）
 * @returns 按钮列表
 */
export const getButtonsByMenuPath = async (menuPath: string): Promise<ButtonDto[]> => {
  try {
    // 先尝试从缓存获取
    const cacheKey = `${BUTTON_CACHE_PREFIX}${menuPath}`
    const cachedButtons = getItem<ButtonDto[]>(cacheKey)
    if (cachedButtons && cachedButtons.length > 0) {
      return cachedButtons
    }

    // 从 API 获取所有按钮
    const response = await http.get<{ success: boolean; data?: { list: ButtonDto[]; total: number } }>(
      api.Button.GetButtonsAsync,
      { params: { pageNum: 1, pageSize: 1000 } }
    )

    if (response.success && response.data?.list) {
      // 缓存所有按钮（按菜单路径分组）
      const buttonsByMenu: Record<string, ButtonDto[]> = {}

      response.data.list.forEach(button => {
        // 根据按钮编码推断菜单路径
        const path = getButtonMenuPath(button)
        if (!buttonsByMenu[path]) {
          buttonsByMenu[path] = []
        }
        buttonsByMenu[path].push(button)
      })

      // 存入缓存
      Object.keys(buttonsByMenu).forEach(path => {
        setItem(`${BUTTON_CACHE_PREFIX}${path}`, buttonsByMenu[path])
      })

      return buttonsByMenu[menuPath] || []
    }

    return []
  } catch (error) {
    console.error('获取按钮列表失败:', error)
    return []
  }
}

/**
 * 根据按钮编码推断菜单路径
 */
const getButtonMenuPath = (button: ButtonDto): string => {
  const code = button.code

  // 根据编码前缀推断菜单路径
  if (code.startsWith('user:')) return 'users'
  if (code.startsWith('role:')) return 'users-role'
  if (code.startsWith('menu:')) return 'settings-menu'
  if (code.startsWith('permission:')) return 'settings-permissions'
  if (code.startsWith('setting:')) return 'settings-system'
  if (code.startsWith('cache:')) return 'settings-system'
  if (code.startsWith('button:')) return 'settings-button'
  if (code.startsWith('product:')) return 'products'

  return ''
}

/**
 * 检查按钮是否可见（基于用户权限）
 * @param buttons 按钮列表
 * @param buttonCode 按钮编码
 * @returns 是否可见
 */
export const hasButton = (buttons: ButtonDto[], buttonCode: string): boolean => {
  const button = buttons.find(b => b.code === buttonCode)
  
  // 按钮不存在或被禁用
  if (!button || button.status !== 1) {
    return false
  }

  // 如果按钮没有配置权限编码，默认显示
  if (!button.permissionCode) {
    return true
  }

  // 检查用户是否有该按钮对应的权限
  const userPermissions = getUserPermissions()
  return userPermissions.includes(button.permissionCode)
}

/**
 * 检查是否有任意一个按钮
 * @param buttons 按钮列表
 * @param buttonCodes 按钮编码数组
 * @returns 是否有任意一个按钮
 */
export const hasAnyButton = (buttons: ButtonDto[], buttonCodes: string[]): boolean => {
  return buttonCodes.some(code => hasButton(buttons, code))
}

/**
 * 获取按钮图标
 * @param buttons 按钮列表
 * @param buttonCode 按钮编码
 * @returns 图标名称
 */
export const getButtonIcon = (buttons: ButtonDto[], buttonCode: string): string | undefined => {
  const button = buttons.find(b => b.code === buttonCode)
  return button?.icon
}

/**
 * 组合式函数：使用按钮
 * @param menuPath 菜单路径
 * @returns 按钮列表和检查函数
 */
export const useButtons = (menuPath: string) => {
  const buttons = ref<ButtonDto[]>([])
  const loading = ref(false)

  // 加载按钮
  const loadButtons = async () => {
    loading.value = true
    try {
      buttons.value = await getButtonsByMenuPath(menuPath)
    } finally {
      loading.value = false
    }
  }

  // 检查按钮是否可见（基于用户权限）
  const hasBtn = (buttonCode: string): boolean => {
    return hasButton(buttons.value, buttonCode)
  }

  // 检查是否有任意一个按钮
  const hasAnyBtn = (buttonCodes: string[]): boolean => {
    return hasAnyButton(buttons.value, buttonCodes)
  }

  // 组件挂载时加载按钮
  onMounted(() => {
    loadButtons()
  })

  return {
    buttons,
    loading,
    hasBtn,
    hasAnyBtn,
    loadButtons
  }
}

/**
 * 清除按钮缓存
 */
export const clearButtonCache = () => {
  // 清除所有以 buttons_ 开头的缓存
  const keys = Object.keys(localStorage)
  keys.forEach(key => {
    if (key.startsWith(BUTTON_CACHE_PREFIX)) {
      localStorage.removeItem(key)
    }
  })
}