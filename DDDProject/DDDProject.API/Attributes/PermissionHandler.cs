using Microsoft.AspNetCore.Authorization;
using DDDProject.Application.Common;
using DDDProject.Application.Interfaces;

namespace DDDProject.API.Attributes;

/// <summary>
/// 权限授权处理程序 - 处理 PermissionRequirement 的授权验证
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PermissionHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    /// <param name="logger">日志记录器</param>
    public PermissionHandler(IServiceProvider serviceProvider, ILogger<PermissionHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// 处理授权要求
    /// </summary>
    /// <param name="context">授权上下文</param>
    /// <param name="requirement">权限要求</param>
    /// <returns>任务</returns>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // 获取当前用户ID
        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            _logger.LogWarning("权限验证失败：无法获取用户ID");
            return;
        }

        // 创建作用域以获取权限服务
        using var scope = _serviceProvider.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        // 根据验证逻辑检查权限
        bool hasPermission = false;

        if (requirement.Logic == PermissionLogic.Any)
        {
            // 满足任一权限即可
            foreach (var permissionCode in requirement.PermissionCodes)
            {
                var result = await permissionService.HasPermissionAsync(userId, permissionCode);
                if (result.Success && result.Data is true)
                {
                    hasPermission = true;
                    _logger.LogDebug("用户 {UserId} 拥有权限 {PermissionCode}", userId, permissionCode);
                    break;
                }
            }
        }
        else
        {
            // 需要满足所有权限
            hasPermission = true;
            foreach (var permissionCode in requirement.PermissionCodes)
            {
                var result = await permissionService.HasPermissionAsync(userId, permissionCode);
                if (!result.Success || result.Data is not true)
                {
                    hasPermission = false;
                    _logger.LogDebug("用户 {UserId} 缺少权限 {PermissionCode}", userId, permissionCode);
                    break;
                }
            }
        }

        if (hasPermission)
        {
            context.Succeed(requirement);
            _logger.LogInformation("用户 {UserId} 权限验证通过，权限编码：{PermissionCodes}",
                userId, string.Join(",", requirement.PermissionCodes));
        }
        else
        {
            _logger.LogWarning("用户 {UserId} 权限验证失败，权限编码：{PermissionCodes}，验证逻辑：{Logic}",
                userId, string.Join(",", requirement.PermissionCodes), requirement.Logic);
        }
    }
} 
