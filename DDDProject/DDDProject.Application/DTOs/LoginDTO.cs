namespace DDDProject.Application.DTOs;

/// <summary>
/// 登录请求 DTO
/// </summary>
public class LoginRequest : DTO
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 登录响应 DTO
/// </summary>
public class LoginResponse : DTO
{
    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 用户编号
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 用户姓名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; set; }
}
