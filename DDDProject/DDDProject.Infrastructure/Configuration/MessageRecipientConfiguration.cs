using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDProject.Domain.Entities;

namespace DDDProject.Infrastructure.Configuration;

/// <summary>
/// 消息接收者实体配置
/// </summary>
public class MessageRecipientConfiguration : IEntityTypeConfiguration<MessageRecipient>
{
    public void Configure(EntityTypeBuilder<MessageRecipient> builder)
    {
        // 表名
        builder.ToTable("MessageRecipients");

        // 主键
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();

        // 消息ID
        builder.Property(m => m.MessageId)
            .IsRequired();

        // 接收者ID
        builder.Property(m => m.RecipientId)
            .IsRequired();

        // 接收者用户名
        builder.Property(m => m.RecipientName)
            .IsRequired()
            .HasMaxLength(50);

        // 是否已读
        builder.Property(m => m.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        // 阅读时间
        builder.Property(m => m.ReadTime);

        // 是否已删除
        builder.Property(m => m.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // 删除时间
        builder.Property(m => m.DeletedTime);

        // 创建时间
        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // 更新时间
        builder.Property(m => m.UpdatedAt);

        // 索引
        builder.HasIndex(m => m.MessageId);
        builder.HasIndex(m => m.RecipientId);
        builder.HasIndex(m => m.IsRead);
        builder.HasIndex(m => m.IsDeleted);

        // 组合索引（常用查询）
        builder.HasIndex(m => new { m.MessageId, m.RecipientId }).IsUnique();
        builder.HasIndex(m => new { m.RecipientId, m.IsDeleted, m.IsRead });

        // 外键关系
        builder.HasOne<Message>()
            .WithMany()
            .HasForeignKey(m => m.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}