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

        // 添加默认菜单项（单层路由）
        var menus = new List<Menu>
        {
            Menu.Create("首页", "/dashboard", "views/home/Dashboard/index.vue", "HomeOutlined", null, 1, 1),
            Menu.Create("用户管理", "/users", "views/home/Users/index.vue", "UserOutlined", null, 2, 1),
            Menu.Create("用户列表", "/users/list", "views/home/Users/List.vue", "TeamOutlined", null, 3, 1),
            Menu.Create("角色管理", "/users/roles", "views/home/Users/Roles.vue", "SafetyCertificateOutlined", null, 4, 1),
            Menu.Create("产品管理", "/products", "views/home/Products/index.vue", "AppstoreOutlined", null, 5, 1),
            Menu.Create("产品列表", "/products/list", "views/home/Products/List.vue", "ShoppingOutlined", null, 6, 1),
            Menu.Create("分类管理", "/products/categories", "views/home/Products/Categories.vue", "TagOutlined", null, 7, 1),
            Menu.Create("系统设置", "/settings", "", "SettingOutlined", null, 8, 1),
            Menu.Create("菜单管理", "/settings/menus", "views/home/menu/Menu.vue", "MenuOutlined", null, 9, 1),
            Menu.Create("权限管理", "/settings/permissions", "views/home/Settings/Permissions.vue", "LockOutlined", null, 10, 1)
        };
        
        foreach (var menu in menus)
        {
            context.Menus.Add(menu);
        }
        
        context.SaveChanges();
    }
}