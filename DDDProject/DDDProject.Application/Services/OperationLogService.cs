using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Repositories;
using System.Linq.Expressions;

namespace DDDProject.Application.Services;

/// <summary>
/// 操作日志服务实现
/// </summary>
public class OperationLogService : IOperationLogService
{
    private readonly IRepository<OperationLog> _repository;

    public OperationLogService(IRepository<OperationLog> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 获取操作日志列表（分页、支持筛选）
    /// </summary>
    public async Task<ApiRequestResult> GetOperationLogsAsync(OperationLogQueryRequest request)
    {
        var skipCount = (request.PageNumber - 1) * request.PageSize;

        // 构建查询条件
        var predicate = BuildQueryPredicate(request);

        // 获取总数
        var total = await _repository.CountAsync(predicate);

        // 获取列表
        var logs = await _repository.GetListAsync(
            predicate,
            q => q.OrderByDescending(l => l.CreatedAt),
            skipCount,
            request.PageSize
        );

        // 转换为 DTO
        var logDtos = logs.Select(l => new OperationLogDto
        {
            Id = l.Id,
            UserId = l.UserId,
            UserName = l.UserName,
            RealName = l.RealName,
            OperationType = l.OperationType,
            Module = l.Module,
            Description = l.Description,
            RequestMethod = l.RequestMethod,
            RequestPath = l.RequestPath,
            RequestParams = l.RequestParams,
            ResponseResult = l.ResponseResult,
            IpAddress = l.IpAddress,
            Status = l.Status,
            ErrorMessage = l.ErrorMessage,
            Duration = l.Duration,
            Browser = l.Browser,
            OsInfo = l.OsInfo,
            CreatedAt = l.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<OperationLogDto>
        {
            List = logDtos,
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = pagedResult };
    }

    /// <summary>
    /// 获取操作日志详情
    /// </summary>
    public async Task<ApiRequestResult> GetOperationLogByIdAsync(Guid id)
    {
        var log = await _repository.FindAsync(id);
        if (log is null)
        {
            return new ApiRequestResult { Success = false, Message = "日志不存在" };
        }

        var logDto = new OperationLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            UserName = log.UserName,
            RealName = log.RealName,
            OperationType = log.OperationType,
            Module = log.Module,
            Description = log.Description,
            RequestMethod = log.RequestMethod,
            RequestPath = log.RequestPath,
            RequestParams = log.RequestParams,
            ResponseResult = log.ResponseResult,
            IpAddress = log.IpAddress,
            Status = log.Status,
            ErrorMessage = log.ErrorMessage,
            Duration = log.Duration,
            Browser = log.Browser,
            OsInfo = log.OsInfo,
            CreatedAt = log.CreatedAt
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = logDto };
    }

    /// <summary>
    /// 创建操作日志
    /// </summary>
    public async Task<ApiRequestResult> CreateOperationLogAsync(OperationLog log)
    {
        await _repository.AddAsync(log);
        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "日志记录成功", Data = log.Id };
    }

    /// <summary>
    /// 删除操作日志
    /// </summary>
    public async Task<ApiRequestResult> DeleteOperationLogAsync(Guid id)
    {
        var log = await _repository.FindAsync(id);
        if (log is null)
        {
            return new ApiRequestResult { Success = false, Message = "日志不存在" };
        }

        _repository.Remove(log);
        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "删除成功" };
    }

    /// <summary>
    /// 批量删除操作日志
    /// </summary>
    public async Task<ApiRequestResult> BatchDeleteOperationLogsAsync(List<Guid> ids)
    {
        if (ids is null || ids.Count == 0)
        {
            return new ApiRequestResult { Success = false, Message = "请选择要删除的日志" };
        }

        var logs = await _repository.GetListAsync(l => ids.Contains(l.Id));
        if (logs is null || !logs.Any())
        {
            return new ApiRequestResult { Success = false, Message = "未找到要删除的日志" };
        }

        foreach (var log in logs)
        {
            _repository.Remove(log);
        }

        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功删除 {logs.Count()} 条日志" };
    }

    /// <summary>
    /// 清空指定时间范围的操作日志
    /// </summary>
    public async Task<ApiRequestResult> ClearOperationLogsAsync(DateTime? startTime, DateTime? endTime)
    {
        var predicate = BuildTimeRangePredicate(startTime, endTime);

        var count = await _repository.CountAsync(predicate);
        if (count == 0)
        {
            return new ApiRequestResult { Success = true, Message = "指定时间范围内没有日志" };
        }

        var logs = await _repository.GetListAsync(predicate);
        foreach (var log in logs)
        {
            _repository.Remove(log);
        }

        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功清空 {count} 条日志" };
    }

    /// <summary>
    /// 获取操作类型统计
    /// </summary>
    public async Task<ApiRequestResult> GetOperationTypeStatisticsAsync(DateTime? startTime, DateTime? endTime)
    {
        var predicate = BuildTimeRangePredicate(startTime, endTime);

        var logs = await _repository.GetListAsync(predicate);
        var statistics = logs
            .GroupBy(l => l.OperationType)
            .Select(g => new { OperationType = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = statistics };
    }

    /// <summary>
    /// 获取操作模块统计
    /// </summary>
    public async Task<ApiRequestResult> GetModuleStatisticsAsync(DateTime? startTime, DateTime? endTime)
    {
        var predicate = BuildTimeRangePredicate(startTime, endTime);

        var logs = await _repository.GetListAsync(predicate);
        var statistics = logs
            .GroupBy(l => l.Module)
            .Select(g => new { Module = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = statistics };
    }

    /// <summary>
    /// 获取操作日志导出数据
    /// </summary>
    public async Task<ApiRequestResult> GetExportDataAsync(OperationLogQueryRequest request)
    {
        // 构建查询条件
        var predicate = BuildQueryPredicate(request);

        // 获取所有符合条件的日志（不分页）
        var logs = await _repository.GetListAsync(
            predicate,
            q => q.OrderByDescending(l => l.CreatedAt),
            0,
            10000 // 限制最大导出数量
        );

        // 转换为 DTO
        var logDtos = logs.Select(l => new OperationLogDto
        {
            Id = l.Id,
            UserId = l.UserId,
            UserName = l.UserName,
            RealName = l.RealName,
            OperationType = l.OperationType,
            Module = l.Module,
            Description = l.Description,
            RequestMethod = l.RequestMethod,
            RequestPath = l.RequestPath,
            RequestParams = l.RequestParams,
            ResponseResult = l.ResponseResult,
            IpAddress = l.IpAddress,
            Status = l.Status,
            ErrorMessage = l.ErrorMessage,
            Duration = l.Duration,
            Browser = l.Browser,
            OsInfo = l.OsInfo,
            CreatedAt = l.CreatedAt
        }).ToList();

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = logDtos };
    }

    /// <summary>
    /// 构建查询条件表达式
    /// </summary>
    private Expression<Func<OperationLog, bool>> BuildQueryPredicate(OperationLogQueryRequest request)
    {
        var predicate = PredicateBuilder.True<OperationLog>();

        if (!string.IsNullOrEmpty(request.UserName))
        {
            predicate = predicate.And(l => l.UserName.Contains(request.UserName));
        }

        if (!string.IsNullOrEmpty(request.OperationType))
        {
            predicate = predicate.And(l => l.OperationType == request.OperationType);
        }

        if (!string.IsNullOrEmpty(request.Module))
        {
            predicate = predicate.And(l => l.Module == request.Module);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            predicate = predicate.And(l => l.Status == request.Status);
        }

        if (request.StartTime is not null)
        {
            predicate = predicate.And(l => l.CreatedAt >= request.StartTime);
        }

        if (request.EndTime is not null)
        {
            predicate = predicate.And(l => l.CreatedAt <= request.EndTime);
        }

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            predicate = predicate.And(l => l.Description.Contains(request.Keyword));
        }

        return predicate;
    }

    /// <summary>
    /// 构建时间范围查询条件表达式
    /// </summary>
    private Expression<Func<OperationLog, bool>> BuildTimeRangePredicate(DateTime? startTime, DateTime? endTime)
    {
        var predicate = PredicateBuilder.True<OperationLog>();

        if (startTime is not null)
        {
            predicate = predicate.And(l => l.CreatedAt >= startTime);
        }

        if (endTime is not null)
        {
            predicate = predicate.And(l => l.CreatedAt <= endTime);
        }

        return predicate;
    }

    /// <summary>
    /// 导出操作日志为 Excel 文件
    /// </summary>
    public async Task<byte[]> ExportToExcelAsync(OperationLogQueryRequest request)
    {
        // 构建查询条件
        var predicate = BuildQueryPredicate(request);

        // 获取所有符合条件的日志（不分页）
        var logs = await _repository.GetListAsync(
            predicate,
            q => q.OrderByDescending(l => l.CreatedAt),
            0,
            10000 // 限制最大导出数量
        );

        // 操作类型中文映射
        var operationTypeMap = new Dictionary<string, string>
        {
            { "Create", "创建" },
            { "Update", "更新" },
            { "Delete", "删除" },
            { "Export", "导出" },
            { "Import", "导入" },
            { "Enable", "启用" },
            { "Disable", "禁用" },
            { "Login", "登录" },
            { "Logout", "登出" },
            { "Assign", "分配" },
            { "Other", "其他" }
        };

        // 模块中文映射
        var moduleMap = new Dictionary<string, string>
        {
            { "User", "用户" },
            { "Role", "角色" },
            { "Menu", "菜单" },
            { "Permission", "权限" },
            { "Setting", "设置" },
            { "Log", "日志" },
            { "Cache", "缓存" },
            { "Other", "其他" }
        };

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var worksheet = workbook.Worksheets.Add("操作日志");

        // 设置表头
        var headers = new[] { "序号", "操作用户", "操作类型", "操作模块", "操作描述", "请求方法", "请求路径", "IP地址", "状态", "耗时(ms)", "操作时间" };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
        }

        // 填充数据
        var logList = logs.ToList();
        for (int i = 0; i < logList.Count; i++)
        {
            var log = logList[i];
            var row = i + 2;

            worksheet.Cell(row, 1).Value = i + 1;
            worksheet.Cell(row, 2).Value = log.RealName ?? log.UserName;
            worksheet.Cell(row, 3).Value = operationTypeMap.GetValueOrDefault(log.OperationType, log.OperationType);
            worksheet.Cell(row, 4).Value = moduleMap.GetValueOrDefault(log.Module, log.Module);
            worksheet.Cell(row, 5).Value = log.Description ?? "";
            worksheet.Cell(row, 6).Value = log.RequestMethod;
            worksheet.Cell(row, 7).Value = log.RequestPath;
            worksheet.Cell(row, 8).Value = log.IpAddress ?? "";
            worksheet.Cell(row, 9).Value = log.Status == "Success" ? "成功" : "失败";
            worksheet.Cell(row, 10).Value = log.Duration;
            worksheet.Cell(row, 11).Value = log.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // 自动调整列宽
        worksheet.Columns().AdjustToContents();

        // 导出为字节数组
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

/// <summary>
/// PredicateBuilder 用于构建动态 LINQ 表达式
/// </summary>
public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> True<T>() { return f => true; }
    public static Expression<Func<T, bool>> False<T>() { return f => false; }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>(Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>(Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
    }
}