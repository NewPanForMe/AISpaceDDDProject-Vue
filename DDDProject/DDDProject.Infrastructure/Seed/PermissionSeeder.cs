using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DDDProject.Infrastructure.Contexts;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 权限种子数据
/// </summary>
public static class PermissionSeeder
{
    /// <summary>
    /// 种植权限数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedPermissions(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        context.Database.EnsureCreated();

        // 检查是否已存在权限数据
        if (context.Permissions.Any())
        {
            return;
        }

        var permissions = GetSeedPermissions();

        context.Permissions.AddRange(permissions);
        context.SaveChanges();
    }

    /// <summary>
    /// 异步种植权限数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedPermissionsAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 检查是否已存在权限数据
        if (await context.Permissions.AnyAsync())
        {
            return;
        }

        var permissions = GetSeedPermissions();

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 获取种子权限数据
    /// </summary>
    /// <returns>权限列表</returns>
    private static List<Permission> GetSeedPermissions()
    {
        var permissions = new List<Permission>
        {
            // 菜单管理权限
            Permission.Create("menu:add", "添加菜单", "Menu", "添加新菜单", null, 1),
            Permission.Create("menu:edit", "编辑菜单", "Menu", "编辑菜单信息", null, 2),
            Permission.Create("menu:delete", "删除菜单", "Menu", "删除菜单", null, 3),
            Permission.Create("menu:add_child", "添加子菜单", "Menu", "添加子菜单", null, 4),

            // 用户管理权限
            Permission.Create("user:add", "添加用户", "User", "添加新用户", null, 1),
            Permission.Create("user:edit", "编辑用户", "User", "编辑用户信息", null, 2),
            Permission.Create("user:delete", "删除用户", "User", "删除用户", null, 3),
            Permission.Create("user:reset_password", "重置密码", "User", "重置用户密码", null, 4),
            Permission.Create("user:assign_role", "配置角色", "User", "为用户配置角色", null, 5),
            Permission.Create("user:enable", "启用用户", "User", "启用用户账号", null, 6),
            Permission.Create("user:disable", "禁用用户", "User", "禁用用户账号", null, 7),

            // 角色管理权限
            Permission.Create("role:add", "添加角色", "Role", "添加新角色", null, 1),
            Permission.Create("role:edit", "编辑角色", "Role", "编辑角色信息", null, 2),
            Permission.Create("role:delete", "删除角色", "Role", "删除角色", null, 3),
            Permission.Create("role:assign_menu", "分配模块", "Role", "为角色分配菜单模块", null, 4),
            Permission.Create("role:assign_user", "分配用户", "Role", "为角色分配用户", null, 5),
            Permission.Create("role:assign_permission", "分配权限", "Role", "为角色分配权限", null, 6),
            Permission.Create("role:enable", "启用角色", "Role", "启用角色", null, 7),
            Permission.Create("role:disable", "禁用角色", "Role", "禁用角色", null, 8),

            // 系统设置权限
            Permission.Create("setting:save_jwt", "保存JWT配置", "Setting", "保存JWT配置", null, 1),
            Permission.Create("setting:save_system", "保存系统配置", "Setting", "保存系统配置", null, 2),

            // 权限管理权限
            Permission.Create("permission:add", "添加权限", "Permission", "添加新权限", null, 1),
            Permission.Create("permission:edit", "编辑权限", "Permission", "编辑权限信息", null, 2),
            Permission.Create("permission:delete", "删除权限", "Permission", "删除权限", null, 3),
            Permission.Create("permission:enable", "启用权限", "Permission", "启用权限", null, 4),
            Permission.Create("permission:disable", "禁用权限", "Permission", "禁用权限", null, 5),

            // 缓存管理权限
            Permission.Create("cache:clear_auth", "清除认证缓存", "Cache", "清除登录认证令牌", null, 1),
            Permission.Create("cache:clear_user", "清除用户缓存", "Cache", "清除用户信息和权限数据", null, 2),
            Permission.Create("cache:clear_menu", "清除菜单缓存", "Cache", "清除菜单缓存", null, 3),
            Permission.Create("cache:clear_list", "清除列表缓存", "Cache", "清除列表缓存", null, 4),
            Permission.Create("cache:clear_setting", "清除设置缓存", "Cache", "清除系统设置缓存", null, 5),
            Permission.Create("cache:clear_all", "清除全部缓存", "Cache", "清除全部缓存", null, 6),

            // 按钮管理权限
            Permission.Create("button:add", "添加按钮", "Button", "添加新按钮", null, 1),
            Permission.Create("button:edit", "编辑按钮", "Button", "编辑按钮信息", null, 2),
            Permission.Create("button:delete", "删除按钮", "Button", "删除按钮", null, 3),
            Permission.Create("button:enable", "启用按钮", "Button", "启用按钮", null, 4),
            Permission.Create("button:disable", "禁用按钮", "Button", "禁用按钮", null, 5)
        };

        return permissions;
    }
}