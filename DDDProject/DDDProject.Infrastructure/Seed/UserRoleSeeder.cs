using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DDDProject.Infrastructure.Contexts;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 用户角色关联种子数据
/// </summary>
public static class UserRoleSeeder
{
    /// <summary>
    /// 种植用户角色关联数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedUserRoles(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        context.Database.EnsureCreated();

        // 检查是否已存在用户角色关联数据
        if (context.UserRoles.Any())
        {
            return;
        }

        var userRoles = new List<UserRole>();

        // 获取 Admin 用户
        var adminUser = context.Users.FirstOrDefault(u => u.UserName == "Admin");
        // 获取超级管理员角色
        var superAdminRole = context.Roles.FirstOrDefault(r => r.Code == "SUPER_ADMIN");

        if (adminUser is not null && superAdminRole is not null)
        {
            // 为 Admin 用户分配超级管理员角色
            userRoles.Add(UserRole.Create(adminUser.Id, superAdminRole.Id));
        }

        // 获取普通用户 zhangsan
        var zhangsanUser = context.Users.FirstOrDefault(u => u.UserName == "zhangsan");
        // 获取普通用户角色
        var userRole = context.Roles.FirstOrDefault(r => r.Code == "USER");

        if (zhangsanUser is not null && userRole is not null)
        {
            // 为 zhangsan 分配普通用户角色
            userRoles.Add(UserRole.Create(zhangsanUser.Id, userRole.Id));
        }

        // 获取普通用户 lisi
        var lisiUser = context.Users.FirstOrDefault(u => u.UserName == "lisi");
        // 获取管理员角色
        var adminRole = context.Roles.FirstOrDefault(r => r.Code == "ADMIN");

        if (lisiUser is not null && adminRole is not null)
        {
            // 为 lisi 分配管理员角色
            userRoles.Add(UserRole.Create(lisiUser.Id, adminRole.Id));
        }

        if (userRoles.Any())
        {
            context.UserRoles.AddRange(userRoles);
            context.SaveChanges();
        }
    }

    /// <summary>
    /// 异步种植用户角色关联数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedUserRolesAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 检查是否已存在用户角色关联数据
        if (await context.UserRoles.AnyAsync())
        {
            return;
        }

        var userRoles = new List<UserRole>();

        // 获取 Admin 用户
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "Admin");
        // 获取超级管理员角色
        var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "SUPER_ADMIN");

        if (adminUser is not null && superAdminRole is not null)
        {
            // 为 Admin 用户分配超级管理员角色
            userRoles.Add(UserRole.Create(adminUser.Id, superAdminRole.Id));
        }

        // 获取普通用户 zhangsan
        var zhangsanUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "zhangsan");
        // 获取普通用户角色
        var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "USER");

        if (zhangsanUser is not null && userRole is not null)
        {
            // 为 zhangsan 分配普通用户角色
            userRoles.Add(UserRole.Create(zhangsanUser.Id, userRole.Id));
        }

        // 获取普通用户 lisi
        var lisiUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "lisi");
        // 获取管理员角色
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "ADMIN");

        if (lisiUser is not null && adminRole is not null)
        {
            // 为 lisi 分配管理员角色
            userRoles.Add(UserRole.Create(lisiUser.Id, adminRole.Id));
        }

        if (userRoles.Any())
        {
            await context.UserRoles.AddRangeAsync(userRoles);
            await context.SaveChangesAsync();
        }
    }
}