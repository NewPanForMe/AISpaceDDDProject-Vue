using DDDProject.Application.DTOs;
using DDDProject.Domain.Entities;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 操作日志服务接口
/// </summary>
public interface IOperationLogService : IApplicationService
{
    /// <summary>
    /// 获取操作日志列表（分页、支持筛选）
    /// </summary>
    Task<ApiRequestResult> GetOperationLogsAsync(OperationLogQueryRequest request);

    /// <summary>
    /// 获取操作日志详情
    /// </summary>
    Task<ApiRequestResult> GetOperationLogByIdAsync(Guid id);

    /// <summary>
    /// 创建操作日志
    /// </summary>
    Task<ApiRequestResult> CreateOperationLogAsync(OperationLog log);

    /// <summary>
    /// 删除操作日志
    /// </summary>
    Task<ApiRequestResult> DeleteOperationLogAsync(Guid id);

    /// <summary>
    /// 批量删除操作日志
    /// </summary>
    Task<ApiRequestResult> BatchDeleteOperationLogsAsync(List<Guid> ids);

    /// <summary>
    /// 清空指定时间范围的操作日志
    /// </summary>
    Task<ApiRequestResult> ClearOperationLogsAsync(DateTime? startTime, DateTime? endTime);

    /// <summary>
    /// 获取操作类型统计
    /// </summary>
    Task<ApiRequestResult> GetOperationTypeStatisticsAsync(DateTime? startTime, DateTime? endTime);

    /// <summary>
    /// 获取操作模块统计
    /// </summary>
    Task<ApiRequestResult> GetModuleStatisticsAsync(DateTime? startTime, DateTime? endTime);

    /// <summary>
    /// 导出操作日志为 Excel 文件
    /// </summary>
    Task<byte[]> ExportToExcelAsync(OperationLogQueryRequest request);
}