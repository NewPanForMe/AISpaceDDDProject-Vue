using DDDProject.Domain.Entities;
using DDDProject.Infrastructure.Contexts;

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
            // 如果菜单已存在，则跳过初始化
            return;
        }

        // 添加默认菜单项
        var menus = new List<Menu>
        {
            Menu.Create("首页", "/dashboard", "views/home/Dashboard/index.vue", "HomeOutlined", null, 1, 1),
            Menu.Create("用户管理", "/users", "", "UserOutlined", null, 2, 1),
            Menu.Create("产品管理", "/products", "", "AppstoreOutlined", null, 3, 1),
            Menu.Create("系统设置", "/settings", "", "SettingOutlined", null, 4, 1)
        };
        
        // 添加子菜单 - 用户管理
        var userManagementMenu = menus.First(m => m.Name == "用户管理");
        menus.Add(Menu.Create("用户列表", "/users/list", "views/home/Users/index.vue", "TeamOutlined", userManagementMenu.Id, 1, 1));
        menus.Add(Menu.Create("角色管理", "/users/roles", "views/home/Users/Roles.vue", "SafetyCertificateOutlined", userManagementMenu.Id, 2, 1));
        
        // 添加子菜单 - 产品管理
        var productManagementMenu = menus.First(m => m.Name == "产品管理");
        menus.Add(Menu.Create("产品列表", "/products/list", "views/home/Products/index.vue", "ShoppingOutlined", productManagementMenu.Id, 1, 1));
        menus.Add(Menu.Create("分类管理", "/products/categories", "views/home/Products/Categories.vue", "TagOutlined", productManagementMenu.Id, 2, 1));

        // 添加子菜单 - 系统设置
        var systemSettingsMenu = menus.First(m => m.Name == "系统设置");
        menus.Add(Menu.Create("菜单管理", "/settings/menus", "views/home/menu/Menu.vue", "MenuOutlined", systemSettingsMenu.Id, 1, 1));
        menus.Add(Menu.Create("权限管理", "/settings/permissions", "views/home/Settings/Permissions.vue", "LockOutlined", systemSettingsMenu.Id, 2, 1));
        
        foreach (var menu in menus)
        {
            context.Menus.Add(menu);
        }
        
        context.SaveChanges();
    }
}