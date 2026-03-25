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

        // 检查是否已存在用户数据
        if (context.Users.Any())
        {
            return;
        }

        var users = GetSeedUsers();

        context.Users.AddRange(users);
        context.SaveChanges();
    }

    /// <summary>
    /// 异步种植用户数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedUsersAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 检查是否已存在用户数据
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var users = GetSeedUsers();

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 获取种子用户数据
    /// </summary>
    /// <returns>用户列表</returns>
    private static List<User> GetSeedUsers()
    {
        var users = new List<User>();

        // 1. 系统管理员
        var admin = User.Create(
            userName: "Admin",
            email: "admin@aispace.com",
            passwordHash: PasswordHelper.ComputeHash("admin123"),
            realName: "系统管理员"
        );
        admin.Enable();
        users.Add(admin);

        // 2. 普通用户
        var user1 = User.Create(
            userName: "zhangsan",
            email: "zhangsan@aispace.com",
            passwordHash: PasswordHelper.ComputeHash("user123"),
            realName: "张三"
        );
        user1.Update(phoneNumber: "13800138001", remark: "测试用户1");
        user1.Enable();
        users.Add(user1);

        // 3. 普通用户2
        var user2 = User.Create(
            userName: "lisi",
            email: "lisi@aispace.com",
            passwordHash: PasswordHelper.ComputeHash("user123"),
            realName: "李四"
        );
        user2.Update(phoneNumber: "13800138002", remark: "测试用户2");
        user2.Enable();
        users.Add(user2);

        // 4. 禁用用户（用于测试禁用功能）
        var disabledUser = User.Create(
            userName: "wangwu",
            email: "wangwu@aispace.com",
            passwordHash: PasswordHelper.ComputeHash("user123"),
            realName: "王五"
        );
        disabledUser.Update(phoneNumber: "13800138003", remark: "已禁用用户");
        disabledUser.Disable();
        users.Add(disabledUser);

        return users;
    }
}