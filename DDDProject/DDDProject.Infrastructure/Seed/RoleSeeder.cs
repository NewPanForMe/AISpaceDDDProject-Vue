using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DDDProject.Infrastructure.Contexts;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 角色种子数据
/// </summary>
public static class RoleSeeder
{
    /// <summary>
    /// 种植角色数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedRoles(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        context.Database.EnsureCreated();

        // 检查是否已存在角色数据
        if (context.Roles.Any())
        {
            return;
        }

        var roles = GetSeedRoles();

        context.Roles.AddRange(roles);
        context.SaveChanges();
    }

    /// <summary>
    /// 异步种植角色数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedRolesAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 检查是否已存在角色数据
        if (await context.Roles.AnyAsync())
        {
            return;
        }

        var roles = GetSeedRoles();

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 获取种子角色数据
    /// </summary>
    /// <returns>角色列表</returns>
    private static List<Role> GetSeedRoles()
    {
        var roles = new List<Role>();

        // 1. 超级管理员角色
        var superAdmin = Role.Create(
            name: "超级管理员",
            code: "SUPER_ADMIN",
            description: "系统超级管理员，拥有所有权限",
            sortOrder: 1
        );
        roles.Add(superAdmin);

        // 2. 管理员角色
        var admin = Role.Create(
            name: "管理员",
            code: "ADMIN",
            description: "系统管理员，拥有大部分管理权限",
            sortOrder: 2
        );
        roles.Add(admin);

        // 3. 普通用户角色
        var user = Role.Create(
            name: "普通用户",
            code: "USER",
            description: "普通用户，拥有基本功能权限",
            sortOrder: 3
        );
        roles.Add(user);

        // 4. 访客角色
        var guest = Role.Create(
            name: "访客",
            code: "GUEST",
            description: "访客用户，只有查看权限",
            sortOrder: 4
        );
        roles.Add(guest);

        // 5. 禁用角色（用于测试）
        var disabledRole = Role.Create(
            name: "已禁用角色",
            code: "DISABLED",
            description: "测试禁用状态的角色",
            sortOrder: 99
        );
        disabledRole.Disable();
        roles.Add(disabledRole);

        return roles;
    }
}