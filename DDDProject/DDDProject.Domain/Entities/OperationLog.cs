namespace DDDProject.Domain.Entities;

/// <summary>
/// 操作日志实体
/// </summary>
public class OperationLog : AggregateRoot
{
    /// <summary>
    /// 操作用户ID
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// 操作用户名
    /// </summary>
    public string UserName { get; private set; } = string.Empty;

    /// <summary>
    /// 操作用户真实姓名
    /// </summary>
    public string? RealName { get; private set; }

    /// <summary>
    /// 操作类型：Create, Update, Delete, Query, Export, Import, Enable, Disable, Login, Other
    /// </summary>
    public string OperationType { get; private set; } = string.Empty;

    /// <summary>
    /// 操作模块：User, Role, Menu, Permission, Setting, Log, Other
    /// </summary>
    public string Module { get; private set; } = string.Empty;

    /// <summary>
    /// 操作描述
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// 请求方法：GET, POST, PUT, DELETE
    /// </summary>
    public string RequestMethod { get; private set; } = string.Empty;

    /// <summary>
    /// 请求路径
    /// </summary>
    public string RequestPath { get; private set; } = string.Empty;

    /// <summary>
    /// 请求参数（JSON格式）
    /// </summary>
    public string? RequestParams { get; private set; }

    /// <summary>
    /// 响应结果（JSON格式）
    /// </summary>
    public string? ResponseResult { get; private set; }

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    public string IpAddress { get; private set; } = string.Empty;

    /// <summary>
    /// 执行状态：Success, Failure
    /// </summary>
    public string Status { get; private set; } = "Success";

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    public long Duration { get; private set; }

    /// <summary>
    /// 浏览器信息
    /// </summary>
    public string? Browser { get; private set; }

    /// <summary>
    /// 操作系统信息
    /// </summary>
    public string? OsInfo { get; private set; }

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected OperationLog() { }

    /// <summary>
    /// 创建操作日志
    /// </summary>
    public static OperationLog Create(
        Guid? userId,
        string userName,
        string? realName,
        string operationType,
        string module,
        string description,
        string requestMethod,
        string requestPath,
        string? requestParams,
        string ipAddress,
        string? browser = null,
        string? osInfo = null)
    {
        return new OperationLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = userName,
            RealName = realName,
            OperationType = operationType,
            Module = module,
            Description = description,
            RequestMethod = requestMethod,
            RequestPath = requestPath,
            RequestParams = requestParams,
            IpAddress = ipAddress,
            Status = "Success",
            Browser = browser,
            OsInfo = osInfo,
            CreatedAt = DateTime.Now
        };
    }

    /// <summary>
    /// 设置响应结果
    /// </summary>
    public void SetResponseResult(string? responseResult, long duration, string status = "Success", string? errorMessage = null)
    {
        ResponseResult = responseResult;
        Duration = duration;
        Status = status;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 设置执行失败
    /// </summary>
    public void SetFailure(string errorMessage, long duration)
    {
        Status = "Failure";
        ErrorMessage = errorMessage;
        Duration = duration;
        UpdatedAt = DateTime.Now;
    }
}

/// <summary>
/// 操作类型常量
/// </summary>
public static class OperationType
{
    public const string Create = "Create";
    public const string Update = "Update";
    public const string Delete = "Delete";
    public const string Query = "Query";
    public const string Export = "Export";
    public const string Import = "Import";
    public const string Enable = "Enable";
    public const string Disable = "Disable";
    public const string Login = "Login";
    public const string Logout = "Logout";
    public const string Assign = "Assign";
    public const string ResetPassword = "ResetPassword";
    public const string ChangePassword = "ChangePassword";
    public const string Other = "Other";
}

/// <summary>
/// 操作模块常量
/// </summary>
public static class OperationModule
{
    public const string User = "User";
    public const string Role = "Role";
    public const string Menu = "Menu";
    public const string Permission = "Permission";
    public const string Button = "Button";
    public const string Setting = "Setting";
    public const string Log = "Log";
    public const string Login = "Login";
    public const string Cache = "Cache";
    public const string Other = "Other";
}