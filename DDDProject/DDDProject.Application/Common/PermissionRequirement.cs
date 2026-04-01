using Microsoft.AspNetCore.Authorization;

namespace DDDProject.Application.Common;

/// <summary>
/// 权限要求 - 用于授权策略中的权限验证
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// 权限编码列表
    /// </summary>
    public List<string> PermissionCodes { get; }

    /// <summary>
    /// 权限验证逻辑
    /// </summary>
    public PermissionLogic Logic { get; }

    /// <summary>
    /// 构造函数 - 单个权限
    /// </summary>
    /// <param name="permissionCode">权限编码</param>
    public PermissionRequirement(string permissionCode)
    {
        PermissionCodes = new List<string> { permissionCode };
        Logic = PermissionLogic.Any;
    }

    /// <summary>
    /// 构造函数 - 多个权限
    /// </summary>
    /// <param name="permissionCodes">权限编码列表</param>
    /// <param name="logic">验证逻辑</param>
    public PermissionRequirement(List<string> permissionCodes, PermissionLogic logic = PermissionLogic.Any)
    {
        PermissionCodes = permissionCodes ?? new List<string>();
        Logic = logic;
    }
} 
