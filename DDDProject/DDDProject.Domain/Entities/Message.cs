namespace DDDProject.Domain.Entities;

/// <summary>
/// 站内信实体
/// </summary>
public class Message : AggregateRoot
{
    /// <summary>
    /// 发送者 ID（系统消息时为 null）
    /// </summary>
    public Guid? SenderId { get; private set; }

    /// <summary>
    /// 发送者用户名
    /// </summary>
    public string? SenderName { get; private set; } = string.Empty;

    /// <summary>
    /// 接收者 ID
    /// </summary>
    public Guid ReceiverId { get; private set; }

    /// <summary>
    /// 接收者用户名
    /// </summary>
    public string ReceiverName { get; private set; } = string.Empty;

    /// <summary>
    /// 消息标题
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// 消息类型：System, User
    /// </summary>
    public string MessageType { get; private set; } = MessageTypeConst.User;

    /// <summary>
    /// 消息优先级：Normal, Important, Urgent
    /// </summary>
    public string Priority { get; private set; } = MessagePriority.Normal;

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
    /// 是否已推送
    /// </summary>
    public bool IsPushed { get; private set; } = false;

    /// <summary>
    /// 推送时间
    /// </summary>
    public DateTime? PushedTime { get; private set; }

    /// <summary>
    /// 是否已撤回
    /// </summary>
    public bool IsRevoked { get; private set; } = false;

    /// <summary>
    /// 撤回时间
    /// </summary>
    public DateTime? RevokedTime { get; private set; }

    /// <summary>
    /// 撤回人 ID
    /// </summary>
    public Guid? RevokedBy { get; private set; }

    /// <summary>
    /// 构造函数（私有，用于 ORM）
    /// </summary>
    protected Message() { }

    /// <summary>
    /// 创建用户消息
    /// </summary>
    public static Message CreateUserMessage(
        Guid senderId,
        string senderName,
        Guid receiverId,
        string receiverName,
        string title,
        string content,
        string priority = MessagePriority.Normal)
    {
        return new Message
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            SenderName = senderName,
            ReceiverId = receiverId,
            ReceiverName = receiverName,
            Title = title,
            Content = content,
            MessageType = MessageTypeConst.User,
            Priority = priority,
            IsRead = false,
            CreatedAt = DateTime.Now
        };
    }

    /// <summary>
    /// 创建系统消息
    /// </summary>
    public static Message CreateSystemMessage(
        Guid receiverId,
        string receiverName,
        string title,
        string content,
        string priority = MessagePriority.Normal)
    {
        return new Message
        {
            Id = Guid.NewGuid(),
            SenderId = null,
            SenderName = "系统",
            ReceiverId = receiverId,
            ReceiverName = receiverName,
            Title = title,
            Content = content,
            MessageType = MessageTypeConst.System,
            Priority = priority,
            IsRead = false,
            CreatedAt = DateTime.Now
        };
    }

    /// <summary>
    /// 创建推送消息（批量推送时使用，不指定单个接收者）
    /// </summary>
    public static Message CreatePushMessage(
        Guid senderId,
        string senderName,
        string title,
        string content,
        string priority = MessagePriority.Normal)
    {
        return new Message
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            SenderName = string.IsNullOrWhiteSpace(senderName) ? "系统" : senderName,
            ReceiverId = Guid.Empty,
            ReceiverName = string.Empty,
            Title = title,
            Content = content,
            MessageType = MessageTypeConst.System,
            Priority = priority,
            IsRead = false,
            IsPushed = true,
            PushedTime = DateTime.Now,
            CreatedAt = DateTime.Now
        };
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
    /// 删除消息（软删除）
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
    /// 更新消息内容
    /// </summary>
    public void Update(string title, string content, string priority)
    {
        Title = title;
        Content = content;
        Priority = priority;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 标记为已推送
    /// </summary>
    public void MarkAsPushed()
    {
        if (!IsPushed)
        {
            IsPushed = true;
            PushedTime = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// 撤回消息
    /// </summary>
    public void Revoke(Guid revokedBy)
    {
        if (!IsRevoked)
        {
            IsRevoked = true;
            RevokedTime = DateTime.Now;
            RevokedBy = revokedBy;
            UpdatedAt = DateTime.Now;
        }
    }
}

/// <summary>
/// 消息类型常量
/// </summary>
public static class MessageTypeConst
{
    public const string System = "System";
    public const string User = "User";
}

/// <summary>
/// 消息优先级常量
/// </summary>
public static class MessagePriority
{
    public const string Normal = "Normal";
    public const string Important = "Important";
    public const string Urgent = "Urgent";
}
