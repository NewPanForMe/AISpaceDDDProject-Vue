using DDDProject.Domain.Entities;
using DDDProject.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 菜单数据播种器
/// </summary>
public static class MenuSeeder
{
    /// <summary>
    /// 初始化菜单数据
    /// </summary>
    public static void SeedMenus(this ApplicationDbContext context)
    {
        if (context.Menus.Any())
        {
            return;
        }

        var menus = GetSeedMenus();

        context.Menus.AddRange(menus);
        context.SaveChanges();
    }

    /// <summary>
    /// 异步初始化菜单数据
    /// </summary>
    public static async Task SeedMenusAsync(this ApplicationDbContext context)
    {
        if (await context.Menus.AnyAsync())
        {
            return;
        }

        var menus = GetSeedMenus();

        await context.Menus.AddRangeAsync(menus);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 获取种子菜单数据
    /// </summary>
    /// <returns>菜单列表</returns>
    private static List<Menu> GetSeedMenus()
    {
        var menus = new List<Menu>();

        // ==================== 一级菜单 ====================

        // 1. 首页（仪表盘）
        var dashboardMenu = Menu.Create(
            name: "首页",
            path: "dashboard",
            component: "Dashboard/Dashboard",
            icon: "HomeFilled",
            parentId: null,
            sortOrder: 1,
            status: 1
        );
        menus.Add(dashboardMenu);

        // 2. 用户管理
        var userMenu = Menu.Create(
            name: "用户管理",
            path: "users",
            component: "Users", // 有子菜单，不需要组件
            icon: "User",
            parentId: null,
            sortOrder: 2,
            status: 1
        );
        menus.Add(userMenu);

        // 4. 产品管理
        var productMenu = Menu.Create(
            name: "产品管理",
            path: "products",
            component: "Products",
            icon: "Goods",
            parentId: null,
            sortOrder: 4,
            status: 1
        );
        menus.Add(productMenu);

        // 5. 系统设置
        var settingsMenu = Menu.Create(
            name: "系统设置",
            path: "settings",
            component: "Settings", // 有子菜单，不需要组件
            icon: "Setting",
            parentId: null,
            sortOrder: 5,
            status: 1
        );
        menus.Add(settingsMenu);



        // 1. 日志（仪表盘）
        var logMenu = Menu.Create(
            name: "日志管理",
            path: "logs",
            component: "LogV", // 有子菜单，不需要组件
            icon: "HomeFilled",
            parentId: null,
            sortOrder: 6,
            status: 1
        );
        menus.Add(logMenu);

        // 7. 站内信
        var msgMenu = Menu.Create(
            name: "站内信",
            path: "messages",
            component: "Messages/Messages",
            icon: "Bell",
            parentId: null,
            sortOrder: 7,
            status: 1
        );
        menus.Add(msgMenu);

        // ==================== 二级菜单 ====================

        // 3. 角色管理
        var roleMenu = Menu.Create(
            name: "角色管理",
            path: "users-role",
            component: "UserRole/UserRole",
            icon: "UserFilled",
            parentId: userMenu.Id,
            sortOrder: 3,
            status: 1
        );
        menus.Add(roleMenu);
        // 3. 用户信息
        var infoMenu = Menu.Create(
            name: "用户信息",
            path: "users-info",
            component: "Users/Users",
            icon: "UserFilled",
            parentId: userMenu.Id,
            sortOrder: 3,
            status: 1
        );
        menus.Add(infoMenu);
        // 系统设置 -> 菜单管理
        var menuManagementMenu = Menu.Create(
            name: "菜单管理",
            path: "settings-menu",
            component: "menu/Menu",
            icon: "Menu",
            parentId: settingsMenu.Id,
            sortOrder: 1,
            status: 1
        );
        menus.Add(menuManagementMenu);

        // 系统设置 -> 权限管理
        var permissionMenu = Menu.Create(
            name: "权限管理",
            path: "settings-permissions",
            component: "Settings/Permissions",
            icon: "Lock",
            parentId: settingsMenu.Id,
            sortOrder: 2,
            status: 1
        );
        menus.Add(permissionMenu);


        // 系统设置 -> 系统管理
        var systemMenu = Menu.Create(
            name: "系统管理",
            path: "settings-system",
            component: "Settings/System",
            icon: "Lock",
            parentId: settingsMenu.Id,
            sortOrder: 3,
            status: 1
        );
        menus.Add(systemMenu);

        // 日志管理 -> 日志列表
        var logsMenu = Menu.Create(
            name: "日志列表",
            path: "logs-log",
            component: "LogV/Log",
            icon: "Document",
            parentId: logMenu.Id,
            sortOrder: 1,
            status: 1
        );
        menus.Add(logsMenu);



        // 系统设置 -> 字典管理
        var dicMenu = Menu.Create(
            name: "字典管理",
            path: "settings-dictionary",
            component: "Settings/Dictionary",
            icon: "Collection",
            parentId: settingsMenu.Id,
            sortOrder: 4,
            status: 1
        );
        menus.Add(dicMenu);

        return menus;
    }
}