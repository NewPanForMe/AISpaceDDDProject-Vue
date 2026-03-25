using Microsoft.AspNetCore.Mvc;
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;
using Microsoft.AspNetCore.Authorization;

namespace DDDProject.API.Controllers;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class UserController : BaseApiController
{
    private readonly IUserDataService _userDataService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userDataService">用户数据服务</param>
    public UserController(IUserDataService userDataService)
    {
        _userDataService = userDataService;
    }

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    [HttpGet]
    [ActionName("GetUsersAsync")]
    [ApiSearch(Name = "获取用户列表", Description = "返回用户列表（支持分页）", Category = ApiSearchCategory.User)]
    public async Task<ApiRequestResult> GetUsersAsync([FromQuery] int pageNum = 1, [FromQuery] int pageSize = 10)
    {
        var request = new PagedRequest
        {
            PageNumber = pageNum,
            PageSize = pageSize
        };
        return await _userDataService.GetUsersAsync(request);
    }

    /// <summary>
    /// 获取用户详情
    /// </summary>
    [HttpGet]
    [ActionName("GetUserByIdAsync")]
    [ApiSearch(Name = "获取用户详情", Description = "根据ID获取用户详细信息", Category = ApiSearchCategory.User)]
    public async Task<ApiRequestResult> GetUserByIdAsync([FromQuery] Guid id)
    {
        return await _userDataService.GetUserByIdAsync(id);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    [HttpPost]
    [ActionName("CreateUserAsync")]
    [ApiSearch(Name = "创建用户", Description = "创建新的用户", Category = ApiSearchCategory.User)]
    public async Task<ApiRequestResult> CreateUserAsync([FromBody] CreateUserRequest request)
    {
        return await _userDataService.CreateUserAsync(request);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    [HttpPut]
    [ActionName("UpdateUserAsync")]
    [ApiSearch(Name = "更新用户", Description = "更新现有用户", Category = ApiSearchCategory.User)]
    public async Task<ApiRequestResult> UpdateUserAsync([FromBody] UpdateUserRequest request)
    {
        return await _userDataService.UpdateUserAsync(request);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    [HttpDelete]
    [ActionName("DeleteUserAsync")]
    [ApiSearch(Name = "删除用户", Description = "根据ID删除用户", Category = ApiSearchCategory.User)]
    public async Task<IActionResult> DeleteUserAsync([FromQuery] Guid id)
    {
        var result = await _userDataService.DeleteUserAsync(id);
        if (!result.Success)
            return BadRequest(result);
        return NoContent();
    }

    /// <summary>
    /// 启用用户
    /// </summary>
    [HttpPost]
    [ActionName("EnableUserAsync")]
    [ApiSearch(Name = "启用用户", Description = "启用一个被禁用的用户", Category = ApiSearchCategory.User)]
    public async Task<ApiRequestResult> EnableUserAsync([FromQuery] Guid id)
    {
        return await _userDataService.EnableUserAsync(id);
    }

    /// <summary>
    /// 禁用用户
    /// </summary>
    [HttpPost]
    [ActionName("DisableUserAsync")]
    [ApiSearch(Name = "禁用用户", Description = "禁用一个启用的用户", Category = ApiSearchCategory.User)]
    public async Task<ApiRequestResult> DisableUserAsync([FromQuery] Guid id)
    {
        return await _userDataService.DisableUserAsync(id);
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    [HttpPost]
    [ActionName("ResetPasswordAsync")]
    [ApiSearch(Name = "重置用户密码", Description = "重置用户的密码", Category = ApiSearchCategory.User)]
    public async Task<ApiRequestResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
    {
        return await _userDataService.ResetPasswordAsync(request);
    }
}
