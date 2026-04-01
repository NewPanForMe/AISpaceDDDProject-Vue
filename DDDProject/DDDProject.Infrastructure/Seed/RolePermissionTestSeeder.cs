using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DDDProject.Infrastructure.Contexts;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 测试权限角色关联种子数据 - 将测试权限分配给管理员角色
/// </summary>
public static class RolePermissionTestSeeder
{
    /// <summary>
    /// 种植测试权限角色关联数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedTestRolePermissions(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        context.Database.EnsureCreated();

        // 获取管理员角色（假设第一个角色是管理员）
        var adminRole = context.Roles.FirstOrDefault(r => r.Code == "admin" || r.Name == "管理员");
        if (adminRole == null)
        {
            // 如果没有找到管理员角色，使用第一个启用的角色
            adminRole = context.Roles.FirstOrDefault(r => r.Status == 1);
        }

        if (adminRole == null)
        {
            return;
        }

        // 获取测试权限编码列表
        var testPermissionCodes = GetTestPermissionCodes();

        // 获取测试权限的ID列表
        var testPermissionIds = context.Permissions
            .Where(p => testPermissionCodes.Contains(p.Code))
            .Select(p => p.Id)
            .ToList();

        if (testPermissionIds.Count == 0)
        {
            return;
        }

        // 获取已存在的角色权限关联
        var existingPermissionIds = context.RolePermissions
            .Where(rp => rp.RoleId == adminRole.Id && testPermissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .ToList();

        // 过滤出未关联的权限
        var newPermissionIds = testPermissionIds.Except(existingPermissionIds).ToList();

        if (newPermissionIds.Count == 0)
        {
            return;
        }

        // 创建新的角色权限关联
        var rolePermissions = newPermissionIds.Select(permissionId =>
            RolePermission.Create(adminRole.Id, permissionId)
        ).ToList();

        context.RolePermissions.AddRange(rolePermissions);
        context.SaveChanges();
    }

    /// <summary>
    /// 异步种植测试权限角色关联数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedTestRolePermissionsAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 获取管理员角色（假设第一个角色是管理员）
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "admin" || r.Name == "管理员");
        if (adminRole == null)
        {
            // 如果没有找到管理员角色，使用第一个启用的角色
            adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Status == 1);
        }

        if (adminRole == null)
        {
            return;
        }

        // 获取测试权限编码列表
        var testPermissionCodes = GetTestPermissionCodes();

        // 获取测试权限的ID列表
        var testPermissionIds = await context.Permissions
            .Where(p => testPermissionCodes.Contains(p.Code))
            .Select(p => p.Id)
            .ToListAsync();

        if (testPermissionIds.Count == 0)
        {
            return;
        }

        // 获取已存在的角色权限关联
        var existingPermissionIds = await context.RolePermissions
            .Where(rp => rp.RoleId == adminRole.Id && testPermissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        // 过滤出未关联的权限
        var newPermissionIds = testPermissionIds.Except(existingPermissionIds).ToList();

        if (newPermissionIds.Count == 0)
        {
            return;
        }

        // 创建新的角色权限关联
        var rolePermissions = newPermissionIds.Select(permissionId =>
            RolePermission.Create(adminRole.Id, permissionId)
        ).ToList();

        await context.RolePermissions.AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 获取测试权限编码列表
    /// </summary>
    /// <returns>权限编码列表</returns>
    private static List<string> GetTestPermissionCodes()
    {
        return new List<string>
        {
            // 基础测试权限
            "test:single",
            "test:read",
            "test:write",
            "test:admin",
            "test:create",
            "test:update",
            "test:delete",
            "test:super_admin",

            // 模块测试权限
            "test:module1:read",
            "test:module2:write",

            // 批量操作权限
            "test:batch:create",
            "test:batch:update",
            "test:batch:delete",

            // 报告权限
            "test:report:export",

            // 配置权限
            "test:config:read",
            "test:config:write"
        };
    }
} 
