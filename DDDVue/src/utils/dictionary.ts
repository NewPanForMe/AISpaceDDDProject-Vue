import { ref, readonly } from 'vue'
import { getDictionariesByType } from '@/api/dictionary'
import type { DictionaryDto } from '@/api/index'
import { getItem, setItem, removeItem } from '@/utils/storage'

// 字典缓存键前缀
const DICT_CACHE_PREFIX = 'dict_'
// 字典缓存过期时间（毫秒）
const DICT_CACHE_EXPIRE = 30 * 60 * 1000 // 30分钟

// 字典项接口（简化版，用于下拉框等场景）
export interface DictItem {
  label: string  // 显示文本（对应字典的 name）
  value: string | number  // 实际值（对应字典的 value）
  code?: string  // 字典编码
  sortOrder?: number  // 排序
}

// 字典缓存结构
interface DictCache {
  data: DictItem[]
  expireTime: number
}

/**
 * 获取字典数据
 * @param type 字典类型
 * @returns 字典项列表
 */
export const getDictByType = async (type: string): Promise<DictItem[]> => {
  // 检查缓存
  const cacheKey = `${DICT_CACHE_PREFIX}${type}`
  const cached = getItem<DictCache>(cacheKey)
  
  if (cached && cached.expireTime > Date.now()) {
    return cached.data
  }

  // 从 API 获取
  try {
    const response = await getDictionariesByType(type)
    if (response && Array.isArray(response)) {
      // 过滤启用状态的字典，并转换为 DictItem 格式
      const dictItems: DictItem[] = response
        .filter((item: DictionaryDto) => item.status === 1)
        .sort((a: DictionaryDto, b: DictionaryDto) => a.sortOrder - b.sortOrder)
        .map((item: DictionaryDto) => ({
          label: item.name,
          value: item.value,
          code: item.code,
          sortOrder: item.sortOrder
        }))

      // 存入缓存
      setItem(cacheKey, {
        data: dictItems,
        expireTime: Date.now() + DICT_CACHE_EXPIRE
      })

      return dictItems
    }
    return []
  } catch (error) {
    console.error(`获取字典数据失败 [type: ${type}]:`, error)
    return []
  }
}

/**
 * 清除字典缓存
 * @param type 字典类型（可选，不传则清除所有字典缓存）
 */
export const clearDictCache = (type?: string) => {
  if (type) {
    removeItem(`${DICT_CACHE_PREFIX}${type}`)
  } else {
    // 清除所有字典缓存
    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i)
      if (key && key.startsWith(DICT_CACHE_PREFIX)) {
        localStorage.removeItem(key)
      }
    }
  }
}

/**
 * 字典 Hook - 用于组件中获取字典数据
 * @param type 字典类型
 * @returns 字典数据、加载状态、刷新方法
 */
export const useDictionary = (type: string) => {
  const dictData = ref<DictItem[]>([])
  const loading = ref(false)

  // 加载字典数据
  const loadDict = async () => {
    loading.value = true
    try {
      dictData.value = await getDictByType(type)
    } finally {
      loading.value = false
    }
  }

  // 刷新字典数据（清除缓存后重新加载）
  const refreshDict = async () => {
    clearDictCache(type)
    await loadDict()
  }

  // 根据值获取标签
  const getLabelByValue = (value: string | number): string => {
    const item = dictData.value.find(d => d.value === value || d.value === String(value))
    return item?.label || ''
  }

  // 根据标签获取值
  const getValueByLabel = (label: string): string | number | undefined => {
    const item = dictData.value.find(d => d.label === label)
    return item?.value
  }

  return {
    dictData: readonly(dictData),
    loading: readonly(loading),
    loadDict,
    refreshDict,
    getLabelByValue,
    getValueByLabel
  }
}

/**
 * 预定义的字典类型常量
 */
export const DICT_TYPES = {
  STATUS: 'status',           // 状态（启用/禁用）
  GENDER: 'gender',           // 性别
  USER_TYPE: 'user_type',     // 用户类型
  MENU_TYPE: 'menu_type',     // 菜单类型
  PERMISSION_TYPE: 'permission_type'  // 权限类型
}