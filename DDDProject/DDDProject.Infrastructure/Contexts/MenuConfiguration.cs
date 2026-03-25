using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDProject.Domain.Entities;

namespace DDDProject.Infrastructure.Contexts;

/// <summary>
/// Menu实体配置
/// </summary>
public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menus");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Path)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Component)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Icon)
            .HasMaxLength(100);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETDATE()"); // 使用服务器本地时间

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        // 配置父子关系
        builder.HasOne(m => m.Parent)
            .WithMany(m => m.Children)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // 防止级联删除，确保数据完整性
    }
}