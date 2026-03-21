using DDDProject.Application.DTOs;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 角色应用服务接口
/// </summary>
public interface IRoleService : IApplicationService
{
    /// <summary>
    /// 检查用户是否有访问特定菜单的权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="menuId">菜单ID</param>
    /// <returns></returns>
    Task<ApiRequestResult> HasMenuPermissionAsync(Guid userId, Guid menuId);
    
    /// <summary>
    /// 获取用户的菜单权限列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    Task<ApiRequestResult> GetUserMenusAsync(Guid userId);
}