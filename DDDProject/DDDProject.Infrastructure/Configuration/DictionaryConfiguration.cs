using DDDProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDProject.Infrastructure.Configuration;

/// <summary>
/// 字典实体配置
/// </summary>
public class DictionaryConfiguration : IEntityTypeConfiguration<Dictionary>
{
    public void Configure(EntityTypeBuilder<Dictionary> builder)
    {
        // 表名
        builder.ToTable("Dictionaries");

        // 主键
        builder.HasKey(d => d.Id);

        // 字段配置
        builder.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Value)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Status)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(d => d.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.Property(d => d.Remark)
            .HasMaxLength(500);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt);

        // 索引
        builder.HasIndex(d => d.Code)
            .IsUnique();

        builder.HasIndex(d => d.Type);

        builder.HasIndex(d => d.Status);
    }
}