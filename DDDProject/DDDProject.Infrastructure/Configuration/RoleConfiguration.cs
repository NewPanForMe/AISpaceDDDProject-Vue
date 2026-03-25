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