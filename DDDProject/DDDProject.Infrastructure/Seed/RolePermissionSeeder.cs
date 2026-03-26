using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DDDProject.Infrastructure.Contexts;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 角色权限关联种子数据
/// </summary>
public static class RolePermissionSeeder
{
    /// <summary>
    /// 种植角色权限关联数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedRolePermissions(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        context.Database.EnsureCreated();

        // 检查是否已存在角色权限关联数据
        if (context.RolePermissions.Any())
        {
            return;
        }

        var rolePermissions = new List<RolePermission>();

        // 获取超级管理员角色
        var superAdminRole = context.Roles.FirstOrDefault(r => r.Code == "SUPER_ADMIN");
        if (superAdminRole is not null)
        {
            // 为超级管理员分配所有权限
            var allPermissions = context.Permissions.ToList();
            foreach (var permission in allPermissions)
            {
                rolePermissions.Add(RolePermission.Create(superAdminRole.Id, permission.Id));
            }
        }

        // 获取管理员角色
        var adminRole = context.Roles.FirstOrDefault(r => r.Code == "ADMIN");
        if (adminRole is not null)
        {
            // 为管理员分配大部分权限（除了权限管理相关）
            var adminPermissionCodes = new[]
            {
                // 菜单管理
                "menu:add", "menu:edit", "menu:delete", "menu:add_child",
                // 用户管理
                "user:add", "user:edit", "user:delete", "user:reset_password", "user:assign_role", "user:enable", "user:disable",
                // 角色管理
                "role:add", "role:edit", "role:assign_menu", "role:assign_user", "role:enable", "role:disable",
                // 系统设置
                "setting:save_jwt", "setting:save_system",
                // 缓存管理
                "cache:clear_auth", "cache:clear_user", "cache:clear_menu", "cache:clear_list", "cache:clear_setting", "cache:clear_all"
            };

            var adminPermissions = context.Permissions
                .Where(p => adminPermissionCodes.Contains(p.Code))
                .ToList();

            foreach (var permission in adminPermissions)
            {
                rolePermissions.Add(RolePermission.Create(adminRole.Id, permission.Id));
            }
        }

        // 获取普通用户角色
        var userRole = context.Roles.FirstOrDefault(r => r.Code == "USER");
        if (userRole is not null)
        {
            // 为普通用户分配基本权限
            var userPermissionCodes = new[]
            {
                // 查看相关（没有删除权限）
                "menu:add_child",
                // 用户管理（有限）
                "user:edit",
                // 缓存管理
                "cache:clear_auth", "cache:clear_menu", "cache:clear_list"
            };

            var userPermissions = context.Permissions
                .Where(p => userPermissionCodes.Contains(p.Code))
                .ToList();

            foreach (var permission in userPermissions)
            {
                rolePermissions.Add(RolePermission.Create(userRole.Id, permission.Id));
            }
        }

        if (rolePermissions.Any())
        {
            context.RolePermissions.AddRange(rolePermissions);
            context.SaveChanges();
        }
    }

    /// <summary>
    /// 异步种植角色权限关联数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedRolePermissionsAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 检查是否已存在角色权限关联数据
        if (await context.RolePermissions.AnyAsync())
        {
            return;
        }

        var rolePermissions = new List<RolePermission>();

        // 获取超级管理员角色
        var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "SUPER_ADMIN");
        if (superAdminRole is not null)
        {
            // 为超级管理员分配所有权限
            var allPermissions = await context.Permissions.ToListAsync();
            foreach (var permission in allPermissions)
            {
                rolePermissions.Add(RolePermission.Create(superAdminRole.Id, permission.Id));
            }
        }

        // 获取管理员角色
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "ADMIN");
        if (adminRole is not null)
        {
            // 为管理员分配大部分权限（除了权限管理相关）
            var adminPermissionCodes = new[]
            {
                // 菜单管理
                "menu:add", "menu:edit", "menu:delete", "menu:add_child",
                // 用户管理
                "user:add", "user:edit", "user:delete", "user:reset_password", "user:assign_role", "user:enable", "user:disable",
                // 角色管理
                "role:add", "role:edit", "role:assign_menu", "role:assign_user", "role:enable", "role:disable",
                // 系统设置
                "setting:save_jwt", "setting:save_system",
                // 缓存管理
                "cache:clear_auth", "cache:clear_user", "cache:clear_menu", "cache:clear_list", "cache:clear_setting", "cache:clear_all"
            };

            var adminPermissions = await context.Permissions
                .Where(p => adminPermissionCodes.Contains(p.Code))
                .ToListAsync();

            foreach (var permission in adminPermissions)
            {
                rolePermissions.Add(RolePermission.Create(adminRole.Id, permission.Id));
            }
        }

        // 获取普通用户角色
        var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "USER");
        if (userRole is not null)
        {
            // 为普通用户分配基本权限
            var userPermissionCodes = new[]
            {
                // 查看相关（没有删除权限）
                "menu:add_child",
                // 用户管理（有限）
                "user:edit",
                // 缓存管理
                "cache:clear_auth", "cache:clear_menu", "cache:clear_list"
            };

            var userPermissions = await context.Permissions
                .Where(p => userPermissionCodes.Contains(p.Code))
                .ToListAsync();

            foreach (var permission in userPermissions)
            {
                rolePermissions.Add(RolePermission.Create(userRole.Id, permission.Id));
            }
        }

        if (rolePermissions.Any())
        {
            await context.RolePermissions.AddRangeAsync(rolePermissions);
            await context.SaveChangesAsync();
        }
    }
}