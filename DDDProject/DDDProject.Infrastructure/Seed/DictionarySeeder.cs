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
            Dictionary.Create("permission_type_button", "按钮权限", "button", "permission_type", 3, "按钮操作权限"),

            // 操作日志 - 操作类型字典
            Dictionary.Create("log_operation_type_create", "创建", "Create", "log_operation_type", 1, "创建操作"),
            Dictionary.Create("log_operation_type_update", "更新", "Update", "log_operation_type", 2, "更新操作"),
            Dictionary.Create("log_operation_type_delete", "删除", "Delete", "log_operation_type", 3, "删除操作"),
            Dictionary.Create("log_operation_type_export", "导出", "Export", "log_operation_type", 4, "导出操作"),
            Dictionary.Create("log_operation_type_import", "导入", "Import", "log_operation_type", 5, "导入操作"),
            Dictionary.Create("log_operation_type_enable", "启用", "Enable", "log_operation_type", 6, "启用操作"),
            Dictionary.Create("log_operation_type_disable", "禁用", "Disable", "log_operation_type", 7, "禁用操作"),
            Dictionary.Create("log_operation_type_login", "登录", "Login", "log_operation_type", 8, "登录操作"),
            Dictionary.Create("log_operation_type_logout", "登出", "Logout", "log_operation_type", 9, "登出操作"),
            Dictionary.Create("log_operation_type_assign", "分配", "Assign", "log_operation_type", 10, "分配操作"),
            Dictionary.Create("log_operation_type_other", "其他", "Other", "log_operation_type", 11, "其他操作"),

            // 操作日志 - 操作模块字典
            Dictionary.Create("log_module_user", "用户", "User", "log_module", 1, "用户模块"),
            Dictionary.Create("log_module_role", "角色", "Role", "log_module", 2, "角色模块"),
            Dictionary.Create("log_module_menu", "菜单", "Menu", "log_module", 3, "菜单模块"),
            Dictionary.Create("log_module_permission", "权限", "Permission", "log_module", 4, "权限模块"),
            Dictionary.Create("log_module_setting", "设置", "Setting", "log_module", 5, "设置模块"),
            Dictionary.Create("log_module_log", "日志", "Log", "log_module", 6, "日志模块"),
            Dictionary.Create("log_module_cache", "缓存", "Cache", "log_module", 7, "缓存模块"),
            Dictionary.Create("log_module_dictionary", "字典", "Dictionary", "log_module", 8, "字典模块"),
            Dictionary.Create("log_module_other", "其他", "Other", "log_module", 9, "其他模块"),

            // 操作日志 - 状态字典
            Dictionary.Create("log_status_success", "成功", "Success", "log_status", 1, "操作成功"),
            Dictionary.Create("log_status_failure", "失败", "Failure", "log_status", 2, "操作失败"),

            // 站内信 - 消息类型字典
            Dictionary.Create("message_type_system", "系统消息", "System", "message_type", 1, "系统发送的消息"),
            Dictionary.Create("message_type_user", "用户消息", "User", "message_type", 2, "用户发送的消息"),

            // 站内信 - 消息优先级字典
            Dictionary.Create("message_priority_normal", "普通", "Normal", "message_priority", 1, "普通优先级消息"),
            Dictionary.Create("message_priority_important", "重要", "Important", "message_priority", 2, "重要优先级消息"),
            Dictionary.Create("message_priority_urgent", "紧急", "Urgent", "message_priority", 3, "紧急优先级消息")
        };

        return dictionaries;
    }
}