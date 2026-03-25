using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDProject.Domain.Entities;

namespace DDDProject.Infrastructure.Configuration;

/// <summary>
/// 菜单角色关联实体配置
/// </summary>
public class MenuRoleConfiguration : IEntityTypeConfiguration<MenuRole>
{
    public void Configure(EntityTypeBuilder<MenuRole> builder)
    {
        // 表名
        builder.ToTable("MenuRoles");

        // 主键
        builder.HasKey(mr => mr.Id);
        builder.Property(mr => mr.Id).ValueGeneratedOnAdd();

        // 菜单ID - 必填
        builder.Property(mr => mr.MenuId)
            .IsRequired();

        // 角色ID - 必填
        builder.Property(mr => mr.RoleId)
            .IsRequired();

        // 创建复合唯一索引，确保一个菜单只能关联一个角色一次
        builder.HasIndex(mr => new { mr.MenuId, mr.RoleId })
            .IsUnique();

        // 创建时间
        builder.Property(mr => mr.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // 更新时间
        builder.Property(mr => mr.UpdatedAt);
    }
}