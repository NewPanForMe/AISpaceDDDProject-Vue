using Microsoft.EntityFrameworkCore;
using DDDProject.Domain.Repositories;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Models;
using DDDProject.Infrastructure.Repositories;
using DDDProject.Infrastructure.Configuration;

namespace DDDProject.Infrastructure.Contexts;

/// <summary>
/// 数据库上下文
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// 仓储集合
    /// </summary>
    public DbSet<ExampleAggregate> Examples => Set<ExampleAggregate>();

    /// <summary>
    /// 用户集合
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// 菜单集合
    /// </summary>
    public DbSet<Menu> Menus => Set<Menu>();

    /// <summary>
    /// 角色集合
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>
    /// 用户角色关联集合
    /// </summary>
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    /// <summary>
    /// 菜单角色关联集合
    /// </summary>
    public DbSet<MenuRole> MenuRoles => Set<MenuRole>();

    /// <summary>
    /// 系统设置集合
    /// </summary>
    public DbSet<Setting> Settings => Set<Setting>();

    /// <summary>
    /// 权限集合
    /// </summary>
    public DbSet<Permission> Permissions => Set<Permission>();

    /// <summary>
    /// 角色权限关联集合
    /// </summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置示例聚合
        modelBuilder.Entity<ExampleAggregate>(entity =>
        {
            entity.ToTable("Examples");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()"); // 使用服务器本地时间
        });

        // 配置用户实体
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        // 配置菜单实体
        modelBuilder.ApplyConfiguration(new MenuConfiguration());

        // 配置角色实体
        modelBuilder.ApplyConfiguration(new RoleConfiguration());

        // 配置用户角色关联实体
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());

        // 配置菜单角色关联实体
        modelBuilder.ApplyConfiguration(new MenuRoleConfiguration());

        // 配置系统设置实体
        modelBuilder.ApplyConfiguration(new SettingConfiguration());

        // 配置权限实体
        modelBuilder.ApplyConfiguration(new PermissionConfiguration());

        // 配置角色权限关联实体
        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
    }
}

/// <summary>
/// 数据库上下文扩展方法
/// </summary>
public static class ApplicationDbContextExtensions
{
    /// <summary>
    /// 初始化用户角色关联数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedUserRoles(this ApplicationDbContext context)
    {
        // 检查是否已存在用户角色关联数据
        if (context.UserRoles.Any())
        {
            return;
        }

        var userRoles = new List<UserRole>();

        // 获取 Admin 用户
        var adminUser = context.Users.FirstOrDefault(u => u.UserName == "Admin");
        // 获取超级管理员角色
        var superAdminRole = context.Roles.FirstOrDefault(r => r.Code == "SUPER_ADMIN");

        if (adminUser is not null && superAdminRole is not null)
        {
            // 为 Admin 用户分配超级管理员角色
            userRoles.Add(UserRole.Create(adminUser.Id, superAdminRole.Id));
        }

        // 获取普通用户 zhangsan
        var zhangsanUser = context.Users.FirstOrDefault(u => u.UserName == "zhangsan");
        // 获取普通用户角色
        var userRole = context.Roles.FirstOrDefault(r => r.Code == "USER");

        if (zhangsanUser is not null && userRole is not null)
        {
            // 为 zhangsan 分配普通用户角色
            userRoles.Add(UserRole.Create(zhangsanUser.Id, userRole.Id));
        }

        // 获取普通用户 lisi
        var lisiUser = context.Users.FirstOrDefault(u => u.UserName == "lisi");
        // 获取管理员角色
        var adminRole = context.Roles.FirstOrDefault(r => r.Code == "ADMIN");

        if (lisiUser is not null && adminRole is not null)
        {
            // 为 lisi 分配管理员角色
            userRoles.Add(UserRole.Create(lisiUser.Id, adminRole.Id));
        }

        if (userRoles.Any())
        {
            context.UserRoles.AddRange(userRoles);
            context.SaveChanges();
        }
    }

    /// <summary>
    /// 初始化系统设置数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedSettings(this ApplicationDbContext context)
    {
        // 检查是否已存在设置数据
        if (context.Settings.Any())
        {
            return;
        }

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

        context.Settings.AddRange(settings);
        context.SaveChanges();
    }

    /// <summary>
    /// 初始化权限数据
    /// </summary>
    /// <param name="context">数据库上下文</param>
    public static void SeedPermissions(this ApplicationDbContext context)
    {
        // 检查是否已存在权限数据
        if (context.Permissions.Any())
        {
            return;
        }

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
            Permission.Create("role:enable", "启用角色", "Role", "启用角色", null, 6),
            Permission.Create("role:disable", "禁用角色", "Role", "禁用角色", null, 7),

            // 系统设置权限
            Permission.Create("setting:save_jwt", "保存JWT配置", "Setting", "保存JWT配置", null, 1),
            Permission.Create("setting:save_system", "保存系统配置", "Setting", "保存系统配置", null, 2),

            // 缓存管理权限
            Permission.Create("cache:clear_login", "清除登录缓存", "Cache", "清除登录缓存", null, 1),
            Permission.Create("cache:clear_menu", "清除菜单缓存", "Cache", "清除菜单缓存", null, 2),
            Permission.Create("cache:clear_list", "清除列表缓存", "Cache", "清除列表缓存", null, 3),
            Permission.Create("cache:clear_all", "清除全部缓存", "Cache", "清除全部缓存", null, 4)
        };

        context.Permissions.AddRange(permissions);
        context.SaveChanges();
    }
}
