using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDProject.Domain.Entities;

namespace DDDProject.Infrastructure.Configuration;

/// <summary>
/// 用户角色关联实体配置
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // 表名
        builder.ToTable("UserRoles");

        // 主键
        builder.HasKey(ur => ur.Id);
        builder.Property(ur => ur.Id).ValueGeneratedOnAdd();

        // 用户ID - 必填
        builder.Property(ur => ur.UserId)
            .IsRequired();

        // 角色ID - 必填
        builder.Property(ur => ur.RoleId)
            .IsRequired();

        // 创建复合唯一索引，确保一个用户只能关联一个角色一次
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        // 创建时间
        builder.Property(ur => ur.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // 更新时间
        builder.Property(ur => ur.UpdatedAt);
    }
}