using DDDProject.Application.DTOs;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 角色应用服务接口
/// </summary>
public interface IRoleService : IApplicationService
{
    /// <summary>
    /// 获取角色列表（分页）
    /// </summary>
    Task<ApiRequestResult> GetRolesAsync(PagedRequest request);

    /// <summary>
    /// 获取角色详情
    /// </summary>
    Task<ApiRequestResult> GetRoleByIdAsync(Guid id);

    /// <summary>
    /// 创建角色
    /// </summary>
    Task<ApiRequestResult> CreateRoleAsync(CreateRoleRequest request);

    /// <summary>
    /// 更新角色
    /// </summary>
    Task<ApiRequestResult> UpdateRoleAsync(UpdateRoleRequest request);

    /// <summary>
    /// 删除角色
    /// </summary>
    Task<ApiRequestResult> DeleteRoleAsync(Guid id);

    /// <summary>
    /// 启用角色
    /// </summary>
    Task<ApiRequestResult> EnableRoleAsync(Guid id);

    /// <summary>
    /// 禁用角色
    /// </summary>
    Task<ApiRequestResult> DisableRoleAsync(Guid id);

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

    /// <summary>
    /// 获取用户的角色ID列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    Task<ApiRequestResult> GetUserRoleIdsAsync(Guid userId);

    /// <summary>
    /// 配置用户角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleIds">角色ID列表</param>
    Task<ApiRequestResult> AssignUserRolesAsync(Guid userId, List<Guid> roleIds);

    /// <summary>
    /// 获取所有启用的角色列表（用于下拉选择）
    /// </summary>
    Task<ApiRequestResult> GetEnabledRolesAsync();

    /// <summary>
    /// 获取角色的用户ID列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    Task<ApiRequestResult> GetRoleUserIdsAsync(Guid roleId);

    /// <summary>
    /// 为角色分配用户
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="userIds">用户ID列表</param>
    Task<ApiRequestResult> AssignRoleUsersAsync(Guid roleId, List<Guid> userIds);
}