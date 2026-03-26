using DDDProject.Application.DTOs;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 菜单角色应用服务接口
/// </summary>
public interface IMenuRoleService : IApplicationService
{
    /// <summary>
    /// 获取角色的菜单ID列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    Task<ApiRequestResult> GetRoleMenuIdsAsync(Guid roleId);

    /// <summary>
    /// 根据用户ID获取菜单ID列表（通过用户角色关联）
    /// </summary>
    /// <param name="userId">用户ID</param>
    Task<ApiRequestResult> GetRoleMenuIdsByUserIdAsync(Guid userId);

    /// <summary>
    /// 获取菜单的角色ID列表
    /// </summary>
    /// <param name="menuId">菜单ID</param>
    Task<ApiRequestResult> GetMenuRoleIdsAsync(Guid menuId);

    /// <summary>
    /// 为角色分配菜单
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="menuIds">菜单ID列表</param>
    Task<ApiRequestResult> AssignRoleMenusAsync(Guid roleId, List<Guid> menuIds);

    /// <summary>
    /// 为菜单分配角色
    /// </summary>
    /// <param name="menuId">菜单ID</param>
    /// <param name="roleIds">角色ID列表</param>
    Task<ApiRequestResult> AssignMenuRolesAsync(Guid menuId, List<Guid> roleIds);

    /// <summary>
    /// 获取角色的菜单列表（树形结构）
    /// </summary>
    /// <param name="roleId">角色ID</param>
    Task<ApiRequestResult> GetRoleMenusAsync(Guid roleId);

    /// <summary>
    /// 获取用户的所有菜单（通过角色关联）
    /// </summary>
    /// <param name="userId">用户ID</param>
    Task<ApiRequestResult> GetUserMenusByRolesAsync(Guid userId);

    /// <summary>
    /// 清除角色的所有菜单权限
    /// </summary>
    /// <param name="roleId">角色ID</param>
    Task<ApiRequestResult> ClearRoleMenusAsync(Guid roleId);

    /// <summary>
    /// 清除菜单的所有角色关联
    /// </summary>
    /// <param name="menuId">菜单ID</param>
    Task<ApiRequestResult> ClearMenuRolesAsync(Guid menuId);
}