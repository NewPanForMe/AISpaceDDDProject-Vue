namespace DDDProject.Application.DTOs;

/// <summary>
/// 操作日志 DTO
/// </summary>
public class OperationLogDto
{
    /// <summary>
    /// 日志ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 操作用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 操作用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 操作用户真实姓名
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// 操作模块
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 操作描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 请求方法
    /// </summary>
    public string RequestMethod { get; set; } = string.Empty;

    /// <summary>
    /// 请求路径
    /// </summary>
    public string RequestPath { get; set; } = string.Empty;

    /// <summary>
    /// 请求参数
    /// </summary>
    public string? RequestParams { get; set; }

    /// <summary>
    /// 响应结果
    /// </summary>
    public string? ResponseResult { get; set; }

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// 执行状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// 浏览器信息
    /// </summary>
    public string? Browser { get; set; }

    /// <summary>
    /// 操作系统信息
    /// </summary>
    public string? OsInfo { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 操作日志查询请求
/// </summary>
public class OperationLogQueryRequest : PagedRequest
{
    /// <summary>
    /// 用户名（模糊查询）
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public string? OperationType { get; set; }

    /// <summary>
    /// 操作模块
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// 执行状态
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 关键词（搜索描述）
    /// </summary>
    public string? Keyword { get; set; }
}

/// <summary>
/// 创建日志请求（用于手动记录日志）
/// </summary>
public class CreateOperationLogRequest
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// 操作模块
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 操作描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 请求参数（可选）
    /// </summary>
    public string? RequestParams { get; set; }

    /// <summary>
    /// 响应结果（可选）
    /// </summary>
    public string? ResponseResult { get; set; }

    /// <summary>
    /// 执行状态（可选，默认 Success）
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 错误信息（可选）
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 执行耗时（可选）
    /// </summary>
    public long? Duration { get; set; }
}