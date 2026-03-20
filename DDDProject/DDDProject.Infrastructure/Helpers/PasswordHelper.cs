using System.Security.Cryptography;
using System.Text;

namespace DDDProject.Infrastructure.Helpers;

/// <summary>
/// 密码帮助类
/// </summary>
public static class PasswordHelper
{
    /// <summary>
    /// 计算密码哈希（SHA256）
    /// </summary>
    /// <param name="password">密码</param>
    /// <returns>哈希后的密码</returns>
    public static string ComputeHash(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
