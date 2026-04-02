/**
 * 按钮权限管理工具
 * 基于用户角色权限编码列表控制按钮显示
 */

import { ref, onMounted } from 'vue'
import { getUserPermissions } from './storage'

// 按钮编码到权限编码的映射配置
const BUTTON_PERMISSION_MAP: Record<string, string> = {
  // 用户管理
  'user:add': 'user:add',
  'user:edit': 'user:edit',
  'user:delete': 'user:delete',
  'user:reset_password': 'user:reset_password',
  'user:assign_role': 'user:assign_role',
  'user:enable': 'user:enable',
  'user:disable': 'user:disable',

  // 角色管理
  'role:add': 'role:add',
  'role:edit': 'role:edit',
  'role:delete': 'role:delete',
  'role:assign_menu': 'role:assign_menu',
  'role:assign_permission': 'role:assign_permission',
  'role:assign_user': 'role:assign_user',
  'role:enable': 'role:enable',
  'role:disable': 'role:disable',

  // 菜单管理
  'menu:add': 'menu:add',
  'menu:edit': 'menu:edit',
  'menu:delete': 'menu:delete',
  'menu:add_child': 'menu:add_child',
  'menu:enable': 'menu:enable',
  'menu:disable': 'menu:disable',

  // 权限管理
  'permission:add': 'permission:add',
  'permission:edit': 'permission:edit',
  'permission:delete': 'permission:delete',
  'permission:enable': 'permission:enable',
  'permission:disable': 'permission:disable',

  // 系统设置
  'setting:save_jwt': 'setting:save_jwt',
  'setting:save_system': 'setting:save_system',

  // 缓存管理
  'cache:clear_auth': 'cache:clear_auth',
  'cache:clear_user': 'cache:clear_user',
  'cache:clear_menu': 'cache:clear_menu',
  'cache:clear_list': 'cache:clear_list',
  'cache:clear_setting': 'cache:clear_setting',
  'cache:clear_all': 'cache:clear_all',

  // 日志管理
  'log:delete': 'log:delete',
  'log:clear': 'log:clear',
  'log:export': 'log:export',

  // 字典管理
  'dictionary:add': 'dictionary:add',
  'dictionary:edit': 'dictionary:edit',
  'dictionary:delete': 'dictionary:delete',
  'dictionary:enable': 'dictionary:enable',
  'dictionary:disable': 'dictionary:disable',
}

/**
 * 检查按钮是否可见（基于用户权限）
 */
export const hasButton = (buttonCode: string): boolean => {
  const permissionCode = BUTTON_PERMISSION_MAP[buttonCode]

  // 如果没有配置权限映射，默认显示按钮
  if (!permissionCode) {
    console.warn(`[按钮权限] 未配置映射: ${buttonCode}`)
    return true
  }

  // 获取用户权限列表
  const userPermissions = getUserPermissions()
  
  // 检查用户是否有该权限
  return userPermissions.includes(permissionCode)
}

/**
 * 检查是否有任意一个按钮权限
 */
export const hasAnyButton = (buttonCodes: string[]): boolean => {
  const userPermissions = getUserPermissions()

  for (const buttonCode of buttonCodes) {
    const permissionCode = BUTTON_PERMISSION_MAP[buttonCode]
    if (permissionCode && userPermissions.includes(permissionCode)) {
      return true
    }
  }

  return false
}

/**
 * 组合式函数：使用按钮权限
 */
export const useButtons = (_menuPath?: string) => {
  const loading = ref(false)

  const hasBtn = (buttonCode: string): boolean => {
    return hasButton(buttonCode)
  }

  const hasAnyBtn = (buttonCodes: string[]): boolean => {
    return hasAnyButton(buttonCodes)
  }

  onMounted(() => {
    // 权限数据已在登录时加载到 localStorage
  })

  return {
    loading,
    hasBtn,
    hasAnyBtn,
  }
}

/**
 * 清除按钮缓存
 */
export const clearButtonCache = () => {
  console.log('[按钮权限] 缓存已清除')
}
