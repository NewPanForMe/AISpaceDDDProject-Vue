import { http } from '@/utils/http'
import api from '@/api/index'
import type { PagedResult, DictionaryDto, DictionaryQueryRequest, CreateDictionaryRequest, UpdateDictionaryRequest } from '@/api/index'

// 获取字典列表（分页）
export const getDictionaries = (params?: DictionaryQueryRequest) => {
  return http.get<PagedResult<DictionaryDto>>(api.Dictionary.GetDictionariesAsync, { params })
}

// 根据ID获取字典详情
export const getDictionaryById = (id: string) => {
  return http.get<DictionaryDto>(api.Dictionary.GetDictionaryByIdAsync, { params: { id } })
}

// 根据编码获取字典
export const getDictionaryByCode = (code: string) => {
  return http.get<DictionaryDto>(api.Dictionary.GetDictionaryByCodeAsync, { params: { code } })
}

// 根据类型获取字典列表
export const getDictionariesByType = (type: string) => {
  return http.get<DictionaryDto[]>(api.Dictionary.GetDictionariesByTypeAsync, { params: { type } })
}

// 批量根据类型获取字典列表
export const getDictionariesByTypes = (types: string[]) => {
  return http.post<Record<string, DictionaryDto[]>>(api.Dictionary.GetDictionariesByTypesAsync, types)
}

// 创建字典
export const createDictionary = (data: CreateDictionaryRequest) => {
  return http.post(api.Dictionary.CreateDictionaryAsync, data)
}

// 更新字典
export const updateDictionary = (data: UpdateDictionaryRequest) => {
  return http.put(api.Dictionary.UpdateDictionaryAsync, data)
}

// 删除字典
export const deleteDictionary = (id: string) => {
  return http.delete(api.Dictionary.DeleteDictionaryAsync, { params: { id } })
}

// 启用字典
export const enableDictionary = (id: string) => {
  return http.post(api.Dictionary.EnableDictionaryAsync, {}, { params: { id } })
}

// 禁用字典
export const disableDictionary = (id: string) => {
  return http.post(api.Dictionary.DisableDictionaryAsync, {}, { params: { id } })
}