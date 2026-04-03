namespace DDDProject.Application.DTOs;

/// <summary>
/// 站内信 DTO
/// </summary>
public class MessageDto
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 发送者ID
    /// </summary>
    public Guid? SenderId { get; set; }

    /// <summary>
    /// 发送者用户名
    /// </summary>
    public string? SenderName { get; set; } = string.Empty;

    /// <summary>
    /// 接收者ID
    /// </summary>
    public Guid ReceiverId { get; set; }

    /// <summary>
    /// 接收者用户名
    /// </summary>
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 消息标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息类型：System, User
    /// </summary>
    public string MessageType { get; set; } = string.Empty;

    /// <summary>
    /// 消息优先级：Normal, Important, Urgent
    /// </summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>
    /// 是否已读
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// 阅读时间
    /// </summary>
    public DateTime? ReadTime { get; set; }

    /// <summary>
    /// 是否已推送
    /// </summary>
    public bool IsPushed { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 站内信查询请求
/// </summary>
public class MessageQueryRequest : PagedRequest
{
    /// <summary>
    /// 是否只查询未读消息
    /// </summary>
    public bool? OnlyUnread { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public string? MessageType { get; set; }

    /// <summary>
    /// 消息优先级
    /// </summary>
    public string? Priority { get; set; }

    /// <summary>
    /// 关键词（搜索标题）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 发送者用户名
    /// </summary>
    public string? SenderName { get; set; }
}

/// <summary>
/// 创建站内信请求
/// </summary>
public class CreateMessageRequest
{
    /// <summary>
    /// 接收者ID
    /// </summary>
    public Guid ReceiverId { get; set; }

    /// <summary>
    /// 接收者用户名
    /// </summary>
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 消息标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息类型：System, User
    /// </summary>
    public string MessageType { get; set; } = "User";

    /// <summary>
    /// 消息优先级：Normal, Important, Urgent
    /// </summary>
    public string Priority { get; set; } = "Normal";
}

/// <summary>
/// 批量发送系统消息请求
/// </summary>
public class BatchSendMessageRequest
{
    /// <summary>
    /// 接收者ID列表
    /// </summary>
    public List<Guid> ReceiverIds { get; set; } = new();

    /// <summary>
    /// 消息标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息优先级
    /// </summary>
    public string Priority { get; set; } = "Normal";
}

/// <summary>
/// 消息统计 DTO
/// </summary>
public class MessageStatisticsDto
{
    /// <summary>
    /// 总消息数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 未读消息数
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// 系统消息数
    /// </summary>
    public int SystemMessageCount { get; set; }

    /// <summary>
    /// 用户消息数
    /// </summary>
    public int UserMessageCount { get; set; }
}

/// <summary>
/// 更新站内信请求
/// </summary>
public class UpdateMessageRequest
{
    /// <summary>
    /// 消息标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息优先级：Normal, Important, Urgent
    /// </summary>
    public string Priority { get; set; } = "Normal";
}

/// <summary>
/// 推送系统消息请求（给所有用户）
/// </summary>
public class PushMessageRequest
{
    /// <summary>
    /// 消息标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息优先级
    /// </summary>
    public string Priority { get; set; } = "Normal";
}

/// <summary>
/// 推送系统消息给指定角色用户请求
/// </summary>
public class PushMessageToRoleRequest
{
    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<Guid> RoleIds { get; set; } = new();

    /// <summary>
    /// 消息标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息优先级
    /// </summary>
    public string Priority { get; set; } = "Normal";
}

/// <summary>
/// 推送已有消息请求
/// </summary>
public class PushExistingMessageRequest
{
    /// <summary>
    /// 推送类型：all-所有用户，role-指定角色，user-指定用户
    /// </summary>
    public string PushType { get; set; } = "all";

    /// <summary>
    /// 角色ID列表（PushType为role时使用）
    /// </summary>
    public List<Guid>? RoleIds { get; set; }

    /// <summary>
    /// 用户ID列表（PushType为user时使用）
    /// </summary>
    public List<Guid>? UserIds { get; set; }
}