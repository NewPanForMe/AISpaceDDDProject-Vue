import { http } from '@/utils/http'
import api from '@/api/index'
import type { PagedResult, MessageDto, UserMessageDto, MessageQueryRequest, CreateMessageRequest, BatchSendMessageRequest, MessageStatisticsDto, UpdateMessageRequest, PushMessageRequest, PushMessageToRoleRequest, PushExistingMessageRequest, MessageDetailDto, MessageRecipientDto, MessageRecipientQueryRequest } from '@/api/index'

// 获取消息列表（分页）- 返回用户消息视图
export const getMessages = (params?: MessageQueryRequest) => {
  return http.get<PagedResult<UserMessageDto>>(api.Message.GetMessagesAsync, { params })
}

// 根据ID获取消息详情
export const getMessageById = (id: string) => {
  return http.get<MessageDto>(api.Message.GetMessageByIdAsync, { params: { id } })
}

// 根据接收者记录ID获取用户消息详情
export const getUserMessageById = (recipientId: string) => {
  return http.get<UserMessageDto>(api.Message.GetUserMessageByIdAsync, { params: { recipientId } })
}

// 标记用户消息为已读（通过接收者记录ID）
export const markUserMessageAsRead = (recipientId: string) => {
  return http.post(api.Message.MarkUserMessageAsReadAsync, {}, { params: { recipientId } })
}

// 删除用户消息（通过接收者记录ID）
export const deleteUserMessage = (recipientId: string) => {
  return http.delete(api.Message.DeleteUserMessageAsync, { params: { recipientId } })
}

// 批量删除用户消息（通过接收者记录ID列表）
export const batchDeleteUserMessages = (recipientIds: string[]) => {
  return http.delete(api.Message.BatchDeleteUserMessagesAsync, { data: recipientIds })
}

// 发送用户消息
export const sendMessage = (data: CreateMessageRequest) => {
  return http.post(api.Message.SendMessageAsync, data)
}

// 发送系统消息
export const sendSystemMessage = (data: CreateMessageRequest) => {
  return http.post(api.Message.SendSystemMessageAsync, data)
}

// 批量发送系统消息
export const batchSendSystemMessage = (data: BatchSendMessageRequest) => {
  return http.post(api.Message.BatchSendSystemMessageAsync, data)
}

// 标记消息为已读
export const markAsRead = (id: string) => {
  return http.post(api.Message.MarkAsReadAsync, {}, { params: { id } })
}

// 标记消息为未读
export const markAsUnread = (id: string) => {
  return http.post(api.Message.MarkAsUnreadAsync, {}, { params: { id } })
}

// 批量标记为已读
export const batchMarkAsRead = (ids: string[]) => {
  return http.post(api.Message.BatchMarkAsReadAsync, ids)
}

// 标记所有消息为已读
export const markAllAsRead = () => {
  return http.post(api.Message.MarkAllAsReadAsync)
}

// 删除消息
export const deleteMessage = (id: string) => {
  return http.delete(api.Message.DeleteMessageAsync, { params: { id } })
}

// 批量删除消息
export const batchDeleteMessages = (ids: string[]) => {
  return http.delete(api.Message.BatchDeleteMessagesAsync, { data: ids })
}

// 获取消息统计
export const getStatistics = () => {
  return http.get<MessageStatisticsDto>(api.Message.GetStatisticsAsync)
}

// 获取未读消息数量
export const getUnreadCount = () => {
  return http.get<number>(api.Message.GetUnreadCountAsync)
}

// 更新消息内容
export const updateMessage = (id: string, data: UpdateMessageRequest) => {
  return http.put(api.Message.UpdateMessageAsync, data, { params: { id } })
}

// 推送消息给所有用户
export const pushMessageToAll = (data: PushMessageRequest) => {
  return http.post(api.Message.PushMessageToAllAsync, data)
}

// 推送消息给指定角色的用户
export const pushMessageToRole = (data: PushMessageToRoleRequest) => {
  return http.post(api.Message.PushMessageToRoleAsync, data)
}

// 推送已有消息给其他用户
export const pushExistingMessage = (messageId: string, data: PushExistingMessageRequest) => {
  return http.post(api.Message.PushExistingMessageAsync, data, { params: { messageId } })
}

// 撤回消息
export const revokeMessage = (messageId: string) => {
  return http.post(api.Message.RevokeMessageAsync, {}, { params: { messageId } })
}

// 批量撤回消息
export const batchRevokeMessages = (messageIds: string[]) => {
  return http.post(api.Message.BatchRevokeMessagesAsync, messageIds)
}

// 获取消息详情（包含接收者列表）- 管理员查看
export const getMessageDetail = (messageId: string) => {
  return http.get<MessageDetailDto>(api.Message.GetMessageDetailAsync, { params: { messageId } })
}

// 获取消息接收者列表（分页）
export const getMessageRecipients = (messageId: string, params?: MessageRecipientQueryRequest) => {
  return http.get<PagedResult<MessageRecipientDto>>(api.Message.GetMessageRecipientsAsync, { params: { messageId, ...params } })
}