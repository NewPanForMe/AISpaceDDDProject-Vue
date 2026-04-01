using Microsoft.AspNetCore.Authorization;

namespace DDDProject.Application.Common;

/// <summary>
/// 权限验证特性 - 用于标记需要特定权限编码才能访问的 API 方法
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class PermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// 权限编码
    /// </summary>
    public string PermissionCode { get; }

    /// <summary>
    /// 权限验证逻辑 - 可选值：Any（满足任一权限即可）, All（需要满足所有权限）
    /// </summary>
    public PermissionLogic Logic { get; set; } = PermissionLogic.Any;

    /// <summary>
    /// 构造函数 - 单个权限
    /// </summary>
    /// <param name="permissionCode">权限编码，如 "user:add"</param>
    public PermissionAttribute(string permissionCode)
    {
        PermissionCode = permissionCode;
        // 使用自定义策略名称，格式为 Permission_权限编码
        Policy = $"Permission_{permissionCode}";
    }

    /// <summary>
    /// 构造函数 - 多个权限
    /// </summary>
    /// <param name="permissionCodes">权限编码数组</param>
    /// <param name="logic">验证逻辑</param>
    public PermissionAttribute(string[] permissionCodes, PermissionLogic logic = PermissionLogic.Any)
    {
        PermissionCode = string.Join(",", permissionCodes);
        Logic = logic;
        // 使用组合策略名称
        Policy = $"Permission_{string.Join("_", permissionCodes)}_{logic}";
    }
}

/// <summary>
/// 权限验证逻辑
/// </summary>
public enum PermissionLogic
{
    /// <summary>
    /// 满足任一权限即可
    /// </summary>
    Any,

    /// <summary>
    /// 需要满足所有权限
    /// </summary>
    All
}
 
