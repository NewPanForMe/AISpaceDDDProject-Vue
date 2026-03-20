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

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="loginService">登录服务</param>
    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录结果</returns>
    [HttpPost]
    [ApiSearch(Name = "用户登录", Description = "用户登录验证", Category = "认证")]
    public ApiRequestResult Login([FromBody] LoginRequest request)
    {
        return _loginService.Login(request);
    }
}
