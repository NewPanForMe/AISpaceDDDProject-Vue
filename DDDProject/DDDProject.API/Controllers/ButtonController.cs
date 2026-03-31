using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DDDProject.API.Controllers;

/// <summary>
/// 按钮管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class ButtonController : BaseApiController
{
    private readonly IButtonService _buttonService;

    public ButtonController(IButtonService buttonService)
    {
        _buttonService = buttonService;
    }

    /// <summary>
    /// 获取按钮列表（分页）
    /// </summary>
    [HttpGet]
    [ActionName("GetButtonsAsync")]
    [ApiSearch(Name = "获取按钮列表", Description = "分页获取按钮列表，支持按菜单筛选", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> GetButtonsAsync(
        [FromQuery] int pageNum = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? menuId = null)
    {
        return await _buttonService.GetButtonsAsync(
            new PagedRequest { PageNumber = pageNum, PageSize = pageSize },
            menuId);
    }

    /// <summary>
    /// 根据ID获取按钮详情
    /// </summary>
    [HttpGet]
    [ActionName("GetButtonByIdAsync")]
    [ApiSearch(Name = "获取按钮详情", Description = "根据ID获取按钮详细信息", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> GetButtonByIdAsync([FromQuery] Guid id)
    {
        return await _buttonService.GetButtonByIdAsync(id);
    }

    /// <summary>
    /// 根据菜单ID获取按钮列表
    /// </summary>
    [HttpGet]
    [ActionName("GetButtonsByMenuIdAsync")]
    [ApiSearch(Name = "获取菜单按钮", Description = "根据菜单ID获取该菜单下的所有启用按钮", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> GetButtonsByMenuIdAsync([FromQuery] Guid menuId)
    {
        return await _buttonService.GetButtonsByMenuIdAsync(menuId);
    }

    /// <summary>
    /// 创建按钮
    /// </summary>
    [HttpPost]
    [ActionName("CreateButtonAsync")]
    [ApiSearch(Name = "创建按钮", Description = "创建新的按钮", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> CreateButtonAsync([FromBody] CreateButtonRequest request)
    {
        return await _buttonService.CreateButtonAsync(request);
    }

    /// <summary>
    /// 更新按钮
    /// </summary>
    [HttpPut]
    [ActionName("UpdateButtonAsync")]
    [ApiSearch(Name = "更新按钮", Description = "更新按钮信息", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> UpdateButtonAsync([FromBody] UpdateButtonRequest request)
    {
        return await _buttonService.UpdateButtonAsync(request);
    }

    /// <summary>
    /// 删除按钮
    /// </summary>
    [HttpDelete]
    [ActionName("DeleteButtonAsync")]
    [ApiSearch(Name = "删除按钮", Description = "根据ID删除按钮", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> DeleteButtonAsync([FromQuery] Guid id)
    {
        return await _buttonService.DeleteButtonAsync(id);
    }

    /// <summary>
    /// 启用按钮
    /// </summary>
    [HttpPost]
    [ActionName("EnableButtonAsync")]
    [ApiSearch(Name = "启用按钮", Description = "启用指定按钮", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> EnableButtonAsync([FromQuery] Guid id)
    {
        return await _buttonService.EnableButtonAsync(id);
    }

    /// <summary>
    /// 禁用按钮
    /// </summary>
    [HttpPost]
    [ActionName("DisableButtonAsync")]
    [ApiSearch(Name = "禁用按钮", Description = "禁用指定按钮", Category = ApiSearchCategory.Menu)]
    public async Task<ApiRequestResult> DisableButtonAsync([FromQuery] Guid id)
    {
        return await _buttonService.DisableButtonAsync(id);
    }
}
