using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.API.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Controllers;
using DDDProject.Application.Common;

namespace DDDProject.API.Filters;

/// <summary>
/// 操作日志记录过滤器
/// 自动记录控制器中的增删改查导出等操作日志
/// </summary>
public class OperationLogFilter : IAsyncActionFilter
{
    private readonly IOperationLogService _logService;
    private readonly ICurrentUserContext _currentUser;

    public OperationLogFilter(IOperationLogService logService, ICurrentUserContext currentUser)
    {
        _logService = logService;
        _currentUser = currentUser;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
        if (actionDescriptor is null)
        {
            await next();
            return;
        }

        // 获取 ApiSearch 注解，用于确定操作名称和模块
        var apiSearchAttribute = actionDescriptor.MethodInfo
            .GetCustomAttributes(typeof(ApiSearchAttribute), inherit: true)
            .FirstOrDefault() as ApiSearchAttribute;

        // 如果没有 ApiSearch 注解，跳过日志记录
        if (apiSearchAttribute is null)
        {
            await next();
            return;
        }

        // 判断是否需要记录日志（只记录增删改导出等操作，不记录普通查询）
        var httpMethod = context.HttpContext.Request.Method;
        var shouldLog = ShouldRecordLog(httpMethod, actionDescriptor.MethodInfo.Name);

        if (!shouldLog)
        {
            await next();
            return;
        }

        // 记录开始时间
        var stopwatch = Stopwatch.StartNew();

        // 获取请求参数
        var requestParams = GetRequestParameters(context);

        // 获取客户端信息
        var ipAddress = GetClientIpAddress(context.HttpContext);
        var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
        var (browser, osInfo) = ParseUserAgent(userAgent);

        // 创建日志实体
        var log = OperationLog.Create(
            _currentUser.IsAuthenticated ? _currentUser.UserId : null,
            _currentUser.IsAuthenticated ? _currentUser.UserName : "Anonymous",
            _currentUser.IsAuthenticated ? _currentUser.RealName : null,
            DetermineOperationType(httpMethod, actionDescriptor.MethodInfo.Name),
            DetermineModule(apiSearchAttribute.Category),
            apiSearchAttribute.Name,
            httpMethod,
            context.HttpContext.Request.Path.Value ?? string.Empty,
            requestParams,
            ipAddress,
            browser,
            osInfo
        );

        // 执行操作
        var resultContext = await next();

        // 记录结束时间
        stopwatch.Stop();

        // 设置响应结果
        var responseResult = GetResponseResult(resultContext);
        var status = resultContext.Exception is null ? "Success" : "Failure";
        var errorMessage = resultContext.Exception?.Message;

        log.SetResponseResult(responseResult, stopwatch.ElapsedMilliseconds, status, errorMessage);

        // 直接保存日志（在请求上下文中执行，确保 DbContext 可用）
        try
        {
            await _logService.CreateOperationLogAsync(log);
        }
        catch (Exception ex)
        {
            // 日志记录失败不影响主流程，仅记录错误
            Console.WriteLine($"[OperationLogFilter] 日志保存失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 判断是否需要记录日志
    /// 只记录 POST、PUT、DELETE 操作（增删改），以及带有 Export 关键字的方法
    /// </summary>
    private bool ShouldRecordLog(string httpMethod, string methodName)
    {
        // POST、PUT、DELETE 操作都需要记录
        if (httpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
            httpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
            httpMethod.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // 导出操作需要记录
        if (methodName.Contains("Export", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // 登录操作需要记录
        if (methodName.Contains("Login", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 确定操作类型
    /// </summary>
    private string DetermineOperationType(string httpMethod, string methodName)
    {
        // 根据方法名判断
        if (methodName.Contains("Create", StringComparison.OrdinalIgnoreCase) ||
            methodName.Contains("Add", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.Create;
        }

        if (methodName.Contains("Update", StringComparison.OrdinalIgnoreCase) ||
            methodName.Contains("Edit", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.Update;
        }

        if (methodName.Contains("Delete", StringComparison.OrdinalIgnoreCase) ||
            methodName.Contains("Remove", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.Delete;
        }

        if (methodName.Contains("Export", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.Export;
        }

        if (methodName.Contains("Import", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.Import;
        }

        if (methodName.Contains("Enable", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.Enable;
        }

        if (methodName.Contains("Disable", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.Disable;
        }

        if (methodName.Contains("Login", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.Login;
        }

        if (methodName.Contains("Logout", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.Logout;
        }

        if (methodName.Contains("Assign", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.Assign;
        }

        if (methodName.Contains("ResetPassword", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.ResetPassword;
        }

        if (methodName.Contains("ChangePassword", StringComparison.OrdinalIgnoreCase))
        {
            return OperationType.ChangePassword;
        }

        // 根据 HTTP 方法判断
        return httpMethod.ToUpperInvariant() switch
        {
            "POST" => OperationType.Create,
            "PUT" => OperationType.Update,
            "DELETE" => OperationType.Delete,
            _ => OperationType.Other
        };
    }

    /// <summary>
    /// 确定操作模块
    /// </summary>
    private string DetermineModule(ApiSearchCategory category)
    {
        return category switch
        {
            ApiSearchCategory.Login => OperationModule.Login,
            ApiSearchCategory.Menu => OperationModule.Menu,
            ApiSearchCategory.User => OperationModule.User,
            ApiSearchCategory.Role => OperationModule.Role,
            ApiSearchCategory.Dictionary => OperationModule.Other,
            ApiSearchCategory.Log => OperationModule.Log,
            ApiSearchCategory.Setting => OperationModule.Setting,
            ApiSearchCategory.File => OperationModule.Other,
            ApiSearchCategory.Other => OperationModule.Other,
            _ => OperationModule.Other
        };
    }

    /// <summary>
    /// 获取请求参数
    /// </summary>
    private string? GetRequestParameters(ActionExecutingContext context)
    {
        try
        {
            var parameters = context.ActionArguments
                .Where(p => p.Value is not null && !p.Key.Equals("cancellationToken", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(p => p.Key, p => p.Value);

            if (parameters.Count == 0)
            {
                return null;
            }

            return JsonSerializer.Serialize(parameters, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                MaxDepth = 3 // 限制深度，避免序列化过深
            });
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取响应结果
    /// </summary>
    private string? GetResponseResult(ActionExecutedContext context)
    {
        try
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value is not null)
            {
                return JsonSerializer.Serialize(objectResult.Value, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    MaxDepth = 3
                });
            }

            if (context.Result is JsonResult jsonResult && jsonResult.Value is not null)
            {
                return JsonSerializer.Serialize(jsonResult.Value, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    MaxDepth = 3
                });
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取客户端IP地址
    /// </summary>
    private string GetClientIpAddress(HttpContext context)
    {
        // 尝试从 X-Forwarded-For 获取真实IP（适用于反向代理场景）
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }

        // 尝试从 X-Real-IP 获取
        var realIp = context.Request.Headers["X-Real-IP"].ToString();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // 使用连接IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    /// <summary>
    /// 解析 User-Agent 获取浏览器和操作系统信息
    /// </summary>
    private (string? browser, string? osInfo) ParseUserAgent(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
        {
            return (null, null);
        }

        string? browser = null;
        string? osInfo = null;

        // 解析浏览器
        if (userAgent.Contains("Chrome") && !userAgent.Contains("Edg"))
        {
            browser = "Chrome";
        }
        else if (userAgent.Contains("Edg"))
        {
            browser = "Edge";
        }
        else if (userAgent.Contains("Firefox"))
        {
            browser = "Firefox";
        }
        else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
        {
            browser = "Safari";
        }
        else if (userAgent.Contains("MSIE") || userAgent.Contains("Trident"))
        {
            browser = "IE";
        }

        // 解析操作系统
        if (userAgent.Contains("Windows NT 10"))
        {
            osInfo = "Windows 10";
        }
        else if (userAgent.Contains("Windows NT 6.3"))
        {
            osInfo = "Windows 8.1";
        }
        else if (userAgent.Contains("Windows NT 6.2"))
        {
            osInfo = "Windows 8";
        }
        else if (userAgent.Contains("Windows NT 6.1"))
        {
            osInfo = "Windows 7";
        }
        else if (userAgent.Contains("Mac OS X"))
        {
            osInfo = "Mac OS X";
        }
        else if (userAgent.Contains("Linux"))
        {
            osInfo = "Linux";
        }
        else if (userAgent.Contains("Android"))
        {
            osInfo = "Android";
        }
        else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
        {
            osInfo = "iOS";
        }

        return (browser, osInfo);
    }
}