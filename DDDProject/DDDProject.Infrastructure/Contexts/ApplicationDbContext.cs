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
    /// <summary>
    /// 角色权限关联集合
    /// </summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    /// <summary>
    /// 按钮集合
    /// </summary>
    public DbSet<Button> Buttons => Set<Button>();

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

        // 配置按钮实体
        modelBuilder.ApplyConfiguration(new ButtonConfiguration());
    }
}
