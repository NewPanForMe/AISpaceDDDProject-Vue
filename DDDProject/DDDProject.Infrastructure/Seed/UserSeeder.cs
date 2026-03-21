using DDDProject.Domain.Entities;
using DDDProject.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using DDDProject.Infrastructure.Contexts;

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
    public static void SeedUsers(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        context.Database.EnsureCreated();

        // 检查是否已存在 Admin 用户
        var adminUser = context.Users.FirstOrDefault(u => u.UserName == "Admin");
        if (adminUser is null)
        {
            // 使用PasswordHelper计算密码哈希（密码为 admin123）
            var hashedPassword = PasswordHelper.ComputeHash("admin123");
            
            var admin = User.Create(
                userName: "Admin",
                email: "admin@aispace.com",
                passwordHash: hashedPassword,
                realName: "系统管理员"
            );
            
            // 设置初始状态和其他属性
            admin.Enable(); // 确保用户处于启用状态

            context.Users.Add(admin);
            context.SaveChanges();
        }
    }

    /// <summary>
    /// 异步种植用户数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedUsersAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 检查是否已存在 Admin 用户
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "Admin");
        if (adminUser is null)
        {
            // 使用PasswordHelper计算密码哈希（密码为 admin123）
            var hashedPassword = PasswordHelper.ComputeHash("admin123");
            
            var admin = User.Create(
                userName: "Admin",
                email: "admin@aispace.com",
                passwordHash: hashedPassword,
                realName: "系统管理员"
            );
            
            // 设置初始状态和其他属性
            admin.Enable(); // 确保用户处于启用状态

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
