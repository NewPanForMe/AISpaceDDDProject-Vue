import { http } from '@/utils/http'
import api from '@/api/index'
import type { PagedResult, OperationLogDto, OperationLogQueryRequest, ClearLogsRequest } from '@/api/index'

// 获取操作日志列表（分页）
export const getOperationLogs = (params?: OperationLogQueryRequest) => {
  return http.get<PagedResult<OperationLogDto>>(api.OperationLog.GetOperationLogsAsync, { params })
}

// 根据ID获取操作日志详情
export const getOperationLogById = (id: string) => {
  return http.get<OperationLogDto>(api.OperationLog.GetOperationLogByIdAsync, { params: { id } })
}

// 删除操作日志
export const deleteOperationLog = (id: string) => {
  return http.delete(api.OperationLog.DeleteOperationLogAsync, { params: { id } })
}

// 批量删除操作日志
export const batchDeleteOperationLogs = (ids: string[]) => {
  return http.delete(api.OperationLog.BatchDeleteOperationLogsAsync, { data: { ids } })
}

// 清空指定时间范围的操作日志
export const clearOperationLogs = (data: ClearLogsRequest) => {
  return http.delete(api.OperationLog.ClearOperationLogsAsync, { data })
}

// 获取操作类型统计
export const getOperationTypeStatistics = (params?: { startTime?: string; endTime?: string }) => {
  return http.get<Record<string, number>>(api.OperationLog.GetOperationTypeStatisticsAsync, { params })
}

// 获取操作模块统计
export const getModuleStatistics = (params?: { startTime?: string; endTime?: string }) => {
  return http.get<Record<string, number>>(api.OperationLog.GetModuleStatisticsAsync, { params })
}

// 导出操作日志
export const exportOperationLogs = (params?: OperationLogQueryRequest): Promise<Blob> => {
  return http.get<Blob>(api.OperationLog.ExportOperationLogsAsync, { params, responseType: 'blob' }) as unknown as Promise<Blob>
}