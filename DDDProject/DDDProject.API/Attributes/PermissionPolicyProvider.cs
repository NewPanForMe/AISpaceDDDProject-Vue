using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using DDDProject.Application.Common;

namespace DDDProject.API.Attributes;

/// <summary>
/// 自定义权限策略提供者 - 动态处理 PermissionAttribute 的策略
/// </summary>
public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _defaultPolicyProvider;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">授权选项</param>
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _defaultPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    /// <summary>
    /// 获取默认策略
    /// </summary>
    /// <returns>默认授权策略</returns>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _defaultPolicyProvider.GetDefaultPolicyAsync();
    }

    /// <summary>
    /// 获取回退策略
    /// </summary>
    /// <returns>回退授权策略</returns>
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _defaultPolicyProvider.GetFallbackPolicyAsync();
    }

    /// <summary>
    /// 根据策略名称获取策略
    /// </summary>
    /// <param name="policyName">策略名称</param>
    /// <returns>授权策略</returns>
    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // 检查是否是权限策略（以 "Permission_" 开头）
        if (policyName.StartsWith("Permission_", StringComparison.OrdinalIgnoreCase))
        {
            // 解析权限编码
            var permissionPart = policyName.Substring("Permission_".Length);

            // 检查是否包含多个权限和逻辑
            PermissionLogic logic = PermissionLogic.Any;
            List<string> permissionCodes;

            if (permissionPart.EndsWith("_Any"))
            {
                logic = PermissionLogic.Any;
                permissionPart = permissionPart.Substring(0, permissionPart.Length - 4);
                permissionCodes = permissionPart.Split('_').ToList();
            }
            else if (permissionPart.EndsWith("_All"))
            {
                logic = PermissionLogic.All;
                permissionPart = permissionPart.Substring(0, permissionPart.Length - 4);
                permissionCodes = permissionPart.Split('_').ToList();
            }
            else
            {
                // 单个权限
                permissionCodes = new List<string> { permissionPart };
            }

            // 创建策略
            var policy = new AuthorizationPolicyBuilder();
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new PermissionRequirement(permissionCodes, logic));
            return policy.Build();
        }

        // 如果不是权限策略，使用默认策略提供者
        return await _defaultPolicyProvider.GetPolicyAsync(policyName);
    }
} 
