using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DDDProject.Infrastructure.Contexts;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 字典种子数据
/// </summary>
public static class DictionarySeeder
{
    /// <summary>
    /// 种植字典数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedDictionaries(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        context.Database.EnsureCreated();

        // 检查是否已存在字典数据
        if (context.Dictionaries.Any())
        {
            return;
        }

        var dictionaries = GetSeedDictionaries();

        context.Dictionaries.AddRange(dictionaries);
        context.SaveChanges();
    }

    /// <summary>
    /// 异步种植字典数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedDictionariesAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 检查是否已存在字典数据
        if (await context.Dictionaries.AnyAsync())
        {
            return;
        }

        var dictionaries = GetSeedDictionaries();

        await context.Dictionaries.AddRangeAsync(dictionaries);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 获取种子字典数据
    /// </summary>
    /// <returns>字典列表</returns>
    private static List<Dictionary> GetSeedDictionaries()
    {
        var dictionaries = new List<Dictionary>
        {
            // 状态字典
            Dictionary.Create("status_enabled", "启用", "1", "status", 1, "启用状态"),
            Dictionary.Create("status_disabled", "禁用", "0", "status", 2, "禁用状态"),

            // 性别字典
            Dictionary.Create("gender_male", "男", "1", "gender", 1, "男性"),
            Dictionary.Create("gender_female", "女", "2", "gender", 2, "女性"),
            Dictionary.Create("gender_other", "其他", "0", "gender", 3, "其他性别"),

            // 用户类型字典
            Dictionary.Create("user_type_admin", "管理员", "admin", "user_type", 1, "系统管理员"),
            Dictionary.Create("user_type_user", "普通用户", "user", "user_type", 2, "普通用户"),
            Dictionary.Create("user_type_guest", "访客", "guest", "user_type", 3, "访客用户"),

            // 菜单类型字典
            Dictionary.Create("menu_type_directory", "目录", "directory", "menu_type", 1, "菜单目录"),
            Dictionary.Create("menu_type_menu", "菜单", "menu", "menu_type", 2, "菜单页面"),
            Dictionary.Create("menu_type_button", "按钮", "button", "menu_type", 3, "按钮权限"),

            // 权限类型字典
            Dictionary.Create("permission_type_api", "API权限", "api", "permission_type", 1, "API接口权限"),
            Dictionary.Create("permission_type_menu", "菜单权限", "menu", "permission_type", 2, "菜单访问权限"),
            Dictionary.Create("permission_type_button", "按钮权限", "button", "permission_type", 3, "按钮操作权限")
        };

        return dictionaries;
    }
}