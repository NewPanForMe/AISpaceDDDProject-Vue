using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DDDProject.API.Controllers;

/// <summary>
/// 基础控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class BaseApiController : ControllerBase
{
    /// <summary>
    /// 获取当前登录用户ID
    /// </summary>
    /// <returns>用户ID，如果未登录返回null</returns>
    protected Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }

    /// <summary>
    /// 获取当前登录用户名
    /// </summary>
    /// <returns>用户名，如果未登录返回null</returns>
    protected string? GetCurrentUserName()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst("unique_name")?.Value;
    }
}
