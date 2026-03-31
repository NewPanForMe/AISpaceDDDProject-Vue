using Microsoft.AspNetCore.Mvc;
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;
using Microsoft.AspNetCore.Authorization;

namespace DDDProject.API.Controllers;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class RoleController : BaseApiController
{
    private readonly IRoleService _roleService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleService">角色服务</param>
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// 获取角色列表（分页）
    /// </summary>
    [HttpGet]
    [ActionName("GetRolesAsync")]
    [ApiSearch(Name = "获取角色列表", Description = "返回角色列表（支持分页）", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetRolesAsync([FromQuery] int pageNum = 1, [FromQuery] int pageSize = 10)
    {
        var request = new PagedRequest
        {
            PageNumber = pageNum,
            PageSize = pageSize
        };
        return await _roleService.GetRolesAsync(request);
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    [HttpGet]
    [ActionName("GetRoleByIdAsync")]
    [ApiSearch(Name = "获取角色详情", Description = "根据ID获取角色详细信息", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetRoleByIdAsync([FromQuery] Guid id)
    {
        return await _roleService.GetRoleByIdAsync(id);
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    [HttpPost]
    [ActionName("CreateRoleAsync")]
    [ApiSearch(Name = "创建角色", Description = "创建新的角色", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> CreateRoleAsync([FromBody] CreateRoleRequest request)
    {
        return await _roleService.CreateRoleAsync(request);
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    [HttpPut]
    [ActionName("UpdateRoleAsync")]
    [ApiSearch(Name = "更新角色", Description = "更新现有角色", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> UpdateRoleAsync([FromBody] UpdateRoleRequest request)
    {
        return await _roleService.UpdateRoleAsync(request);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    [HttpDelete]
    [ActionName("DeleteRoleAsync")]
    [ApiSearch(Name = "删除角色", Description = "根据ID删除角色", Category = ApiSearchCategory.Role)]
    public async Task<IActionResult> DeleteRoleAsync([FromQuery] Guid id)
    {
        var result = await _roleService.DeleteRoleAsync(id);
        if (!result.Success)
            return BadRequest(result);
        return NoContent();
    }

    /// <summary>
    /// 启用角色
    /// </summary>
    [HttpPost]
    [ActionName("EnableRoleAsync")]
    [ApiSearch(Name = "启用角色", Description = "启用一个被禁用的角色", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> EnableRoleAsync([FromQuery] Guid id)
    {
        return await _roleService.EnableRoleAsync(id);
    }

    /// <summary>
    /// 禁用角色
    /// </summary>
    [HttpPost]
    [ActionName("DisableRoleAsync")]
    [ApiSearch(Name = "禁用角色", Description = "禁用一个启用的角色", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> DisableRoleAsync([FromQuery] Guid id)
    {
        return await _roleService.DisableRoleAsync(id);
    }

    /// <summary>
    /// 获取用户的角色ID列表
    /// </summary>
    [HttpGet]
    [ActionName("GetUserRoleIdsAsync")]
    [ApiSearch(Name = "获取用户角色", Description = "获取指定用户的角色ID列表", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetUserRoleIdsAsync([FromQuery] Guid userId)
    {
        return await _roleService.GetUserRoleIdsAsync(userId);
    }

    /// <summary>
    /// 获取用户的角色详情列表
    /// </summary>
    [HttpGet]
    [ActionName("GetUserRolesAsync")]
    [ApiSearch(Name = "获取用户角色详情", Description = "获取指定用户的角色详情列表", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetUserRolesAsync([FromQuery] Guid userId)
    {
        return await _roleService.GetUserRolesAsync(userId);
    }

    /// <summary>
    /// 配置用户角色
    /// </summary>
    [HttpPost]
    [ActionName("AssignUserRolesAsync")]
    [ApiSearch(Name = "配置用户角色", Description = "为指定用户分配角色", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> AssignUserRolesAsync([FromBody] AssignUserRolesRequest request)
    {
        return await _roleService.AssignUserRolesAsync(request.UserId, request.RoleIds);
    }

    /// <summary>
    /// 获取所有启用的角色列表
    /// </summary>
    [HttpGet]
    [ActionName("GetEnabledRolesAsync")]
    [ApiSearch(Name = "获取启用角色", Description = "获取所有启用的角色列表（用于下拉选择）", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetEnabledRolesAsync()
    {
        return await _roleService.GetEnabledRolesAsync();
    }

    /// <summary>
    /// 获取角色的用户ID列表
    /// </summary>
    [HttpGet]
    [ActionName("GetRoleUserIdsAsync")]
    [ApiSearch(Name = "获取角色用户", Description = "获取指定角色的用户ID列表", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetRoleUserIdsAsync([FromQuery] Guid roleId)
    {
        return await _roleService.GetRoleUserIdsAsync(roleId);
    }

    /// <summary>
    /// 为角色分配用户
    /// </summary>
    [HttpPost]
    [ActionName("AssignRoleUsersAsync")]
    [ApiSearch(Name = "配置角色用户", Description = "为指定角色分配用户", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> AssignRoleUsersAsync([FromBody] AssignRoleUsersRequest request)
    {
        return await _roleService.AssignRoleUsersAsync(request.RoleId, request.UserIds);
    }
}

/// <summary>
/// 系统设置控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class SettingController : BaseApiController
{
    private readonly ISettingService _settingService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="settingService">设置服务</param>
    public SettingController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    /// <summary>
    /// 获取所有设置
    /// </summary>
    [HttpGet]
    [ActionName("GetAllSettingsAsync")]
    [ApiSearch(Name = "获取所有设置", Description = "获取所有系统设置项", Category = ApiSearchCategory.Setting)]
    public async Task<ApiRequestResult> GetAllSettingsAsync()
    {
        return await _settingService.GetAllSettingsAsync();
    }

    /// <summary>
    /// 根据分组获取设置
    /// </summary>
    [HttpGet]
    [ActionName("GetSettingsByGroupAsync")]
    [ApiSearch(Name = "按分组获取设置", Description = "根据分组获取系统设置项", Category = ApiSearchCategory.Setting)]
    public async Task<ApiRequestResult> GetSettingsByGroupAsync([FromQuery] string group)
    {
        return await _settingService.GetSettingsByGroupAsync(group);
    }

    /// <summary>
    /// 根据键获取设置值
    /// </summary>
    [HttpGet]
    [ActionName("GetSettingByKeyAsync")]
    [ApiSearch(Name = "获取设置值", Description = "根据键获取设置值", Category = ApiSearchCategory.Setting)]
    public async Task<ApiRequestResult> GetSettingByKeyAsync([FromQuery] string key)
    {
        return await _settingService.GetSettingByKeyAsync(key);
    }

    /// <summary>
    /// 更新单个设置
    /// </summary>
    [HttpPost]
    [ActionName("UpdateSettingAsync")]
    [ApiSearch(Name = "更新设置", Description = "更新单个设置项", Category = ApiSearchCategory.Setting)]
    public async Task<ApiRequestResult> UpdateSettingAsync([FromBody] UpdateSettingRequest request)
    {
        return await _settingService.UpdateSettingAsync(request);
    }

    /// <summary>
    /// 批量更新设置
    /// </summary>
    [HttpPost]
    [ActionName("BatchUpdateSettingsAsync")]
    [ApiSearch(Name = "批量更新设置", Description = "批量更新多个设置项", Category = ApiSearchCategory.Setting)]
    public async Task<ApiRequestResult> BatchUpdateSettingsAsync([FromBody] BatchUpdateSettingsRequest request)
    {
        return await _settingService.BatchUpdateSettingsAsync(request);
    }
}

/// <summary>
/// 权限控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class PermissionController : BaseApiController
{
    private readonly IPermissionService _permissionService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="permissionService">权限服务</param>
    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// 获取权限列表（分页）
    /// </summary>
    [HttpGet]
    [ActionName("GetPermissionsAsync")]
    [ApiSearch(Name = "获取权限列表", Description = "分页获取权限列表，支持按模块筛选", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetPermissionsAsync(
        [FromQuery] int pageNum = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? module = null)
    {
        return await _permissionService.GetPermissionsAsync(new PagedRequest { PageNumber = pageNum, PageSize = pageSize }, module);
    }

    /// <summary>
    /// 获取所有启用的权限列表
    /// </summary>
    [HttpGet]
    [ActionName("GetAllEnabledPermissionsAsync")]
    [ApiSearch(Name = "获取启用权限", Description = "获取所有启用的权限列表", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetAllEnabledPermissionsAsync()
    {
        return await _permissionService.GetAllEnabledPermissionsAsync();
    }

    /// <summary>
    /// 根据模块获取权限列表
    /// </summary>
    [HttpGet]
    [ActionName("GetPermissionsByModuleAsync")]
    [ApiSearch(Name = "按模块获取权限", Description = "根据模块获取权限列表", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetPermissionsByModuleAsync([FromQuery] string module)
    {
        return await _permissionService.GetPermissionsByModuleAsync(module);
    }

    /// <summary>
    /// 获取权限详情
    /// </summary>
    [HttpGet]
    [ActionName("GetPermissionByIdAsync")]
    [ApiSearch(Name = "获取权限详情", Description = "根据ID获取权限详情", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetPermissionByIdAsync([FromQuery] Guid id)
    {
        return await _permissionService.GetPermissionByIdAsync(id);
    }

    /// <summary>
    /// 创建权限
    /// </summary>
    [HttpPost]
    [ActionName("CreatePermissionAsync")]
    [ApiSearch(Name = "创建权限", Description = "创建新的权限", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> CreatePermissionAsync([FromBody] CreatePermissionRequest request)
    {
        return await _permissionService.CreatePermissionAsync(request);
    }

    /// <summary>
    /// 更新权限
    /// </summary>
    [HttpPut]
    [ActionName("UpdatePermissionAsync")]
    [ApiSearch(Name = "更新权限", Description = "更新权限信息", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> UpdatePermissionAsync([FromBody] UpdatePermissionRequest request)
    {
        return await _permissionService.UpdatePermissionAsync(request);
    }

    /// <summary>
    /// 删除权限
    /// </summary>
    [HttpDelete]
    [ActionName("DeletePermissionAsync")]
    [ApiSearch(Name = "删除权限", Description = "删除指定权限", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> DeletePermissionAsync([FromQuery] Guid id)
    {
        return await _permissionService.DeletePermissionAsync(id);
    }

    /// <summary>
    /// 启用权限
    /// </summary>
    [HttpPost]
    [ActionName("EnablePermissionAsync")]
    [ApiSearch(Name = "启用权限", Description = "启用指定权限", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> EnablePermissionAsync([FromQuery] Guid id)
    {
        return await _permissionService.EnablePermissionAsync(id);
    }

    /// <summary>
    /// 禁用权限
    /// </summary>
    [HttpPost]
    [ActionName("DisablePermissionAsync")]
    [ApiSearch(Name = "禁用权限", Description = "禁用指定权限", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> DisablePermissionAsync([FromQuery] Guid id)
    {
        return await _permissionService.DisablePermissionAsync(id);
    }

    /// <summary>
    /// 获取角色的权限ID列表
    /// </summary>
    [HttpGet]
    [ActionName("GetRolePermissionIdsAsync")]
    [ApiSearch(Name = "获取角色权限", Description = "获取角色的权限ID列表", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetRolePermissionIdsAsync([FromQuery] Guid roleId)
    {
        return await _permissionService.GetRolePermissionIdsAsync(roleId);
    }

    /// <summary>
    /// 为角色分配权限
    /// </summary>
    [HttpPost]
    [ActionName("AssignRolePermissionsAsync")]
    [ApiSearch(Name = "分配角色权限", Description = "为角色分配权限", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> AssignRolePermissionsAsync([FromBody] AssignRolePermissionsRequest request)
    {
        return await _permissionService.AssignRolePermissionsAsync(request.RoleId, request.PermissionIds);
    }

    /// <summary>
    /// 获取用户的权限列表
    /// </summary>
    [HttpGet]
    [ActionName("GetUserPermissionsAsync")]
    [ApiSearch(Name = "获取用户权限", Description = "获取用户的权限列表（通过角色）", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetUserPermissionsAsync([FromQuery] Guid userId)
    {
        return await _permissionService.GetUserPermissionsAsync(userId);
    }

    /// <summary>
    /// 检查用户是否有指定权限
    /// </summary>
    [HttpGet]
    [ActionName("HasPermissionAsync")]
    [ApiSearch(Name = "检查用户权限", Description = "检查用户是否有指定权限", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> HasPermissionAsync([FromQuery] Guid userId, [FromQuery] string permissionCode)
    {
        return await _permissionService.HasPermissionAsync(userId, permissionCode);
    }
}