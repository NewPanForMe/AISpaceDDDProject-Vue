using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DDDProject.Infrastructure.Contexts;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 测试权限种子数据 - 用于 PermissionTestController 的测试权限
/// </summary>
public static class PermissionTestSeeder
{
    /// <summary>
    /// 种植测试权限数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedTestPermissions(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        context.Database.EnsureCreated();

        // 获取测试权限编码列表
        var testPermissionCodes = GetTestPermissionCodes();

        // 检查是否已存在这些测试权限
        var existingCodes = context.Permissions
            .Where(p => testPermissionCodes.Contains(p.Code))
            .Select(p => p.Code)
            .ToList();

        // 过滤出未存在的权限
        var newPermissionCodes = testPermissionCodes.Except(existingCodes).ToList();

        if (newPermissionCodes.Count == 0)
        {
            return;
        }

        // 创建新的权限实体
        var permissions = newPermissionCodes.Select((code, index) =>
        {
            var (name, module, description) = GetPermissionInfo(code);
            return Permission.Create(code, name, module, description, null, index + 1);
        }).ToList();

        context.Permissions.AddRange(permissions);
        context.SaveChanges();
    }

    /// <summary>
    /// 异步种植测试权限数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedTestPermissionsAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 获取测试权限编码列表
        var testPermissionCodes = GetTestPermissionCodes();

        // 检查是否已存在这些测试权限
        var existingCodes = await context.Permissions
            .Where(p => testPermissionCodes.Contains(p.Code))
            .Select(p => p.Code)
            .ToListAsync();

        // 过滤出未存在的权限
        var newPermissionCodes = testPermissionCodes.Except(existingCodes).ToList();

        if (newPermissionCodes.Count == 0)
        {
            return;
        }

        // 创建新的权限实体
        var permissions = newPermissionCodes.Select((code, index) =>
        {
            var (name, module, description) = GetPermissionInfo(code);
            return Permission.Create(code, name, module, description, null, index + 1);
        }).ToList();

        await context.Permissions.AddRangeAsync(permissions);
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

    /// <summary>
    /// 获取权限信息（名称、模块、描述）
    /// </summary>
    /// <param name="code">权限编码</param>
    /// <returns>(名称, 模块, 描述)</returns>
    private static (string Name, string Module, string Description) GetPermissionInfo(string code)
    {
        return code switch
        {
            "test:single" => ("测试-单个权限", "Test", "测试单个权限验证"),
            "test:read" => ("测试-读取权限", "Test", "测试读取权限"),
            "test:write" => ("测试-写入权限", "Test", "测试写入权限"),
            "test:admin" => ("测试-管理员权限", "Test", "测试管理员权限"),
            "test:create" => ("测试-创建权限", "Test", "测试创建操作权限"),
            "test:update" => ("测试-更新权限", "Test", "测试更新操作权限"),
            "test:delete" => ("测试-删除权限", "Test", "测试删除操作权限"),
            "test:super_admin" => ("测试-超级管理员", "Test", "测试超级管理员权限"),
            "test:module1:read" => ("测试-模块1读取", "Test", "测试模块1读取权限"),
            "test:module2:write" => ("测试-模块2写入", "Test", "测试模块2写入权限"),
            "test:batch:create" => ("测试-批量创建", "Test", "测试批量创建权限"),
            "test:batch:update" => ("测试-批量更新", "Test", "测试批量更新权限"),
            "test:batch:delete" => ("测试-批量删除", "Test", "测试批量删除权限"),
            "test:report:export" => ("测试-报告导出", "Test", "测试报告导出权限"),
            "test:config:read" => ("测试-配置读取", "Test", "测试配置读取权限"),
            "test:config:write" => ("测试-配置写入", "Test", "测试配置写入权限"),
            _ => ("测试权限", "Test", "测试权限")
        };
    }
} 
