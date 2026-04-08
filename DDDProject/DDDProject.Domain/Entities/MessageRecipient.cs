namespace DDDProject.Domain.Entities;

/// <summary>
/// 消息接收者实体 - 存储消息推送的相关人员及阅读状态
/// </summary>
public class MessageRecipient : Entity<Guid>
{
    /// <summary>
    /// 消息 ID
    /// </summary>
    public Guid MessageId { get; private set; }

    /// <summary>
    /// 接收者 ID
    /// </summary>
    public Guid RecipientId { get; private set; }

    /// <summary>
    /// 接收者用户名
    /// </summary>
    public string RecipientName { get; private set; } = string.Empty;

    /// <summary>
    /// 是否已读
    /// </summary>
    public bool IsRead { get; private set; } = false;

    /// <summary>
    /// 阅读时间
    /// </summary>
    public DateTime? ReadTime { get; private set; }

    /// <summary>
    /// 是否已删除（接收者删除）
    /// </summary>
    public bool IsDeleted { get; private set; } = false;

    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime? DeletedTime { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// 是否已撤回（被发送者撤回）
    /// </summary>
    public bool IsRevoked { get; private set; } = false;

    /// <summary>
    /// 撤回时间
    /// </summary>
    public DateTime? RevokedTime { get; private set; }

    /// <summary>
    /// 构造函数（私有，用于 ORM）
    /// </summary>
    protected MessageRecipient() { }

    /// <summary>
    /// 创建消息接收者
    /// </summary>
    public static MessageRecipient Create(
        Guid messageId,
        Guid recipientId,
        string recipientName)
    {
        return new MessageRecipient
        {
            Id = Guid.NewGuid(),
            MessageId = messageId,
            RecipientId = recipientId,
            RecipientName = recipientName,
            IsRead = false,
            IsDeleted = false,
            CreatedAt = DateTime.Now
        };
    }

    /// <summary>
    /// 批量创建消息接收者
    /// </summary>
    public static List<MessageRecipient> CreateBatch(
        Guid messageId,
        List<(Guid Id, string Name)> recipients)
    {
        return recipients.Select(r => Create(messageId, r.Id, r.Name)).ToList();
    }

    /// <summary>
    /// 标记为已读
    /// </summary>
    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadTime = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// 标记为未读
    /// </summary>
    public void MarkAsUnread()
    {
        IsRead = false;
        ReadTime = null;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 删除（软删除）
    /// </summary>
    public void Delete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedTime = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// 更新接收者信息
    /// </summary>
    public void UpdateRecipientName(string recipientName)
    {
        RecipientName = recipientName;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 标记为已撤回
    /// </summary>
    public void Revoke()
    {
        if (!IsRevoked)
        {
            IsRevoked = true;
            RevokedTime = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
