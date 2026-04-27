using DDDProject.Application.DTOs;
using DDDProject.Domain.Entities;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 站内信服务接口
/// </summary>
public interface IMessageService : IApplicationService
{
    /// <summary>
    /// 获取当前用户的消息列表（分页、支持筛选）
    /// </summary>
    Task<ApiRequestResult> GetMessagesAsync(Guid userId, MessageQueryRequest request);

    /// <summary>
    /// 获取消息详情
    /// </summary>
    Task<ApiRequestResult> GetMessageByIdAsync(Guid id, Guid userId);

    /// <summary>
    /// 发送用户消息
    /// </summary>
    Task<ApiRequestResult> SendMessageAsync(Guid senderId, string senderName, CreateMessageRequest request);

    /// <summary>
    /// 发送系统消息给指定用户
    /// </summary>
    Task<ApiRequestResult> SendSystemMessageAsync(Guid receiverId, string receiverName, string title, string content, string priority = MessagePriority.Normal);

    /// <summary>
    /// 批量发送系统消息
    /// </summary>
    Task<ApiRequestResult> BatchSendSystemMessageAsync(BatchSendMessageRequest request, Guid userId);

    /// <summary>
    /// 标记消息为已读
    /// </summary>
    Task<ApiRequestResult> MarkAsReadAsync(Guid id, Guid userId);

    /// <summary>
    /// 标记消息为未读
    /// </summary>
    Task<ApiRequestResult> MarkAsUnreadAsync(Guid id, Guid userId);

    /// <summary>
    /// 批量标记为已读
    /// </summary>
    Task<ApiRequestResult> BatchMarkAsReadAsync(List<Guid> ids, Guid userId);

    /// <summary>
    /// 标记所有消息为已读
    /// </summary>
    Task<ApiRequestResult> MarkAllAsReadAsync(Guid userId);

    /// <summary>
    /// 删除消息（软删除）
    /// </summary>
    Task<ApiRequestResult> DeleteMessageAsync(Guid id, Guid userId);

    /// <summary>
    /// 批量删除消息
    /// </summary>
    Task<ApiRequestResult> BatchDeleteMessagesAsync(List<Guid> ids, Guid userId);

    /// <summary>
    /// 获取当前用户的消息统计
    /// </summary>
    Task<ApiRequestResult> GetStatisticsAsync(Guid userId);

    /// <summary>
    /// 获取未读消息数量
    /// </summary>
    Task<ApiRequestResult> GetUnreadCountAsync(Guid userId);

    /// <summary>
    /// 更新消息内容（仅支持未读消息）
    /// </summary>
    Task<ApiRequestResult> UpdateMessageAsync(Guid id, Guid userId, UpdateMessageRequest request);

    /// <summary>
    /// 推送系统消息给所有用户
    /// </summary>
    Task<ApiRequestResult> PushMessageToAllAsync(PushMessageRequest request, Guid userId);

    /// <summary>
    /// 推送系统消息给指定角色的用户
    /// </summary>
    Task<ApiRequestResult> PushMessageToRoleAsync(PushMessageToRoleRequest request, Guid userId);

    /// <summary>
    /// 推送已有消息给其他用户
    /// </summary>
    Task<ApiRequestResult> PushExistingMessageAsync(Guid messageId, Guid userId, PushExistingMessageRequest request);

    /// <summary>
    /// 获取消息详情（包含接收者列表）- 管理员查看
    /// </summary>
    Task<ApiRequestResult> GetMessageDetailAsync(Guid messageId);

    /// <summary>
    /// 获取消息接收者列表
    /// </summary>
    Task<ApiRequestResult> GetMessageRecipientsAsync(Guid messageId, MessageRecipientQueryRequest request);

    /// <summary>
    /// 获取用户的消息列表（基于 MessageRecipient）
    /// </summary>
    Task<ApiRequestResult> GetUserMessagesAsync(Guid userId, MessageQueryRequest request);

    /// <summary>
    /// 获取用户消息详情
    /// </summary>
    Task<ApiRequestResult> GetUserMessageByIdAsync(Guid recipientId, Guid userId);

    /// <summary>
    /// 标记用户消息为已读
    /// </summary>
    Task<ApiRequestResult> MarkUserMessageAsReadAsync(Guid recipientId, Guid userId);

    /// <summary>
    /// 删除用户消息
    /// </summary>
    Task<ApiRequestResult> DeleteUserMessageAsync(Guid recipientId, Guid userId);

    /// <summary>
    /// 批量删除用户消息
    /// </summary>
    Task<ApiRequestResult> BatchDeleteUserMessagesAsync(List<Guid> recipientIds, Guid userId);

    /// <summary>
    /// 获取所有消息列表（管理员）
    /// </summary>
    Task<ApiRequestResult> GetAllMessagesAsync(MessageQueryRequest request);

    /// <summary>
    /// 撤回消息（只有发送者可以撤回）
    /// </summary>
    Task<ApiRequestResult> RevokeMessageAsync(Guid messageId, Guid userId);

    /// <summary>
    /// 批量撤回消息
    /// </summary>
    Task<ApiRequestResult> BatchRevokeMessagesAsync(List<Guid> messageIds, Guid userId);
}