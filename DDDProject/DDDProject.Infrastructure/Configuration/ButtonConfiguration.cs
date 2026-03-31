using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDProject.Domain.Entities;

namespace DDDProject.Infrastructure.Configuration;

/// <summary>
/// 按钮实体配置
/// </summary>
public class ButtonConfiguration : IEntityTypeConfiguration<Button>
{
    public void Configure(EntityTypeBuilder<Button> builder)
    {
        // 表名
        builder.ToTable("Buttons");

        // 主键
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();

        // 按钮名称 - 必填，最大长度50
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(50);

        // 按钮编码 - 必填，最大长度100，唯一
        builder.Property(b => b.Code)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(b => b.Code)
            .IsUnique();

        // 所属菜单ID - 必填
        builder.Property(b => b.MenuId)
            .IsRequired();

        // 权限编码 - 可选，最大长度100
        builder.Property(b => b.PermissionCode)
            .HasMaxLength(100);

        // 图标 - 可选，最大长度50
        builder.Property(b => b.Icon)
            .HasMaxLength(50);

        // 排序号 - 默认0
        builder.Property(b => b.SortOrder)
            .HasDefaultValue(0);

        // 状态 - 默认1（启用）
        builder.Property(b => b.Status)
            .HasDefaultValue(1);

        // 描述 - 可选，最大长度500
        builder.Property(b => b.Description)
            .HasMaxLength(500);

        // 创建时间 - 默认当前时间
        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // 更新时间
        builder.Property(b => b.UpdatedAt);

        // 外键关联 - 菜单
        builder.HasOne(b => b.Menu)
            .WithMany()
            .HasForeignKey(b => b.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        // 创建复合索引 - 菜单ID + 排序号
        builder.HasIndex(b => new { b.MenuId, b.SortOrder });

        // 创建索引 - 状态
        builder.HasIndex(b => b.Status);
    }
}
