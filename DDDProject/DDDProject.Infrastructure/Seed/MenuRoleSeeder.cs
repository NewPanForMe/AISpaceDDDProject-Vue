using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DDDProject.Infrastructure.Contexts;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 菜单角色关联种子数据
/// </summary>
public static class MenuRoleSeeder
{
    /// <summary>
    /// 种植菜单角色关联数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedMenuRoles(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        context.Database.EnsureCreated();

        // 检查是否已存在菜单角色关联数据
        if (context.MenuRoles.Any())
        {
            return;
        }

        var menuRoles = new List<MenuRole>();

        // 获取所有菜单
        var allMenus = context.Menus.ToList();

        // 获取超级管理员角色
        var superAdminRole = context.Roles.FirstOrDefault(r => r.Code == "SUPER_ADMIN");
        if (superAdminRole is not null)
        {
            // 为超级管理员分配所有菜单
            foreach (var menu in allMenus)
            {
                menuRoles.Add(MenuRole.Create(menu.Id, superAdminRole.Id));
            }
        }

        // 获取管理员角色
        var adminRole = context.Roles.FirstOrDefault(r => r.Code == "ADMIN");
        if (adminRole is not null)
        {
            // 为管理员分配大部分菜单（除了特殊的系统管理菜单）
            var adminMenuPaths = new[]
            {
                "dashboard", "users", "users-role", "users-info", "products",
                "settings", "settings-menu", "settings-permissions"
            };

            var adminMenus = allMenus.Where(m => adminMenuPaths.Contains(m.Path)).ToList();
            foreach (var menu in adminMenus)
            {
                menuRoles.Add(MenuRole.Create(menu.Id, adminRole.Id));
            }
        }

        // 获取普通用户角色
        var userRole = context.Roles.FirstOrDefault(r => r.Code == "USER");
        if (userRole is not null)
        {
            // 为普通用户分配基本菜单
            var userMenuPaths = new[]
            {
                "dashboard", "users", "users-info", "products"
            };

            var userMenus = allMenus.Where(m => userMenuPaths.Contains(m.Path)).ToList();
            foreach (var menu in userMenus)
            {
                menuRoles.Add(MenuRole.Create(menu.Id, userRole.Id));
            }
        }

        // 获取访客角色
        var guestRole = context.Roles.FirstOrDefault(r => r.Code == "GUEST");
        if (guestRole is not null)
        {
            // 为访客分配最基本菜单
            var guestMenuPaths = new[] { "dashboard" };

            var guestMenus = allMenus.Where(m => guestMenuPaths.Contains(m.Path)).ToList();
            foreach (var menu in guestMenus)
            {
                menuRoles.Add(MenuRole.Create(menu.Id, guestRole.Id));
            }
        }

        if (menuRoles.Any())
        {
            context.MenuRoles.AddRange(menuRoles);
            context.SaveChanges();
        }
    }

    /// <summary>
    /// 异步种植菜单角色关联数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedMenuRolesAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 检查是否已存在菜单角色关联数据
        if (await context.MenuRoles.AnyAsync())
        {
            return;
        }

        var menuRoles = new List<MenuRole>();

        // 获取所有菜单
        var allMenus = await context.Menus.ToListAsync();

        // 获取超级管理员角色
        var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "SUPER_ADMIN");
        if (superAdminRole is not null)
        {
            // 为超级管理员分配所有菜单
            foreach (var menu in allMenus)
            {
                menuRoles.Add(MenuRole.Create(menu.Id, superAdminRole.Id));
            }
        }

        // 获取管理员角色
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "ADMIN");
        if (adminRole is not null)
        {
            // 为管理员分配大部分菜单（除了特殊的系统管理菜单）
            var adminMenuPaths = new[]
            {
                "dashboard", "users", "users-role", "users-info", "products",
                "settings", "settings-menu", "settings-permissions"
            };

            var adminMenus = allMenus.Where(m => adminMenuPaths.Contains(m.Path)).ToList();
            foreach (var menu in adminMenus)
            {
                menuRoles.Add(MenuRole.Create(menu.Id, adminRole.Id));
            }
        }

        // 获取普通用户角色
        var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "USER");
        if (userRole is not null)
        {
            // 为普通用户分配基本菜单
            var userMenuPaths = new[]
            {
                "dashboard", "users", "users-info", "products"
            };

            var userMenus = allMenus.Where(m => userMenuPaths.Contains(m.Path)).ToList();
            foreach (var menu in userMenus)
            {
                menuRoles.Add(MenuRole.Create(menu.Id, userRole.Id));
            }
        }

        // 获取访客角色
        var guestRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "GUEST");
        if (guestRole is not null)
        {
            // 为访客分配最基本菜单
            var guestMenuPaths = new[] { "dashboard" };

            var guestMenus = allMenus.Where(m => guestMenuPaths.Contains(m.Path)).ToList();
            foreach (var menu in guestMenus)
            {
                menuRoles.Add(MenuRole.Create(menu.Id, guestRole.Id));
            }
        }

        if (menuRoles.Any())
        {
            await context.MenuRoles.AddRangeAsync(menuRoles);
            await context.SaveChangesAsync();
        }
    }
}