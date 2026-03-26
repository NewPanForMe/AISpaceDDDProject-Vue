using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDProject.Domain.Entities;

namespace DDDProject.Infrastructure.Configuration;

/// <summary>
/// 角色实体配置
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // 表名
        builder.ToTable("Roles");

        // 主键
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        // 角色名称 - 必填
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.Name)
            .IsUnique();

        // 角色编码 - 必填，唯一
        builder.Property(r => r.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.Code)
            .IsUnique();

        // 角色描述
        builder.Property(r => r.Description)
            .HasMaxLength(200);

        // 角色状态 - 必填，默认为1（启用）
        builder.Property(r => r.Status)
            .IsRequired()
            .HasDefaultValue(1);

        // 排序号
        builder.Property(r => r.SortOrder)
            .HasDefaultValue(0);

        // 备注
        builder.Property(r => r.Remark)
            .HasMaxLength(500);

        // 创建时间
        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // 更新时间
        builder.Property(r => r.UpdatedAt);
    }
}

/// <summary>
/// 系统设置实体配置
/// </summary>
public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        // 表名
        builder.ToTable("Settings");

        // 主键
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        // 设置键 - 必填，唯一
        builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.Key)
            .IsUnique();

        // 设置值 - 必填
        builder.Property(s => s.Value)
            .IsRequired()
            .HasMaxLength(2000);

        // 设置描述
        builder.Property(s => s.Description)
            .HasMaxLength(500);

        // 设置分组
        builder.Property(s => s.Group)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("General");

        builder.HasIndex(s => s.Group);

        // 创建时间
        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // 更新时间
        builder.Property(s => s.UpdatedAt);
    }
}

/// <summary>
/// 权限实体配置
/// </summary>
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        // 表名
        builder.ToTable("Permissions");

        // 主键
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        // 权限编码 - 必填，唯一
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.Code)
            .IsUnique();

        // 权限名称 - 必填
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);

        // 权限描述
        builder.Property(p => p.Description)
            .HasMaxLength(200);

        // 所属模块 - 必填
        builder.Property(p => p.Module)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.Module);

        // 关联菜单ID
        builder.Property(p => p.MenuId);

        // 排序号
        builder.Property(p => p.SortOrder)
            .HasDefaultValue(0);

        // 状态 - 必填，默认为1（启用）
        builder.Property(p => p.Status)
            .IsRequired()
            .HasDefaultValue(1);

        // 创建时间
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // 更新时间
        builder.Property(p => p.UpdatedAt);
    }
}

/// <summary>
/// 角色权限关联实体配置
/// </summary>
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        // 表名
        builder.ToTable("RolePermissions");

        // 主键
        builder.HasKey(rp => rp.Id);
        builder.Property(rp => rp.Id).ValueGeneratedOnAdd();

        // 角色ID - 必填
        builder.Property(rp => rp.RoleId)
            .IsRequired();

        // 权限ID - 必填
        builder.Property(rp => rp.PermissionId)
            .IsRequired();

        // 创建复合唯一索引
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique();

        // 创建时间
        builder.Property(rp => rp.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // 更新时间
        builder.Property(rp => rp.UpdatedAt);
    }
}