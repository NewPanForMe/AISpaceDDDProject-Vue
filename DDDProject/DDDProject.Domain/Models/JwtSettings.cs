namespace DDDProject.Domain.Models;

/// <summary>
/// JWT配置选项
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// 发行人
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 受众
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// 密钥
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间(分钟)
    /// </summary>
    public int ExpireMinutes { get; set; } = 120;
}
