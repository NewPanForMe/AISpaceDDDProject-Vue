import { ref, readonly } from 'vue'
import { getDictionariesByType, getDictionariesByTypes } from '@/api/dictionary'
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
    // 后端返回 { success, message, data: DictionaryDto[] }
    const responseData = response.data || response
    if (responseData && Array.isArray(responseData)) {
      // 过滤启用状态的字典，并转换为 DictItem 格式
      const dictItems: DictItem[] = responseData
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
 * 批量获取字典数据
 * @param types 字典类型数组
 * @returns 按类型分组的字典项列表
 */
export const getDictByTypes = async (types: string[]): Promise<Record<string, DictItem[]>> => {
  if (!types || types.length === 0) {
    return {}
  }

  const result: Record<string, DictItem[]> = {}
  const typesToFetch: string[] = []

  // 先检查缓存
  for (const type of types) {
    const cacheKey = `${DICT_CACHE_PREFIX}${type}`
    const cached = getItem<DictCache>(cacheKey)

    if (cached && cached.expireTime > Date.now()) {
      result[type] = cached.data
    } else {
      typesToFetch.push(type)
    }
  }

  // 如果有未缓存的类型，从 API 批量获取
  if (typesToFetch.length > 0) {
    try {
      const response = await getDictionariesByTypes(typesToFetch)
      // 后端返回 { success, message, data: Record<string, DictionaryDto[]> }
      const responseData = response.data || response
      if (responseData && typeof responseData === 'object') {
        for (const type of typesToFetch) {
          const items = (responseData as any)[type] || []
          // 过滤启用状态的字典，并转换为 DictItem 格式
          const dictItems: DictItem[] = items
            .filter((item: DictionaryDto) => item.status === 1)
            .sort((a: DictionaryDto, b: DictionaryDto) => a.sortOrder - b.sortOrder)
            .map((item: DictionaryDto) => ({
              label: item.name,
              value: item.value,
              code: item.code,
              sortOrder: item.sortOrder
            }))

          // 存入缓存
          setItem(`${DICT_CACHE_PREFIX}${type}`, {
            data: dictItems,
            expireTime: Date.now() + DICT_CACHE_EXPIRE
          })

          result[type] = dictItems
        }
      }
    } catch (error) {
      console.error('批量获取字典数据失败:', error)
      // 对于获取失败的类型，设置空数组
      for (const type of typesToFetch) {
        result[type] = []
      }
    }
  }

  return result
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
 * 批量字典 Hook - 用于组件中批量获取多个字典数据
 * @param types 字典类型数组
 * @returns 字典数据映射、加载状态、加载方法、获取标签方法
 */
export const useDictionaries = (types: string[]) => {
  const dictDataMap = ref<Record<string, DictItem[]>>({})
  const loading = ref(false)

  // 加载所有字典数据
  const loadDicts = async () => {
    loading.value = true
    try {
      dictDataMap.value = await getDictByTypes(types)
    } finally {
      loading.value = false
    }
  }

  // 刷新指定类型的字典数据
  const refreshDict = async (type: string) => {
    clearDictCache(type)
    await loadDicts()
  }

  // 刷新所有字典数据
  const refreshAllDicts = async () => {
    for (const type of types) {
      clearDictCache(type)
    }
    await loadDicts()
  }

  // 根据类型和值获取标签
  const getLabelByValue = (type: string, value: string | number): string => {
    const items = dictDataMap.value[type] || []
    const item = items.find(d => d.value === value || d.value === String(value))
    return item?.label || ''
  }

  // 根据类型获取字典数据
  const getDictData = (type: string): DictItem[] => {
    return dictDataMap.value[type] || []
  }

  return {
    dictDataMap: readonly(dictDataMap),
    loading: readonly(loading),
    loadDicts,
    refreshDict,
    refreshAllDicts,
    getLabelByValue,
    getDictData
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
  PERMISSION_TYPE: 'permission_type',  // 权限类型
  // 操作日志相关
  LOG_OPERATION_TYPE: 'log_operation_type',  // 操作类型
  LOG_MODULE: 'log_module',                  // 操作模块
  LOG_STATUS: 'log_status',                  // 日志状态
  // 站内信相关
  MESSAGE_TYPE: 'message_type',              // 消息类型
  MESSAGE_PRIORITY: 'message_priority'       // 消息优先级
}