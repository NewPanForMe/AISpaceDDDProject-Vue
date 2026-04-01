using Microsoft.AspNetCore.Mvc;
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;

namespace DDDProject.API.Controllers;

/// <summary>
/// 登录控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class LoginController : BaseApiController
{
    private readonly ILoginService _loginService;
    private readonly ISettingService _settingService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="loginService">登录服务</param>
    /// <param name="settingService">设置服务</param>
    public LoginController(ILoginService loginService, ISettingService settingService)
    {
        _loginService = loginService;
        _settingService = settingService;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录结果</returns>
    [HttpPost()]
    [ActionName("LoginAsync")]
    [ApiSearch(Name = "用户登录", Description = "用户登录验证", Category = ApiSearchCategory.Login)]
    public async Task<ApiRequestResult> LoginAsync([FromBody] LoginRequest request)
    {
        return await _loginService.LoginAsync(request);
    }

    /// <summary>
    /// 获取公开的系统设置（无需登录）
    /// </summary>
    /// <remarks>
    /// 返回系统名称、系统描述等公开配置，用于登录页等无需认证的场景
    /// </remarks>
    [HttpGet]
    [ActionName("GetPublicSettingsAsync")]
    public async Task<ApiRequestResult> GetPublicSettingsAsync()
    {
        return await _settingService.GetPublicSettingsAsync();
    }
}
