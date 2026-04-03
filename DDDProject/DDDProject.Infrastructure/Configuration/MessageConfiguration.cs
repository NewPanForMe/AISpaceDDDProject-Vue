using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDProject.Domain.Entities;

namespace DDDProject.Infrastructure.Configuration;

/// <summary>
/// 站内信实体配置
/// </summary>
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        // 表名
        builder.ToTable("Messages");

        // 主键
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();

        // 发送者ID
        builder.Property(m => m.SenderId);

        // 发送者用户名
        builder.Property(m => m.SenderName)
            .HasMaxLength(50);

        // 接收者ID
        builder.Property(m => m.ReceiverId)
            .IsRequired();

        // 接收者用户名
        builder.Property(m => m.ReceiverName)
            .IsRequired()
            .HasMaxLength(50);

        // 消息标题
        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);

        // 消息内容
        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(4000);

        // 消息类型
        builder.Property(m => m.MessageType)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue(MessageTypeConst.User);

        // 消息优先级
        builder.Property(m => m.Priority)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue(MessagePriority.Normal);

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

        // 是否已推送
        builder.Property(m => m.IsPushed)
            .IsRequired()
            .HasDefaultValue(false);

        // 推送时间
        builder.Property(m => m.PushedTime);

        // 创建时间
        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // 更新时间
        builder.Property(m => m.UpdatedAt);

        // 索引
        builder.HasIndex(m => m.SenderId);
        builder.HasIndex(m => m.ReceiverId);
        builder.HasIndex(m => m.MessageType);
        builder.HasIndex(m => m.Priority);
        builder.HasIndex(m => m.IsRead);
        builder.HasIndex(m => m.IsDeleted);
        builder.HasIndex(m => m.IsPushed);
        builder.HasIndex(m => m.CreatedAt);

        // 组合索引（常用查询）
        builder.HasIndex(m => new { m.ReceiverId, m.IsDeleted, m.IsRead });
    }
}