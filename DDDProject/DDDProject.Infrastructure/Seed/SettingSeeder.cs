using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DDDProject.Infrastructure.Contexts;

namespace DDDProject.Infrastructure.Seed;

/// <summary>
/// 系统设置种子数据
/// </summary>
public static class SettingSeeder
{
    /// <summary>
    /// 种植系统设置数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedSettings(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        context.Database.EnsureCreated();

        // 检查是否已存在设置数据
        if (context.Settings.Any())
        {
            return;
        }

        var settings = GetSeedSettings();

        context.Settings.AddRange(settings);
        context.SaveChanges();
    }

    /// <summary>
    /// 异步种植系统设置数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static async Task SeedSettingsAsync(this ApplicationDbContext context)
    {
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();

        // 检查是否已存在设置数据
        if (await context.Settings.AnyAsync())
        {
            return;
        }

        var settings = GetSeedSettings();

        await context.Settings.AddRangeAsync(settings);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 获取种子系统设置数据
    /// </summary>
    /// <returns>设置列表</returns>
    private static List<Setting> GetSeedSettings()
    {
        var settings = new List<Setting>
        {
            // JWT 配置
            Setting.Create("JwtSettings_Issuer", "DDDProject", "JWT 颁发者", "JwtSettings"),
            Setting.Create("JwtSettings_Audience", "DDDProject", "JWT 受众", "JwtSettings"),
            Setting.Create("JwtSettings_Key", "1fe277c55303f1c97e0d5861959039077", "JWT 签名密钥（建议使用 32 位以上随机字符串）", "JwtSettings"),
            Setting.Create("JwtSettings_ExpireMinutes", "720", "JWT 过期时间（分钟）", "JwtSettings"),

            // 系统配置
            Setting.Create("SystemName", "DDD 项目管理系统", "系统名称", "System"),
            Setting.Create("SystemDescription", "基于 DDD 架构的企业级管理系统", "系统描述", "System")
        };

        return settings;
    }
}