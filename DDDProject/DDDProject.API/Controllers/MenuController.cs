using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;
using DDDProject.Domain.Models;
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

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// 获取树形结构的菜单（用于侧边栏菜单，根据用户角色权限过滤）
    /// </summary>
    [HttpGet]
    [ActionName("GetSidebarMenusAsync")]
    [ApiSearch(Name = "获取侧边栏菜单", Description = "返回树形结构的菜单数据，根据用户角色权限过滤，用于侧边栏菜单显示", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> GetSidebarMenusAsync()
    {
        // MenuService.GetSidebarMenusAsync 已实现根据当前用户角色获取菜单的逻辑
        return await _menuService.GetSidebarMenusAsync();
    }


    /// <summary>
    /// 获取路由配置（用于前端动态路由）
    /// </summary>
    [HttpGet]
    [ActionName("GetRoutesAsync")]
    [ApiSearch(Name = "获取路由配置", Description = "返回路由配置列表，用于前端动态路由", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> GetRoutesAsync()
    {
        return await _menuService.GetRoutesAsync();
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    [HttpPost]
    [ActionName("CreateMenuAsync")]
    [Permission("menu:add")]
    [ApiSearch(Name = "创建菜单", Description = "在系统中创建新的菜单项", Category = ApiSearchCategory.Menu)]
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
    [Permission("menu:edit")]
    [ApiSearch(Name = "更新菜单", Description = "更新系统中现有的菜单项", Category = ApiSearchCategory.Menu)]
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
    [Permission("menu:delete")]
    [ApiSearch(Name = "删除菜单", Description = "根据ID删除系统中的菜单项", Category = ApiSearchCategory.Menu)]
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
    [Permission("menu:enable")]
    [ApiSearch(Name = "启用菜单", Description = "启用一个被禁用的菜单项", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> EnableMenuAsync([FromQuery] Guid id)
    {
        return await _menuService.EnableMenuAsync(id);
    }

    /// <summary>
    /// 禁用菜单
    /// </summary>
    [HttpPost]
    [ActionName("DisableMenuAsync")]
    [Permission("menu:disable")]
    [ApiSearch(Name = "禁用菜单", Description = "禁用一个启用的菜单项", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> DisableMenuAsync([FromQuery] Guid id)
    {
        return await _menuService.DisableMenuAsync(id);
    }


    /// <summary>
    /// 获取分页的树形菜单数据（用于大数据量场景）
    /// </summary>
    [HttpGet]
    [ActionName("GetPagedTreeMenusAsync")]
    [ApiSearch(Name = "获取分页树形菜单", Description = "分页获取树形菜单数据，适用于大数据量场景", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> GetPagedTreeMenusAsync([FromQuery] int pageNum = 1, [FromQuery] int pageSize = 1000)
    {
        var request = new PagedRequest
        {
            PageNumber = pageNum,
            PageSize = pageSize
        };
        return await _menuService.GetPagedTreeMenusAsync(request);
    }
}