using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;
using DDDProject.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DDDProject.API.Controllers;

/// <summary>
/// 菜单管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize] // 要求身份验证
public class MenuController : BaseApiController
{
    private readonly IMenuService _menuService;
    private readonly CurrentUser _currentUser;

    public MenuController(IMenuService menuService, CurrentUser currentUser)
    {
        _menuService = menuService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// 获取菜单列表
    /// </summary>
    [HttpGet]
    [ApiSearch(Name = "获取菜单列表", Description = "返回系统中所有的菜单项", Category = "菜单管理")]
    public async Task<ApiRequestResult> GetMenusAsync()
    {
        // 可以根据当前用户过滤菜单
        return await _menuService.GetMenusAsync();
    }

    /// <summary>
    /// 获取菜单详情
    /// </summary>
    [HttpGet]
    [ActionName("GetMenuByIdAsync")]
    [ApiSearch(Name = "获取菜单详情", Description = "根据ID获取特定菜单的详细信息", Category = "菜单管理")]
    public async Task<ApiRequestResult> GetMenuByIdAsync([FromQuery] Guid id)
    {
        return await _menuService.GetMenuByIdAsync(id);
    }

    /// <summary>
    /// 获取用户特定菜单树形结构
    /// </summary>
    [HttpGet]
    [ActionName("GetUserMenuTreeAsync")]
    [ApiSearch(Name = "获取用户菜单树", Description = "返回当前用户的菜单树形结构", Category = "菜单管理")]
    public async Task<ApiRequestResult> GetUserMenuTreeAsync()
    {
        // 可以根据当前用户获取特定的菜单
        var userId = _currentUser.UserId;
        var userName = _currentUser.UserName;
        var realName = _currentUser.RealName;
        
        // 这里假设IMenuService有一个方法来获取用户特定的菜单
        // 如果MenuService没有这个方法，我们稍后会对其进行扩展
        return await _menuService.GetUserMenuTreeAsync(userId);
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    [HttpPost]
    [ActionName("CreateMenuAsync")]
    [ApiSearch(Name = "创建菜单", Description = "在系统中创建新的菜单项", Category = "菜单管理")]
    public async Task<ApiRequestResult> CreateMenuAsync([FromBody] MenuDto menuDto)
    {
        // 可以验证当前用户是否有创建菜单的权限
        return await _menuService.CreateMenuAsync(menuDto);
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    [HttpPut]
    [ActionName("UpdateMenuAsync")]
    [ApiSearch(Name = "更新菜单", Description = "更新系统中现有的菜单项", Category = "菜单管理")]
    public async Task<ApiRequestResult> UpdateMenuAsync([FromBody] MenuDto menuDto)
    {
        // 可以验证当前用户是否有更新菜单的权限
        return await _menuService.UpdateMenuAsync(menuDto);
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    [HttpDelete]
    [ActionName("DeleteMenuAsync")]
    [ApiSearch(Name = "删除菜单", Description = "根据ID删除系统中的菜单项", Category = "菜单管理")]
    public async Task<ApiRequestResult> DeleteMenuAsync([FromQuery] Guid id)
    {
        // 可以验证当前用户是否有删除菜单的权限
        return await _menuService.DeleteMenuAsync(id);
    }

    /// <summary>
    /// 启用菜单
    /// </summary>
    [HttpPost]
    [ActionName("EnableMenuAsync")]
    [ApiSearch(Name = "启用菜单", Description = "启用一个被禁用的菜单项", Category = "菜单管理")]
    public async Task<ApiRequestResult> EnableMenuAsync([FromQuery] Guid id)
    {
        return await _menuService.EnableMenuAsync(id);
    }

    /// <summary>
    /// 禁用菜单
    /// </summary>
    [HttpPost]
    [ActionName("DisableMenuAsync")]
    [ApiSearch(Name = "禁用菜单", Description = "禁用一个启用的菜单项", Category = "菜单管理")]
    public async Task<ApiRequestResult> DisableMenuAsync([FromQuery] Guid id)
    {
        return await _menuService.DisableMenuAsync(id);
    }
}