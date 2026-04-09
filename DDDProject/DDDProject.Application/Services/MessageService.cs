using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Repositories;
using System.Linq.Expressions;

namespace DDDProject.Application.Services;

/// <summary>
/// 站内信服务实现
/// </summary>
public class MessageService : IMessageService
{
    private readonly IRepository<Message> _messageRepository;
    private readonly IRepository<MessageRecipient> _recipientRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<Role> _roleRepository;

    public MessageService(
        IRepository<Message> messageRepository,
        IRepository<MessageRecipient> recipientRepository,
        IRepository<User> userRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<Role> roleRepository)
    {
        _messageRepository = messageRepository;
        _recipientRepository = recipientRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
    }

    /// <summary>
    /// 获取当前用户的消息列表（分页、支持筛选）- 兼容旧方法
    /// </summary>
    public async Task<ApiRequestResult> GetMessagesAsync(Guid userId, MessageQueryRequest request)
    {
        return await GetUserMessagesAsync(userId, request);
    }

    /// <summary>
    /// 获取消息详情 - 兼容旧方法
    /// </summary>
    public async Task<ApiRequestResult> GetMessageByIdAsync(Guid id, Guid userId)
    {
        // 通过 MessageRecipient 查找
        var recipients = await _recipientRepository.GetListAsync(
            r => r.MessageId == id && r.RecipientId == userId && !r.IsDeleted);
        var recipient = recipients.FirstOrDefault();

        if (recipient is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        return await GetUserMessageByIdAsync(recipient.Id, userId);
    }

    /// <summary>
    /// 发送用户消息
    /// </summary>
    public async Task<ApiRequestResult> SendMessageAsync(Guid senderId, string senderName, CreateMessageRequest request)
    {
        // 验证接收者是否存在
        var receiver = await _userRepository.FindAsync(request.ReceiverId);
        if (receiver is null)
        {
            return new ApiRequestResult { Success = false, Message = "接收者不存在" };
        }

        var message = Message.CreateUserMessage(
            senderId,
            senderName,
            request.ReceiverId,
            request.ReceiverName,
            request.Title,
            request.Content,
            request.Priority
        );

        await _messageRepository.AddAsync(message);

        // 创建接收者记录
        var recipient = MessageRecipient.Create(message.Id, request.ReceiverId, request.ReceiverName);
        await _recipientRepository.AddAsync(recipient);

        await _messageRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "消息发送成功", Data = message.Id };
    }

    /// <summary>
    /// 发送系统消息给指定用户
    /// </summary>
    public async Task<ApiRequestResult> SendSystemMessageAsync(Guid receiverId, string receiverName, string title, string content, string priority = MessagePriority.Normal)
    {
        // 验证接收者是否存在
        var receiver = await _userRepository.FindAsync(receiverId);
        if (receiver is null)
        {
            return new ApiRequestResult { Success = false, Message = "接收者不存在" };
        }

        var message = Message.CreateSystemMessage(
            receiverId,
            receiverName,
            title,
            content,
            priority
        );

        await _messageRepository.AddAsync(message);

        // 创建接收者记录
        var recipient = MessageRecipient.Create(message.Id, receiverId, receiverName);
        await _recipientRepository.AddAsync(recipient);

        await _messageRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "系统消息发送成功", Data = message.Id };
    }

    /// <summary>
    /// 批量发送系统消息                 PushMessageToAllAsync(PushMessageRequest request)
    /// </summary>
    public async Task<ApiRequestResult> BatchSendSystemMessageAsync(BatchSendMessageRequest request, Guid userId)
    {
        if (request.ReceiverIds is null || request.ReceiverIds.Count == 0)
        {
            return new ApiRequestResult { Success = false, Message = "请选择接收者" };
        }

        // 获取用户信息
        var users = await _userRepository.GetListAsync(u => request.ReceiverIds.Contains(u.Id));
        var userList = users.ToList();

        // 创建消息 - 使用 CreatePushMessage 静态方法
        var message = Message.CreatePushMessage(
            request.Title,
            request.Content, userId,
            request.Priority
        );

        await _messageRepository.AddAsync(message);

        // 创建接收者记录
        var recipients = userList.Select(u => MessageRecipient.Create(
            message.Id,
            u.Id,
            u.RealName ?? u.UserName
        )).ToList();

        foreach (var recipient in recipients)
        {
            await _recipientRepository.AddAsync(recipient);
        }

        await _messageRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功发送 {recipients.Count} 条系统消息" };
    }

    /// <summary>
    /// 标记消息为已读
    /// </summary>
    public async Task<ApiRequestResult> MarkAsReadAsync(Guid id, Guid userId)
    {
        var recipients = await _recipientRepository.GetListAsync(
            r => r.MessageId == id && r.RecipientId == userId && !r.IsDeleted);
        var recipient = recipients.FirstOrDefault();

        if (recipient is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        // 已撤回的消息不能标记为已读
        if (recipient.IsRevoked)
        {
            return new ApiRequestResult { Success = false, Message = "已撤回的消息不能标记为已读" };
        }

        recipient.MarkAsRead();
        await _recipientRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "已标记为已读" };
    }

    /// <summary>
    /// 标记消息为未读
    /// </summary>
    public async Task<ApiRequestResult> MarkAsUnreadAsync(Guid id, Guid userId)
    {
        var recipients = await _recipientRepository.GetListAsync(
            r => r.MessageId == id && r.RecipientId == userId && !r.IsDeleted);
        var recipient = recipients.FirstOrDefault();

        if (recipient is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        recipient.MarkAsUnread();
        await _recipientRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "已标记为未读" };
    }

    /// <summary>
    /// 批量标记为已读
    /// </summary>
    public async Task<ApiRequestResult> BatchMarkAsReadAsync(List<Guid> ids, Guid userId)
    {
        if (ids is null || ids.Count == 0)
        {
            return new ApiRequestResult { Success = false, Message = "请选择要标记的消息" };
        }

        var recipients = await _recipientRepository.GetListAsync(
            r => ids.Contains(r.MessageId) && r.RecipientId == userId && !r.IsDeleted);

        if (recipients is null || !recipients.Any())
        {
            return new ApiRequestResult { Success = false, Message = "未找到要标记的消息" };
        }

        // 过滤掉已撤回的消息
        var validRecipients = recipients.Where(r => !r.IsRevoked).ToList();

        if (!validRecipients.Any())
        {
            return new ApiRequestResult { Success = false, Message = "选中的消息已撤回，无法标记为已读" };
        }

        foreach (var recipient in validRecipients)
        {
            recipient.MarkAsRead();
        }

        await _recipientRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功标记 {validRecipients.Count} 条消息为已读" };
    }

    /// <summary>
    /// 标记所有消息为已读
    /// </summary>
    public async Task<ApiRequestResult> MarkAllAsReadAsync(Guid userId)
    {
        var recipients = await _recipientRepository.GetListAsync(
            r => r.RecipientId == userId && !r.IsDeleted && !r.IsRead && !r.IsRevoked);

        if (recipients is null || !recipients.Any())
        {
            return new ApiRequestResult { Success = true, Message = "没有未读消息" };
        }

        foreach (var recipient in recipients)
        {
            recipient.MarkAsRead();
        }

        await _recipientRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功标记 {recipients.Count()} 条消息为已读" };
    }

    /// <summary>
    /// 删除消息（软删除）
    /// </summary>
    public async Task<ApiRequestResult> DeleteMessageAsync(Guid id, Guid userId)
    {
        var recipients = await _recipientRepository.GetListAsync(
            r => r.MessageId == id && r.RecipientId == userId && !r.IsDeleted);
        var recipient = recipients.FirstOrDefault();

        if (recipient is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        recipient.Delete();
        await _recipientRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "删除成功" };
    }

    /// <summary>
    /// 批量删除消息
    /// </summary>
    public async Task<ApiRequestResult> BatchDeleteMessagesAsync(List<Guid> ids, Guid userId)
    {
        if (ids is null || ids.Count == 0)
        {
            return new ApiRequestResult { Success = false, Message = "请选择要删除的消息" };
        }

        var recipients = await _recipientRepository.GetListAsync(
            r => ids.Contains(r.MessageId) && r.RecipientId == userId && !r.IsDeleted);

        if (recipients is null || !recipients.Any())
        {
            return new ApiRequestResult { Success = false, Message = "未找到要删除的消息" };
        }

        foreach (var recipient in recipients)
        {
            recipient.Delete();
        }

        await _recipientRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功删除 {recipients.Count()} 条消息" };
    }

    /// <summary>
    /// 获取当前用户的消息统计
    /// </summary>
    public async Task<ApiRequestResult> GetStatisticsAsync(Guid userId)
    {
        var recipients = await _recipientRepository.GetListAsync(
            r => r.RecipientId == userId && !r.IsDeleted);

        var recipientList = recipients.ToList();
        var messageIds = recipientList.Select(r => r.MessageId).ToList();
        var messages = await _messageRepository.GetListAsync(m => messageIds.Contains(m.Id));
        var messageList = messages.ToList();

        var statistics = new MessageStatisticsDto
        {
            TotalCount = recipientList.Count,
            UnreadCount = recipientList.Count(r => !r.IsRead),
            SystemMessageCount = messageList.Count(m => m.MessageType == MessageTypeConst.System),
            UserMessageCount = messageList.Count(m => m.MessageType == MessageTypeConst.User)
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = statistics };
    }

    /// <summary>
    /// 获取未读消息数量
    /// </summary>
    public async Task<ApiRequestResult> GetUnreadCountAsync(Guid userId)
    {
        var count = await _recipientRepository.CountAsync(
            r => r.RecipientId == userId && !r.IsDeleted && !r.IsRead);

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = count };
    }

    /// <summary>
    /// 更新消息内容（仅支持未读且未推送的消息）
    /// </summary>
    public async Task<ApiRequestResult> UpdateMessageAsync(Guid id, Guid userId, UpdateMessageRequest request)
    {
        var message = await _messageRepository.FindAsync(id);
        if (message is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        // 已推送的消息不能修改
        if (message.IsPushed)
        {
            return new ApiRequestResult { Success = false, Message = "已推送的消息不能修改" };
        }

        message.Update(request.Title, request.Content, request.Priority);
        await _messageRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "消息更新成功" };
    }

    /// <summary>
    /// 推送系统消息给所有用户
    /// </summary>
    public async Task<ApiRequestResult> PushMessageToAllAsync(PushMessageRequest request, Guid userId)
    {
        // 验证用户是否具有管理员权限
        var userRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == userId);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        
        var roles = await _roleRepository.GetListAsync(r => roleIds.Contains(r.Id));
        var roleList = roles.ToList();
        
        // 检查是否有管理员权限（包括SUPER_ADMIN或其他管理员角色）
        var isAdmin = roleList.Any(r => r.Code == "SUPER_ADMIN" || r.Code.Contains("ADMIN") || r.Code.Contains("admin"));
        if (!isAdmin)
        {
            return new ApiRequestResult { Success = false, Message = "无权推送系统消息" };
        }

        // 获取所有启用的用户
        var users = await _userRepository.GetListAsync(u => u.Status == 1);
        var userList = users.ToList();

        if (!userList.Any())
        {
            return new ApiRequestResult { Success = false, Message = "没有可推送的用户" };
        }

        // 创建消息 - 使用 CreatePushMessage 静态方法
        var message = Message.CreatePushMessage(
            request.Title,
            request.Content,  userId,
            request.Priority
        );

        await _messageRepository.AddAsync(message);

        // 创建接收者记录
        var recipients = userList.Select(u => MessageRecipient.Create(
            message.Id,
            u.Id,
            u.RealName ?? u.UserName
        )).ToList();

        foreach (var recipient in recipients)
        {
            await _recipientRepository.AddAsync(recipient);
        }

        await _messageRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功推送 {recipients.Count} 条系统消息" };
    }
    
    /// <summary>
    /// 推送系统消息给所有用户（兼容旧方法，不推荐使用）
    /// </summary>
    public async Task<ApiRequestResult> PushMessageToAllAsync(PushMessageRequest request)
    {
        // 为保持向后兼容，这里返回错误信息，因为需要userId参数
        return new ApiRequestResult { Success = false, Message = "缺少用户ID参数，无法验证权限" };
    }

    /// <summary>
    /// 推送系统消息给指定角色的用户
    /// </summary>
    public async Task<ApiRequestResult> PushMessageToRoleAsync(PushMessageToRoleRequest request, Guid userId)
    {
        // 验证用户是否具有管理员权限
        var userRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == userId);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        
        var roles = await _roleRepository.GetListAsync(r => roleIds.Contains(r.Id));
        var roleList = roles.ToList();
        
        // 检查是否有管理员权限（包括SUPER_ADMIN或其他管理员角色）
        var isAdmin = roleList.Any(r => r.Code == "SUPER_ADMIN" || r.Code.Contains("ADMIN") || r.Code.Contains("admin"));
        if (!isAdmin)
        {
            return new ApiRequestResult { Success = false, Message = "无权推送系统消息" };
        }

        if (request.RoleIds is null || request.RoleIds.Count == 0)
        {
            return new ApiRequestResult { Success = false, Message = "请选择角色" };
        }

        // 获取指定角色的用户ID
        var userRolesForTarget = await _userRoleRepository.GetListAsync(
            ur => request.RoleIds.Contains(ur.RoleId));
        var userRoleList = userRolesForTarget.ToList();

        if (!userRoleList.Any())
        {
            return new ApiRequestResult { Success = false, Message = "所选角色没有用户" };
        }

        // 获取用户信息
        var userIds = userRoleList.Select(ur => ur.UserId).Distinct().ToList();
        var users = await _userRepository.GetListAsync(
            u => userIds.Contains(u.Id) && u.Status == 1);
        var userList = users.ToList();

        if (!userList.Any())
        {
            return new ApiRequestResult { Success = false, Message = "所选角色没有启用的用户" };
        }

        // 创建消息 - 使用 CreatePushMessage 静态方法
        var message = Message.CreatePushMessage(
            request.Title,
            request.Content, userId,
            request.Priority
        );

        await _messageRepository.AddAsync(message);

        // 创建接收者记录
        var recipients = userList.Select(u => MessageRecipient.Create(
            message.Id,
            u.Id,
            u.RealName ?? u.UserName
        )).ToList();

        foreach (var recipient in recipients)
        {
            await _recipientRepository.AddAsync(recipient);
        }

        await _messageRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功推送 {recipients.Count} 条系统消息" };
    }
    
    /// <summary>
    /// 推送系统消息给指定角色的用户（兼容旧方法，不推荐使用）
    /// </summary>
    public async Task<ApiRequestResult> PushMessageToRoleAsync(PushMessageToRoleRequest request)
    {
        // 为保持向后兼容，这里返回错误信息，因为需要userId参数
        return new ApiRequestResult { Success = false, Message = "缺少用户ID参数，无法验证权限" };
    }

    /// <summary>
    /// 推送已有消息给其他用户
    /// </summary>
    public async Task<ApiRequestResult> PushExistingMessageAsync(Guid messageId, Guid userId, PushExistingMessageRequest request)
    {
        // 获取原消息
        var originalMessage = await _messageRepository.FindAsync(messageId);
        if (originalMessage is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        // 验证推送权限：当前用户必须是原始消息的发送者或者是管理员
        // 如果原始消息没有发送者（系统消息），则需要管理员权限
        if (originalMessage.SenderId.HasValue)
        {
            // 用户消息：只有原始发送者可以推送
            if (originalMessage.SenderId != userId)
            {
                return new ApiRequestResult { Success = false, Message = "无权推送此消息" };
            }
        }
        else
        {
            // 系统消息：需要管理员权限
            // 检查用户是否具有管理员权限
            var userRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == userId);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            
            var roles = await _roleRepository.GetListAsync(r => roleIds.Contains(r.Id));
            var roleList = roles.ToList();
            
            // 检查是否有管理员权限（包括SUPER_ADMIN或其他管理员角色）
            var isAdmin = roleList.Any(r => r.Code == "SUPER_ADMIN" || r.Code.Contains("ADMIN") || r.Code.Contains("admin"));
            if (!isAdmin)
            {
                return new ApiRequestResult { Success = false, Message = "无权推送系统消息" };
            }
        }

        List<User> targetUsers = new List<User>();

        switch (request.PushType)
        {
            case "all":
                var allUsers = await _userRepository.GetListAsync(u => u.Status == 1);
                targetUsers = allUsers.ToList();
                break;

            case "role":
                if (request.RoleIds is null || request.RoleIds.Count == 0)
                {
                    return new ApiRequestResult { Success = false, Message = "请选择角色" };
                }
                var userRoles = await _userRoleRepository.GetListAsync(ur => request.RoleIds.Contains(ur.RoleId));
                var userIds = userRoles.Select(ur => ur.UserId).Distinct().ToList();
                var roleUsers = await _userRepository.GetListAsync(u => userIds.Contains(u.Id) && u.Status == 1);
                targetUsers = roleUsers.ToList();
                break;

            case "user":
                if (request.UserIds is null || request.UserIds.Count == 0)
                {
                    return new ApiRequestResult { Success = false, Message = "请选择用户" };
                }
                var selectedUsers = await _userRepository.GetListAsync(u => request.UserIds.Contains(u.Id) && u.Status == 1);
                targetUsers = selectedUsers.ToList();
                break;

            default:
                return new ApiRequestResult { Success = false, Message = "无效的推送类型" };
        }

        if (!targetUsers.Any())
        {
            return new ApiRequestResult { Success = false, Message = "没有可推送的用户" };
        }

        // 排除已接收的用户
        var existingRecipients = await _recipientRepository.GetListAsync(r => r.MessageId == messageId);
        var existingUserIds = existingRecipients.Select(r => r.RecipientId).ToHashSet();
        targetUsers = targetUsers.Where(u => !existingUserIds.Contains(u.Id)).ToList();

        if (!targetUsers.Any())
        {
            return new ApiRequestResult { Success = false, Message = "所有目标用户已接收此消息" };
        }

        // 创建接收者记录
        var recipients = targetUsers.Select(u => MessageRecipient.Create(
            messageId,
            u.Id,
            u.RealName ?? u.UserName
        )).ToList();

        // 如果原消息已撤回，新接收人记录也应该标记为撤回
        if (originalMessage.IsRevoked)
        {
            foreach (var recipient in recipients)
            {
                recipient.Revoke();
            }
        }

        foreach (var recipient in recipients)
        {
            await _recipientRepository.AddAsync(recipient);
        }

        // 标记原消息为已推送
        originalMessage.MarkAsPushed();

        await _messageRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功推送 {recipients.Count} 条消息" };
    }

    #region 新增方法

    /// <summary>
    /// 获取消息详情（包含接收者列表）- 管理员查看
    /// </summary>
    public async Task<ApiRequestResult> GetMessageDetailAsync(Guid messageId)
    {
        var message = await _messageRepository.FindAsync(messageId);
        if (message is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        // 获取接收者列表
        var recipients = await _recipientRepository.GetListAsync(r => r.MessageId == messageId);
        var recipientList = recipients.ToList();

        var detail = new MessageDetailDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderName = message.SenderName,
            Title = message.Title,
            Content = message.Content,
            MessageType = message.MessageType,
            Priority = message.Priority,
            IsPushed = message.IsPushed,
            PushedTime = message.PushedTime,
            CreatedAt = message.CreatedAt,
            Recipients = recipientList.Select(r => new MessageRecipientDto
            {
                Id = r.Id,
                MessageId = r.MessageId,
                RecipientId = r.RecipientId,
                RecipientName = r.RecipientName,
                IsRead = r.IsRead,
                ReadTime = r.ReadTime,
                IsDeleted = r.IsDeleted,
                CreatedAt = r.CreatedAt,
                IsRevoked = r.IsRevoked,
                RevokedTime = r.RevokedTime
            }).ToList(),
            RecipientCount = recipientList.Count,
            ReadCount = recipientList.Count(r => r.IsRead),
            UnreadCount = recipientList.Count(r => !r.IsRead)
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = detail };
    }

    /// <summary>
    /// 获取消息接收者列表
    /// </summary>
    public async Task<ApiRequestResult> GetMessageRecipientsAsync(Guid messageId, MessageRecipientQueryRequest request)
    {
        var skipCount = (request.PageNumber - 1) * request.PageSize;

        var predicate = PredicateBuilder.True<MessageRecipient>()
            .And(r => r.MessageId == messageId);

        if (request.IsRead.HasValue)
        {
            predicate = predicate.And(r => r.IsRead == request.IsRead.Value);
        }

        if (request.IsDeleted.HasValue)
        {
            predicate = predicate.And(r => r.IsDeleted == request.IsDeleted.Value);
        }

        var total = await _recipientRepository.CountAsync(predicate);
        var recipients = await _recipientRepository.GetListAsync(
            predicate,
            q => q.OrderByDescending(r => r.CreatedAt),
            skipCount,
            request.PageSize
        );

        var recipientDtos = recipients.Select(r => new MessageRecipientDto
        {
            Id = r.Id,
            MessageId = r.MessageId,
            RecipientId = r.RecipientId,
            RecipientName = r.RecipientName,
            IsRead = r.IsRead,
            ReadTime = r.ReadTime,
            IsDeleted = r.IsDeleted,
            CreatedAt = r.CreatedAt,
            IsRevoked = r.IsRevoked,
            RevokedTime = r.RevokedTime
        }).ToList();

        var pagedResult = new PagedResult<MessageRecipientDto>
        {
            List = recipientDtos,
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = pagedResult };
    }

    /// <summary>
    /// 获取用户的消息列表（基于 MessageRecipient）
    /// </summary>
    public async Task<ApiRequestResult> GetUserMessagesAsync(Guid userId, MessageQueryRequest request)
    {
        var skipCount = (request.PageNumber - 1) * request.PageSize;

        // 构建查询条件
        var predicate = PredicateBuilder.True<MessageRecipient>()
            .And(r => r.RecipientId == userId)
            .And(r => !r.IsDeleted);

        if (request.OnlyUnread is true)
        {
            predicate = predicate.And(r => !r.IsRead);
        }

        // 获取接收者记录
        var total = await _recipientRepository.CountAsync(predicate);
        var recipients = await _recipientRepository.GetListAsync(
            predicate,
            q => q.OrderByDescending(r => r.IsRead).ThenByDescending(r => r.CreatedAt),
            skipCount,
            request.PageSize
        );

        var recipientList = recipients.ToList();
        if (!recipientList.Any())
        {
            return new ApiRequestResult
            {
                Success = true,
                Message = "操作成功",
                Data = new PagedResult<UserMessageDto>
                {
                    List = new List<UserMessageDto>(),
                    Total = 0,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                }
            };
        }

        // 获取消息详情
        var messageIds = recipientList.Select(r => r.MessageId).ToList();
        var messages = await _messageRepository.GetListAsync(m => messageIds.Contains(m.Id));
        var messageDict = messages.ToDictionary(m => m.Id);

        // 筛选消息
        var filteredRecipientIds = new List<Guid>();
        foreach (var recipient in recipientList)
        {
            if (!messageDict.TryGetValue(recipient.MessageId, out var message))
                continue;

            if (!string.IsNullOrEmpty(request.MessageType) && message.MessageType != request.MessageType)
                continue;

            if (!string.IsNullOrEmpty(request.Priority) && message.Priority != request.Priority)
                continue;

            if (!string.IsNullOrEmpty(request.Keyword) && !message.Title.Contains(request.Keyword))
                continue;

            filteredRecipientIds.Add(recipient.Id);
        }

        // 获取所有消息的接收者统计（用于判断是否有其他用户已读）
        var allRecipients = await _recipientRepository.GetListAsync(
            r => messageIds.Contains(r.MessageId) && r.IsRead && !r.IsDeleted);
        var readRecipientDict = allRecipients
            .GroupBy(r => r.MessageId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => r.RecipientId).ToHashSet()
            );

        // 构建结果
        var userMessages = recipientList
            .Where(r => filteredRecipientIds.Contains(r.Id))
            .Select(r =>
            {
                var message = messageDict.GetValueOrDefault(r.MessageId);
                // 检查是否有其他用户已读：同一条消息有其他接收者且已读
                bool hasBeenReadByOthers = false;
                if (message is not null && readRecipientDict.TryGetValue(message.Id, out var readRecipientIds))
                {
                    // 除了当前用户外，还有其他已读的接收者
                    hasBeenReadByOthers = readRecipientIds.Any(id => id != userId);
                }

                return new UserMessageDto
                {
                    RecipientId = r.Id,
                    MessageId = r.MessageId,
                    SenderId = message?.SenderId,
                    SenderName = message?.SenderName,
                    Title = message?.Title ?? string.Empty,
                    Content = message?.Content ?? string.Empty,
                    MessageType = message?.MessageType ?? string.Empty,
                    Priority = message?.Priority ?? string.Empty,
                    IsRead = r.IsRead,
                    ReadTime = r.ReadTime,
                    IsPushed = message?.IsPushed ?? false,
                    CreatedAt = message?.CreatedAt ?? r.CreatedAt,
                    HasBeenReadByOthers = hasBeenReadByOthers,
                    IsRevoked = r.IsRevoked,
                    RevokedTime = r.RevokedTime
                };
            }).ToList();

        var pagedResult = new PagedResult<UserMessageDto>
        {
            List = userMessages,
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = pagedResult };
    }

    /// <summary>
    /// 获取用户消息详情
    /// </summary>
    public async Task<ApiRequestResult> GetUserMessageByIdAsync(Guid recipientId, Guid userId)
    {
        var recipient = await _recipientRepository.FindAsync(recipientId);
        if (recipient is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        if (recipient.RecipientId != userId)
        {
            return new ApiRequestResult { Success = false, Message = "无权查看此消息" };
        }

        if (recipient.IsDeleted)
        {
            return new ApiRequestResult { Success = false, Message = "消息已被删除" };
        }

        var message = await _messageRepository.FindAsync(recipient.MessageId);
        if (message is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        // 检查是否有其他用户已读：查询同一条消息的其他接收者记录
        var otherRecipients = await _recipientRepository.GetListAsync(
            r => r.MessageId == message.Id && r.RecipientId != userId && r.IsRead && !r.IsDeleted);
        bool hasBeenReadByOthers = otherRecipients.Any();

        var userMessage = new UserMessageDto
        {
            RecipientId = recipient.Id,
            MessageId = message.Id,
            SenderId = message.SenderId,
            SenderName = message.SenderName,
            Title = message.Title,
            Content = message.Content,
            MessageType = message.MessageType,
            Priority = message.Priority,
            IsRead = recipient.IsRead,
            ReadTime = recipient.ReadTime,
            IsPushed = message.IsPushed,
            CreatedAt = message.CreatedAt,
            HasBeenReadByOthers = hasBeenReadByOthers,
            IsRevoked = recipient.IsRevoked,
            RevokedTime = recipient.RevokedTime
        };

        // 自动标记为已读（已撤回的消息不自动标记）
        if (!recipient.IsRead && !recipient.IsRevoked)
        {
            recipient.MarkAsRead();
            await _recipientRepository.SaveChangesAsync();
        }

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = userMessage };
    }

    /// <summary>
    /// 标记用户消息为已读
    /// </summary>
    public async Task<ApiRequestResult> MarkUserMessageAsReadAsync(Guid recipientId, Guid userId)
    {
        var recipient = await _recipientRepository.FindAsync(recipientId);
        if (recipient is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        if (recipient.RecipientId != userId)
        {
            return new ApiRequestResult { Success = false, Message = "无权操作此消息" };
        }

        // 已撤回的消息不能标记为已读
        if (recipient.IsRevoked)
        {
            return new ApiRequestResult { Success = false, Message = "已撤回的消息不能标记为已读" };
        }

        recipient.MarkAsRead();
        await _recipientRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "已标记为已读" };
    }

    /// <summary>
    /// 撤回消息（强制撤回已发出的通知）
    /// </summary>
    public async Task<ApiRequestResult> RevokeMessageAsync(Guid messageId, Guid userId)
    {
        // 查找消息
        var message = await _messageRepository.FindAsync(messageId);
        if (message is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        // 检查是否是发送者撤回
        if (message.SenderId != userId)
        {
            return new ApiRequestResult { Success = false, Message = "只有发送者才能撤回消息" };
        }

        // 已撤回的消息不能重复撤回
        if (message.IsRevoked)
        {
            return new ApiRequestResult { Success = false, Message = "消息已被撤回" };
        }

        // 撤回消息
        message.Revoke(userId);

        // 撤回所有接收者记录
        var recipients = await _recipientRepository.GetListAsync(r => r.MessageId == messageId && !r.IsDeleted);
        var recipientList = recipients.ToList();

        foreach (var recipient in recipientList)
        {
            recipient.Revoke();
        }

        await _messageRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "消息已撤回" };
    }

    /// <summary>
    /// 批量撤回消息
    /// </summary>
    public async Task<ApiRequestResult> BatchRevokeMessagesAsync(List<Guid> messageIds, Guid userId)
    {
        if (messageIds is null || messageIds.Count == 0)
        {
            return new ApiRequestResult { Success = false, Message = "请选择要撤回的消息" };
        }

        var messages = await _messageRepository.GetListAsync(m => messageIds.Contains(m.Id));
        var messageList = messages.ToList();

        var revokedCount = 0;
        foreach (var message in messageList)
        {
            // 只有发送者才能撤回
            if (message.SenderId != userId || message.IsRevoked)
            {
                continue;
            }

            message.Revoke(userId);
            revokedCount++;

            // 撤回所有接收者记录
            var recipients = await _recipientRepository.GetListAsync(r => r.MessageId == message.Id && !r.IsDeleted);
            var recipientList = recipients.ToList();

            foreach (var recipient in recipientList)
            {
                recipient.Revoke();
            }
        }

        await _messageRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功撤回 {revokedCount} 条消息" };
    }

    /// <summary>
    /// 删除用户消息
    /// </summary>
    public async Task<ApiRequestResult> DeleteUserMessageAsync(Guid recipientId, Guid userId)
    {
        var recipient = await _recipientRepository.FindAsync(recipientId);
        if (recipient is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        if (recipient.RecipientId != userId)
        {
            return new ApiRequestResult { Success = false, Message = "无权删除此消息" };
        }

        if (recipient.IsDeleted)
        {
            return new ApiRequestResult { Success = false, Message = "消息已被删除" };
        }

        // 检查是否有其他用户已读
        var otherRecipients = await _recipientRepository.GetListAsync(
            r => r.MessageId == recipient.MessageId && r.RecipientId != userId && r.IsRead && !r.IsDeleted);
        if (otherRecipients.Any())
        {
            return new ApiRequestResult { Success = false, Message = "该消息已有其他用户阅读，无法删除" };
        }

        recipient.Delete();
        await _recipientRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "删除成功" };
    }

    /// <summary>
    /// 批量删除用户消息
    /// </summary>
    public async Task<ApiRequestResult> BatchDeleteUserMessagesAsync(List<Guid> recipientIds, Guid userId)
    {
        if (recipientIds is null || recipientIds.Count == 0)
        {
            return new ApiRequestResult { Success = false, Message = "请选择要删除的消息" };
        }

        var recipients = await _recipientRepository.GetListAsync(
            r => recipientIds.Contains(r.Id) && r.RecipientId == userId && !r.IsDeleted);

        if (recipients is null || !recipients.Any())
        {
            return new ApiRequestResult { Success = false, Message = "未找到要删除的消息" };
        }

        // 检查是否有消息已被其他用户阅读
        var messageIds = recipients.Select(r => r.MessageId).Distinct().ToList();
        var otherReadRecipients = await _recipientRepository.GetListAsync(
            r => messageIds.Contains(r.MessageId) && r.RecipientId != userId && r.IsRead && !r.IsDeleted);
        
        if (otherReadRecipients.Any())
        {
            var blockedMessageIds = otherReadRecipients.Select(r => r.MessageId).Distinct().ToHashSet();
            return new ApiRequestResult { 
                Success = false, 
                Message = $"有 {blockedMessageIds.Count} 条消息已被其他用户阅读，无法删除" 
            };
        }

        foreach (var recipient in recipients)
        {
            recipient.Delete();
        }

        await _recipientRepository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功删除 {recipients.Count()} 条消息" };
    }

    /// <summary>
    /// 获取所有消息列表（管理员）
    /// </summary>
    public async Task<ApiRequestResult> GetAllMessagesAsync(MessageQueryRequest request)
    {
        var skipCount = (request.PageNumber - 1) * request.PageSize;

        var predicate = PredicateBuilder.True<Message>();

        if (!string.IsNullOrEmpty(request.MessageType))
        {
            predicate = predicate.And(m => m.MessageType == request.MessageType);
        }

        if (!string.IsNullOrEmpty(request.Priority))
        {
            predicate = predicate.And(m => m.Priority == request.Priority);
        }

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            predicate = predicate.And(m => m.Title.Contains(request.Keyword));
        }

        if (!string.IsNullOrEmpty(request.SenderName))
        {
            predicate = predicate.And(m => m.SenderName != null && m.SenderName.Contains(request.SenderName));
        }

        var total = await _messageRepository.CountAsync(predicate);
        var messages = await _messageRepository.GetListAsync(
            predicate,
            q => q.OrderByDescending(m => m.CreatedAt),
            skipCount,
            request.PageSize
        );

        var messageList = messages.ToList();
        var messageIds = messageList.Select(m => m.Id).ToList();

        // 获取每个消息的接收者统计
        var recipients = await _recipientRepository.GetListAsync(r => messageIds.Contains(r.MessageId));
        var recipientStats = recipients.GroupBy(r => r.MessageId)
            .ToDictionary(g => g.Key, g => new { Total = g.Count(), Read = g.Count(r => r.IsRead) });

        var messageDtos = messageList.Select(m =>
        {
            var stats = recipientStats.GetValueOrDefault(m.Id);
            return new MessageDetailDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.SenderName,
                Title = m.Title,
                Content = m.Content,
                MessageType = m.MessageType,
                Priority = m.Priority,
                IsPushed = m.IsPushed,
                PushedTime = m.PushedTime,
                CreatedAt = m.CreatedAt,
                RecipientCount = stats?.Total ?? 0,
                ReadCount = stats?.Read ?? 0,
                UnreadCount = (stats?.Total ?? 0) - (stats?.Read ?? 0)
            };
        }).ToList();

        var pagedResult = new PagedResult<MessageDetailDto>
        {
            List = messageDtos,
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = pagedResult };
    }

    #endregion
}