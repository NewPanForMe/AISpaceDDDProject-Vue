using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DDDProject.Infrastructure.Contexts;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 按钮种子数据
/// </summary>
public static class ButtonSeeder
{
    /// <summary>
    /// 种植按钮数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedButtons(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        context.Database.EnsureCreated();

        // 检查是否已存在按钮数据
        if (context.Buttons.Any())
        {
            return;
        }

        var buttons = GetSeedButtons(context);

        context.Buttons.AddRange(buttons);
        context.SaveChanges();
    }

    /// <summary>
    /// 异步种植按钮数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedButtonsAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 检查是否已存在按钮数据
        if (await context.Buttons.AnyAsync())
        {
            return;
        }

        var buttons = await GetSeedButtonsAsync(context);

        await context.Buttons.AddRangeAsync(buttons);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 获取种子按钮数据（同步版本）
    /// </summary>
    /// <param name="context">数据库上下文</param>
    /// <returns>按钮列表</returns>
    private static List<Button> GetSeedButtons(ApplicationDbContext context)
    {
        var buttons = new List<Button>();

        // 获取所有菜单
        var menus = context.Menus.ToList();

        // 根据菜单路径生成按钮
        foreach (var menu in menus)
        {
            // 跳过没有组件的菜单（父级菜单）
            if (string.IsNullOrEmpty(menu.Component))
            {
                continue;
            }

            // 根据菜单路径生成按钮
            var menuButtons = GenerateButtonsForMenu(menu);
            buttons.AddRange(menuButtons);
        }

        return buttons;
    }

    /// <summary>
    /// 获取种子按钮数据（异步版本）
    /// </summary>
    /// <param name="context">数据库上下文</param>
    /// <returns>按钮列表</returns>
    private static async Task<List<Button>> GetSeedButtonsAsync(ApplicationDbContext context)
    {
        var buttons = new List<Button>();

        // 获取所有菜单
        var menus = await context.Menus.ToListAsync();

        // 根据菜单路径生成按钮
        foreach (var menu in menus)
        {
            // 跳过没有组件的菜单（父级菜单）
            if (string.IsNullOrEmpty(menu.Component))
            {
                continue;
            }

            // 根据菜单路径生成按钮
            var menuButtons = GenerateButtonsForMenu(menu);
            buttons.AddRange(menuButtons);
        }

        return buttons;
    }

    /// <summary>
    /// 根据菜单生成按钮
    /// </summary>
    /// <param name="menu">菜单实体</param>
    /// <returns>按钮列表</returns>
    private static List<Button> GenerateButtonsForMenu(Menu menu)
    {
        var buttons = new List<Button>();

        // 从菜单路径提取模块名（如 "users" -> "user", "settings-menu" -> "menu"）
        var moduleName = ExtractModuleName(menu.Path);

        // 根据菜单类型生成不同的按钮
        switch (menu.Path)
        {
            case "dashboard":
                // 首页通常没有操作按钮
                break;

            case "users":
            case "users-info":
                // 用户管理
                buttons.Add(CreateButton("添加用户", $"{moduleName}:add", menu.Id, "Plus", 1));
                buttons.Add(CreateButton("编辑用户", $"{moduleName}:edit", menu.Id, "Edit", 2));
                buttons.Add(CreateButton("删除用户", $"{moduleName}:delete", menu.Id, "Delete", 3));
                buttons.Add(CreateButton("重置密码", $"{moduleName}:reset_password", menu.Id, "Key", 4));
                buttons.Add(CreateButton("分配角色", $"{moduleName}:assign_role", menu.Id, "UserFilled", 5));
                buttons.Add(CreateButton("启用用户", $"{moduleName}:enable", menu.Id, "CircleCheck", 6));
                buttons.Add(CreateButton("禁用用户", $"{moduleName}:disable", menu.Id, "CircleClose", 7));
                break;

            case "users-role":
                // 角色管理
                buttons.Add(CreateButton("添加角色", $"{moduleName}:add", menu.Id, "Plus", 1));
                buttons.Add(CreateButton("编辑角色", $"{moduleName}:edit", menu.Id, "Edit", 2));
                buttons.Add(CreateButton("删除角色", $"{moduleName}:delete", menu.Id, "Delete", 3));
                buttons.Add(CreateButton("分配菜单", $"{moduleName}:assign_menu", menu.Id, "Menu", 4));
                buttons.Add(CreateButton("分配用户", $"{moduleName}:assign_user", menu.Id, "User", 5));
                buttons.Add(CreateButton("分配权限", $"{moduleName}:assign_permission", menu.Id, "Lock", 6));
                buttons.Add(CreateButton("启用角色", $"{moduleName}:enable", menu.Id, "CircleCheck", 7));
                buttons.Add(CreateButton("禁用角色", $"{moduleName}:disable", menu.Id, "CircleClose", 8));
                break;

            case "products":
                // 产品管理
                buttons.Add(CreateButton("添加产品", $"{moduleName}:add", menu.Id, "Plus", 1));
                buttons.Add(CreateButton("编辑产品", $"{moduleName}:edit", menu.Id, "Edit", 2));
                buttons.Add(CreateButton("删除产品", $"{moduleName}:delete", menu.Id, "Delete", 3));
                buttons.Add(CreateButton("启用产品", $"{moduleName}:enable", menu.Id, "CircleCheck", 4));
                buttons.Add(CreateButton("禁用产品", $"{moduleName}:disable", menu.Id, "CircleClose", 5));
                break;

            case "settings-menu":
                // 菜单管理
                buttons.Add(CreateButton("添加菜单", $"{moduleName}:add", menu.Id, "Plus", 1));
                buttons.Add(CreateButton("编辑菜单", $"{moduleName}:edit", menu.Id, "Edit", 2));
                buttons.Add(CreateButton("删除菜单", $"{moduleName}:delete", menu.Id, "Delete", 3));
                buttons.Add(CreateButton("添加子菜单", $"{moduleName}:add_child", menu.Id, "Plus", 4));
                break;

            case "settings-permissions":
                // 权限管理
                buttons.Add(CreateButton("添加权限", $"permission:add", menu.Id, "Plus", 1));
                buttons.Add(CreateButton("编辑权限", $"permission:edit", menu.Id, "Edit", 2));
                buttons.Add(CreateButton("删除权限", $"permission:delete", menu.Id, "Delete", 3));
                buttons.Add(CreateButton("启用权限", $"permission:enable", menu.Id, "CircleCheck", 4));
                buttons.Add(CreateButton("禁用权限", $"permission:disable", menu.Id, "CircleClose", 5));
                break;

            case "settings-system":
                // 系统管理
                buttons.Add(CreateButton("保存JWT配置", $"setting:save_jwt", menu.Id, "Check", 1));
                buttons.Add(CreateButton("保存系统配置", $"setting:save_system", menu.Id, "Check", 2));
                buttons.Add(CreateButton("清除认证缓存", $"cache:clear_auth", menu.Id, "Delete", 3));
                buttons.Add(CreateButton("清除用户缓存", $"cache:clear_user", menu.Id, "Delete", 4));
                buttons.Add(CreateButton("清除菜单缓存", $"cache:clear_menu", menu.Id, "Delete", 5));
                buttons.Add(CreateButton("清除列表缓存", $"cache:clear_list", menu.Id, "Delete", 6));
                buttons.Add(CreateButton("清除设置缓存", $"cache:clear_setting", menu.Id, "Delete", 7));
                buttons.Add(CreateButton("清除全部缓存", $"cache:clear_all", menu.Id, "Delete", 8));
                break;

            default:
                // 默认按钮（增删改查）
                buttons.Add(CreateButton("添加", $"{moduleName}:add", menu.Id, "Plus", 1));
                buttons.Add(CreateButton("编辑", $"{moduleName}:edit", menu.Id, "Edit", 2));
                buttons.Add(CreateButton("删除", $"{moduleName}:delete", menu.Id, "Delete", 3));
                buttons.Add(CreateButton("启用", $"{moduleName}:enable", menu.Id, "CircleCheck", 4));
                buttons.Add(CreateButton("禁用", $"{moduleName}:disable", menu.Id, "CircleClose", 5));
                break;
        }

        return buttons;
    }

    /// <summary>
    /// 从菜单路径提取模块名
    /// </summary>
    /// <param name="path">菜单路径</param>
    /// <returns>模块名</returns>
    private static string ExtractModuleName(string path)
    {
        // 处理 settings-xxx 格式
        if (path.StartsWith("settings-"))
        {
            return path.Substring(9); // 移除 "settings-" 前缀
        }

        // 处理 users-xxx 格式
        if (path.StartsWith("users-"))
        {
            return path.Substring(6); // 移除 "users-" 前缀
        }

        // 默认返回原路径（移除复数形式）
        return path.TrimEnd('s');
    }

    /// <summary>
    /// 创建按钮
    /// </summary>
    /// <param name="name">按钮名称</param>
    /// <param name="code">按钮编码</param>
    /// <param name="menuId">所属菜单ID</param>
    /// <param name="icon">图标</param>
    /// <param name="sortOrder">排序号</param>
    /// <returns>按钮实体</returns>
    private static Button CreateButton(string name, string code, Guid menuId, string icon, int sortOrder)
    {
        return Button.Create(
            name: name,
            code: code,
            menuId: menuId,
            permissionCode: code, // 权限编码与按钮编码一致
            icon: icon,
            sortOrder: sortOrder,
            description: $"{name}按钮",
            status: 1
        );
    }
}