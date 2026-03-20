using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 用户种子数据
/// </summary>
public static class UserSeeder
{
    /// <summary>
    /// 种植用户数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedUsers(this DbContext context)
    {
        // 检查是否已存在 Admin 用户
        var adminUser = context.Set<User>().FirstOrDefault(u => u.UserName == "Admin");
        if (adminUser is null)
        {
            // 创建 Admin 用户，密码为 12345
            var admin = User.Create(
                userName: "Admin",
                email: "admin@aispace.com",
                passwordHash: ComputePasswordHash("12345"),
                realName: "管理员"
            );

            context.Set<User>().Add(admin);
        }
    }

    /// <summary>
    /// 计算密码哈希（SHA256）
    /// </summary>
    private static string ComputePasswordHash(string password)
    {
        
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return System.BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
