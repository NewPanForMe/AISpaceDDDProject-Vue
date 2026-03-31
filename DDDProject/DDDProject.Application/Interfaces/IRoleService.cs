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
    /// 获取用户的角色详情列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    Task<ApiRequestResult> GetUserRolesAsync(Guid userId);

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

/// <summary>
/// 系统设置应用服务接口
/// </summary>
public interface ISettingService : IApplicationService
{
    /// <summary>
    /// 获取所有设置
    /// </summary>
    Task<ApiRequestResult> GetAllSettingsAsync();

    /// <summary>
    /// 根据分组获取设置
    /// </summary>
    /// <param name="group">设置分组</param>
    Task<ApiRequestResult> GetSettingsByGroupAsync(string group);

    /// <summary>
    /// 根据键获取设置值
    /// </summary>
    /// <param name="key">设置键</param>
    Task<ApiRequestResult> GetSettingByKeyAsync(string key);

    /// <summary>
    /// 更新单个设置
    /// </summary>
    /// <param name="request">更新设置请求</param>
    Task<ApiRequestResult> UpdateSettingAsync(UpdateSettingRequest request);

    /// <summary>
    /// 批量更新设置
    /// </summary>
    /// <param name="request">批量更新设置请求</param>
    Task<ApiRequestResult> BatchUpdateSettingsAsync(BatchUpdateSettingsRequest request);
}

/// <summary>
/// 权限应用服务接口
/// </summary>
public interface IPermissionService : IApplicationService
{
    /// <summary>
    /// 获取权限列表（分页）
    /// </summary>
    /// <param name="request">分页请求</param>
    /// <param name="module">模块名称（可选，用于筛选）</param>
    Task<ApiRequestResult> GetPermissionsAsync(PagedRequest request, string? module = null);

    /// <summary>
    /// 获取所有启用的权限列表
    /// </summary>
    Task<ApiRequestResult> GetAllEnabledPermissionsAsync();

    /// <summary>
    /// 根据模块获取权限列表
    /// </summary>
    /// <param name="module">模块名称</param>
    Task<ApiRequestResult> GetPermissionsByModuleAsync(string module);

    /// <summary>
    /// 获取权限详情
    /// </summary>
    Task<ApiRequestResult> GetPermissionByIdAsync(Guid id);

    /// <summary>
    /// 创建权限
    /// </summary>
    Task<ApiRequestResult> CreatePermissionAsync(CreatePermissionRequest request);

    /// <summary>
    /// 更新权限
    /// </summary>
    Task<ApiRequestResult> UpdatePermissionAsync(UpdatePermissionRequest request);

    /// <summary>
    /// 删除权限
    /// </summary>
    Task<ApiRequestResult> DeletePermissionAsync(Guid id);

    /// <summary>
    /// 启用权限
    /// </summary>
    Task<ApiRequestResult> EnablePermissionAsync(Guid id);

    /// <summary>
    /// 禁用权限
    /// </summary>
    Task<ApiRequestResult> DisablePermissionAsync(Guid id);

    /// <summary>
    /// 获取角色的权限ID列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    Task<ApiRequestResult> GetRolePermissionIdsAsync(Guid roleId);

    /// <summary>
    /// 为角色分配权限
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="permissionIds">权限ID列表</param>
    Task<ApiRequestResult> AssignRolePermissionsAsync(Guid roleId, List<Guid> permissionIds);

    /// <summary>
    /// 获取用户的权限列表（通过角色）
    /// </summary>
    /// <param name="userId">用户ID</param>
    Task<ApiRequestResult> GetUserPermissionsAsync(Guid userId);

    /// <summary>
    /// 检查用户是否有指定权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionCode">权限编码</param>
    Task<ApiRequestResult> HasPermissionAsync(Guid userId, string permissionCode);
}