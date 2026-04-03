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
    private readonly IRepository<Message> _repository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<UserRole> _userRoleRepository;

    public MessageService(
        IRepository<Message> repository,
        IRepository<User> userRepository,
        IRepository<UserRole> userRoleRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
    }

    /// <summary>
    /// 获取当前用户的消息列表（分页、支持筛选）
    /// </summary>
    public async Task<ApiRequestResult> GetMessagesAsync(Guid userId, MessageQueryRequest request)
    {
        var skipCount = (request.PageNumber - 1) * request.PageSize;

        // 构建查询条件：只查询当前用户的消息，且未删除
        var predicate = PredicateBuilder.True<Message>()
            .And(m => m.ReceiverId == userId)
            .And(m => !m.IsDeleted);

        // 添加筛选条件
        if (request.OnlyUnread is true)
        {
            predicate = predicate.And(m => !m.IsRead);
        }

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

        // 获取总数
        var total = await _repository.CountAsync(predicate);

        // 获取列表（按创建时间倒序，优先级高的在前）
        var messages = await _repository.GetListAsync(
            predicate,
            q => q.OrderByDescending(m => m.Priority).ThenByDescending(m => m.CreatedAt),
            skipCount,
            request.PageSize
        );

        // 转换为 DTO
        var messageDtos = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderName = m.SenderName,
            ReceiverId = m.ReceiverId,
            ReceiverName = m.ReceiverName,
            Title = m.Title,
            Content = m.Content,
            MessageType = m.MessageType,
            Priority = m.Priority,
            IsRead = m.IsRead,
            ReadTime = m.ReadTime,
            IsPushed = m.IsPushed,
            CreatedAt = m.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<MessageDto>
        {
            List = messageDtos,
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = pagedResult };
    }

    /// <summary>
    /// 获取消息详情
    /// </summary>
    public async Task<ApiRequestResult> GetMessageByIdAsync(Guid id, Guid userId)
    {
        var message = await _repository.FindAsync(id);
        if (message is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        // 验证消息是否属于当前用户
        if (message.ReceiverId != userId)
        {
            return new ApiRequestResult { Success = false, Message = "无权查看此消息" };
        }

        // 验证消息是否已删除
        if (message.IsDeleted)
        {
            return new ApiRequestResult { Success = false, Message = "消息已被删除" };
        }

        var messageDto = new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderName = message.SenderName,
            ReceiverId = message.ReceiverId,
            ReceiverName = message.ReceiverName,
            Title = message.Title,
            Content = message.Content,
            MessageType = message.MessageType,
            Priority = message.Priority,
            IsRead = message.IsRead,
            ReadTime = message.ReadTime,
            IsPushed = message.IsPushed,
            CreatedAt = message.CreatedAt
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = messageDto };
    }

    /// <summary>
    /// 发送用户消息
    /// </summary>
    public async Task<ApiRequestResult> SendMessageAsync(Guid senderId, string senderName, CreateMessageRequest request)
    {
        var message = Message.CreateUserMessage(
            senderId,
            senderName,
            request.ReceiverId,
            request.ReceiverName,
            request.Title,
            request.Content,
            request.Priority
        );

        await _repository.AddAsync(message);
        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "消息发送成功", Data = message.Id };
    }

    /// <summary>
    /// 发送系统消息给指定用户
    /// </summary>
    public async Task<ApiRequestResult> SendSystemMessageAsync(Guid receiverId, string receiverName, string title, string content, string priority = MessagePriority.Normal)
    {
        var message = Message.CreateSystemMessage(
            receiverId,
            receiverName,
            title,
            content,
            priority
        );

        await _repository.AddAsync(message);
        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "系统消息发送成功", Data = message.Id };
    }

    /// <summary>
    /// 批量发送系统消息
    /// </summary>
    public async Task<ApiRequestResult> BatchSendSystemMessageAsync(BatchSendMessageRequest request)
    {
        if (request.ReceiverIds is null || request.ReceiverIds.Count == 0)
        {
            return new ApiRequestResult { Success = false, Message = "请选择接收者" };
        }

        var messages = new List<Message>();
        foreach (var receiverId in request.ReceiverIds)
        {
            var message = Message.CreateSystemMessage(
                receiverId,
                "",
                request.Title,
                request.Content,
                request.Priority
            );
            messages.Add(message);
        }

        foreach (var message in messages)
        {
            await _repository.AddAsync(message);
        }

        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功发送 {messages.Count} 条系统消息" };
    }

    /// <summary>
    /// 标记消息为已读
    /// </summary>
    public async Task<ApiRequestResult> MarkAsReadAsync(Guid id, Guid userId)
    {
        var message = await _repository.FindAsync(id);
        if (message is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        if (message.ReceiverId != userId)
        {
            return new ApiRequestResult { Success = false, Message = "无权操作此消息" };
        }

        message.MarkAsRead();
        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "已标记为已读" };
    }

    /// <summary>
    /// 标记消息为未读
    /// </summary>
    public async Task<ApiRequestResult> MarkAsUnreadAsync(Guid id, Guid userId)
    {
        var message = await _repository.FindAsync(id);
        if (message is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        if (message.ReceiverId != userId)
        {
            return new ApiRequestResult { Success = false, Message = "无权操作此消息" };
        }

        message.MarkAsUnread();
        await _repository.SaveChangesAsync();

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

        var messages = await _repository.GetListAsync(m => ids.Contains(m.Id) && m.ReceiverId == userId && !m.IsDeleted);
        if (messages is null || !messages.Any())
        {
            return new ApiRequestResult { Success = false, Message = "未找到要标记的消息" };
        }

        foreach (var message in messages)
        {
            message.MarkAsRead();
        }

        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功标记 {messages.Count()} 条消息为已读" };
    }

    /// <summary>
    /// 标记所有消息为已读
    /// </summary>
    public async Task<ApiRequestResult> MarkAllAsReadAsync(Guid userId)
    {
        var predicate = PredicateBuilder.True<Message>()
            .And(m => m.ReceiverId == userId)
            .And(m => !m.IsDeleted)
            .And(m => !m.IsRead);

        var messages = await _repository.GetListAsync(predicate);
        if (messages is null || !messages.Any())
        {
            return new ApiRequestResult { Success = true, Message = "没有未读消息" };
        }

        foreach (var message in messages)
        {
            message.MarkAsRead();
        }

        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功标记 {messages.Count()} 条消息为已读" };
    }

    /// <summary>
    /// 删除消息（软删除）
    /// </summary>
    public async Task<ApiRequestResult> DeleteMessageAsync(Guid id, Guid userId)
    {
        var message = await _repository.FindAsync(id);
        if (message is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        if (message.ReceiverId != userId)
        {
            return new ApiRequestResult { Success = false, Message = "无权删除此消息" };
        }

        message.Delete();
        await _repository.SaveChangesAsync();

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

        var messages = await _repository.GetListAsync(m => ids.Contains(m.Id) && m.ReceiverId == userId && !m.IsDeleted);
        if (messages is null || !messages.Any())
        {
            return new ApiRequestResult { Success = false, Message = "未找到要删除的消息" };
        }

        foreach (var message in messages)
        {
            message.Delete();
        }

        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功删除 {messages.Count()} 条消息" };
    }

    /// <summary>
    /// 获取当前用户的消息统计
    /// </summary>
    public async Task<ApiRequestResult> GetStatisticsAsync(Guid userId)
    {
        var predicate = PredicateBuilder.True<Message>()
            .And(m => m.ReceiverId == userId)
            .And(m => !m.IsDeleted);

        var messages = await _repository.GetListAsync(predicate);
        var messageList = messages.ToList();

        var statistics = new MessageStatisticsDto
        {
            TotalCount = messageList.Count,
            UnreadCount = messageList.Count(m => !m.IsRead),
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
        var predicate = PredicateBuilder.True<Message>()
            .And(m => m.ReceiverId == userId)
            .And(m => !m.IsDeleted)
            .And(m => !m.IsRead);

        var count = await _repository.CountAsync(predicate);

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = count };
    }

    /// <summary>
    /// 更新消息内容（仅支持未读且未推送的消息）
    /// </summary>
    public async Task<ApiRequestResult> UpdateMessageAsync(Guid id, Guid userId, UpdateMessageRequest request)
    {
        var message = await _repository.FindAsync(id);
        if (message is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        // 验证消息是否属于当前用户
        if (message.ReceiverId != userId)
        {
            return new ApiRequestResult { Success = false, Message = "无权修改此消息" };
        }

        // 只允许修改未读消息
        if (message.IsRead)
        {
            return new ApiRequestResult { Success = false, Message = "已读消息不能修改" };
        }

        // 已推送的消息不能修改
        if (message.IsPushed)
        {
            return new ApiRequestResult { Success = false, Message = "已推送的消息不能修改" };
        }

        message.Update(request.Title, request.Content, request.Priority);
        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "消息更新成功" };
    }

    /// <summary>
    /// 推送系统消息给所有用户
    /// </summary>
    public async Task<ApiRequestResult> PushMessageToAllAsync(PushMessageRequest request)
    {
        // 获取所有启用的用户
        var users = await _userRepository.GetListAsync(u => u.Status == 1);
        var userList = users.ToList();

        if (!userList.Any())
        {
            return new ApiRequestResult { Success = false, Message = "没有可推送的用户" };
        }

        var messages = new List<Message>();
        foreach (var user in userList)
        {
            var message = Message.CreateSystemMessage(
                user.Id,
                user.RealName ?? user.UserName,
                request.Title,
                request.Content,
                request.Priority
            );
            messages.Add(message);
        }

        foreach (var message in messages)
        {
            await _repository.AddAsync(message);
        }

        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功推送 {messages.Count} 条系统消息" };
    }

    /// <summary>
    /// 推送系统消息给指定角色的用户
    /// </summary>
    public async Task<ApiRequestResult> PushMessageToRoleAsync(PushMessageToRoleRequest request)
    {
        if (request.RoleIds is null || request.RoleIds.Count == 0)
        {
            return new ApiRequestResult { Success = false, Message = "请选择角色" };
        }

        // 获取指定角色的用户ID
        var userRoles = await _userRoleRepository.GetListAsync(
            ur => request.RoleIds.Contains(ur.RoleId)
        );
        var userRoleList = userRoles.ToList();

        if (!userRoleList.Any())
        {
            return new ApiRequestResult { Success = false, Message = "所选角色没有用户" };
        }

        // 获取用户信息
        var userIds = userRoleList.Select(ur => ur.UserId).Distinct().ToList();
        var users = await _userRepository.GetListAsync(
            u => userIds.Contains(u.Id) && u.Status == 1
        );
        var userList = users.ToList();

        if (!userList.Any())
        {
            return new ApiRequestResult { Success = false, Message = "所选角色没有启用的用户" };
        }

        var messages = new List<Message>();
        foreach (var user in userList)
        {
            var message = Message.CreateSystemMessage(
                user.Id,
                user.RealName ?? user.UserName,
                request.Title,
                request.Content,
                request.Priority
            );
            messages.Add(message);
        }

        foreach (var message in messages)
        {
            await _repository.AddAsync(message);
        }

        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功推送 {messages.Count} 条系统消息" };
    }

    /// <summary>
    /// 推送已有消息给其他用户
    /// </summary>
    public async Task<ApiRequestResult> PushExistingMessageAsync(Guid messageId, Guid userId, PushExistingMessageRequest request)
    {
        // 获取原消息
        var originalMessage = await _repository.FindAsync(messageId);
        if (originalMessage is null)
        {
            return new ApiRequestResult { Success = false, Message = "消息不存在" };
        }

        // 验证消息是否属于当前用户
        if (originalMessage.ReceiverId != userId)
        {
            return new ApiRequestResult { Success = false, Message = "无权推送此消息" };
        }

        List<User> targetUsers = new List<User>();

        switch (request.PushType)
        {
            case "all":
                // 推送给所有用户
                var allUsers = await _userRepository.GetListAsync(u => u.Status == 1);
                targetUsers = allUsers.ToList();
                break;

            case "role":
                // 推送给指定角色的用户
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
                // 推送给指定用户
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

        // 排除原消息接收者
        targetUsers = targetUsers.Where(u => u.Id != originalMessage.ReceiverId).ToList();

        if (!targetUsers.Any())
        {
            return new ApiRequestResult { Success = false, Message = "没有可推送的新用户" };
        }

        // 创建新消息
        var messages = new List<Message>();
        foreach (var user in targetUsers)
        {
            var message = Message.CreateSystemMessage(
                user.Id,
                user.RealName ?? user.UserName,
                originalMessage.Title,
                originalMessage.Content,
                originalMessage.Priority
            );
            messages.Add(message);
        }

        foreach (var message in messages)
        {
            await _repository.AddAsync(message);
        }

        // 标记原消息为已推送
        originalMessage.MarkAsPushed();

        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = $"成功推送 {messages.Count} 条消息" };
    }
}