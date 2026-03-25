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