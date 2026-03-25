using Microsoft.AspNetCore.Mvc;
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;
using Microsoft.AspNetCore.Authorization;

namespace DDDProject.API.Controllers;

/// <summary>
/// 菜单角色管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class MenuRoleController : BaseApiController
{
    private readonly IMenuRoleService _menuRoleService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="menuRoleService">菜单角色服务</param>
    public MenuRoleController(IMenuRoleService menuRoleService)
    {
        _menuRoleService = menuRoleService;
    }

    /// <summary>
    /// 获取角色的菜单ID列表
    /// </summary>
    [HttpGet]
    [ActionName("GetRoleMenuIdsAsync")]
    [ApiSearch(Name = "获取角色菜单", Description = "获取指定角色的菜单ID列表", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetRoleMenuIdsAsync([FromQuery] Guid roleId)
    {
        return await _menuRoleService.GetRoleMenuIdsAsync(roleId);
    }

    /// <summary>
    /// 获取菜单的角色ID列表
    /// </summary>
    [HttpGet]
    [ActionName("GetMenuRoleIdsAsync")]
    [ApiSearch(Name = "获取菜单角色", Description = "获取指定菜单的角色ID列表", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> GetMenuRoleIdsAsync([FromQuery] Guid menuId)
    {
        return await _menuRoleService.GetMenuRoleIdsAsync(menuId);
    }

    /// <summary>
    /// 为角色分配菜单
    /// </summary>
    [HttpPost]
    [ActionName("AssignRoleMenusAsync")]
    [ApiSearch(Name = "分配角色菜单", Description = "为指定角色分配菜单权限", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> AssignRoleMenusAsync([FromBody] AssignRoleMenusRequest request)
    {
        return await _menuRoleService.AssignRoleMenusAsync(request.RoleId, request.MenuIds);
    }

    /// <summary>
    /// 为菜单分配角色
    /// </summary>
    [HttpPost]
    [ActionName("AssignMenuRolesAsync")]
    [ApiSearch(Name = "分配菜单角色", Description = "为指定菜单分配角色", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> AssignMenuRolesAsync([FromBody] AssignMenuRolesRequest request)
    {
        return await _menuRoleService.AssignMenuRolesAsync(request.MenuId, request.RoleIds);
    }

    /// <summary>
    /// 获取角色的菜单列表（树形结构）
    /// </summary>
    [HttpGet]
    [ActionName("GetRoleMenusAsync")]
    [ApiSearch(Name = "获取角色菜单树", Description = "获取指定角色的菜单列表（树形结构）", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> GetRoleMenusAsync([FromQuery] Guid roleId)
    {
        return await _menuRoleService.GetRoleMenusAsync(roleId);
    }

    /// <summary>
    /// 获取用户的所有菜单（通过角色关联）
    /// </summary>
    [HttpGet]
    [ActionName("GetUserMenusByRolesAsync")]
    [ApiSearch(Name = "获取用户菜单", Description = "获取用户通过角色关联的所有菜单", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> GetUserMenusByRolesAsync([FromQuery] Guid userId)
    {
        return await _menuRoleService.GetUserMenusByRolesAsync(userId);
    }

    /// <summary>
    /// 清除角色的所有菜单权限
    /// </summary>
    [HttpDelete]
    [ActionName("ClearRoleMenusAsync")]
    [ApiSearch(Name = "清除角色菜单", Description = "清除指定角色的所有菜单权限", Category = ApiSearchCategory.Role)]
    public async Task<ApiRequestResult> ClearRoleMenusAsync([FromQuery] Guid roleId)
    {
        return await _menuRoleService.ClearRoleMenusAsync(roleId);
    }

    /// <summary>
    /// 清除菜单的所有角色关联
    /// </summary>
    [HttpDelete]
    [ActionName("ClearMenuRolesAsync")]
    [ApiSearch(Name = "清除菜单角色", Description = "清除指定菜单的所有角色关联", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> ClearMenuRolesAsync([FromQuery] Guid menuId)
    {
        return await _menuRoleService.ClearMenuRolesAsync(menuId);
    }
}