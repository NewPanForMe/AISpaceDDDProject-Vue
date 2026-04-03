using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DDDProject.API.Controllers;

/// <summary>
/// 站内信控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class MessageController : BaseApiController
{
    private readonly IMessageService _messageService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="messageService">站内信服务</param>
    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// 获取当前用户的消息列表（分页）
    /// </summary>
    [HttpGet]
    [ActionName("GetMessagesAsync")]
    [ApiSearch(Name = "获取消息列表", Description = "返回当前用户的消息列表（支持分页和筛选）", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> GetMessagesAsync([FromQuery] MessageQueryRequest request)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        return await _messageService.GetMessagesAsync(userId, request);
    }

    /// <summary>
    /// 获取消息详情
    /// </summary>
    [HttpGet]
    [ActionName("GetMessageByIdAsync")]
    [ApiSearch(Name = "获取消息详情", Description = "根据ID获取消息详细信息", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> GetMessageByIdAsync([FromQuery] Guid id)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        return await _messageService.GetMessageByIdAsync(id, userId);
    }

    /// <summary>
    /// 发送用户消息
    /// </summary>
    [HttpPost]
    [ActionName("SendMessageAsync")]
    [Permission("message:send")]
    [ApiSearch(Name = "发送消息", Description = "发送用户消息给指定用户", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> SendMessageAsync([FromBody] CreateMessageRequest request)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        var userName = GetCurrentUserName() ?? "";
        return await _messageService.SendMessageAsync(userId, userName, request);
    }

    /// <summary>
    /// 发送系统消息（管理员）
    /// </summary>
    [HttpPost]
    [ActionName("SendSystemMessageAsync")]
    [Permission("message:system")]
    [ApiSearch(Name = "发送系统消息", Description = "发送系统消息给指定用户", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> SendSystemMessageAsync([FromBody] CreateMessageRequest request)
    {
        return await _messageService.SendSystemMessageAsync(
            request.ReceiverId,
            request.ReceiverName,
            request.Title,
            request.Content,
            request.Priority
        );
    }

    /// <summary>
    /// 批量发送系统消息（管理员）
    /// </summary>
    [HttpPost]
    [ActionName("BatchSendSystemMessageAsync")]
    [Permission("message:system")]
    [ApiSearch(Name = "批量发送系统消息", Description = "批量发送系统消息给多个用户", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> BatchSendSystemMessageAsync([FromBody] BatchSendMessageRequest request)
    {
        return await _messageService.BatchSendSystemMessageAsync(request);
    }

    /// <summary>
    /// 标记消息为已读
    /// </summary>
    [HttpPost]
    [ActionName("MarkAsReadAsync")]
    [ApiSearch(Name = "标记已读", Description = "将消息标记为已读", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> MarkAsReadAsync([FromQuery] Guid id)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        return await _messageService.MarkAsReadAsync(id, userId);
    }

    /// <summary>
    /// 标记消息为未读
    /// </summary>
    [HttpPost]
    [ActionName("MarkAsUnreadAsync")]
    [ApiSearch(Name = "标记未读", Description = "将消息标记为未读", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> MarkAsUnreadAsync([FromQuery] Guid id)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        return await _messageService.MarkAsUnreadAsync(id, userId);
    }

    /// <summary>
    /// 批量标记为已读
    /// </summary>
    [HttpPost]
    [ActionName("BatchMarkAsReadAsync")]
    [ApiSearch(Name = "批量标记已读", Description = "批量将消息标记为已读", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> BatchMarkAsReadAsync([FromBody] List<Guid> ids)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        return await _messageService.BatchMarkAsReadAsync(ids, userId);
    }

    /// <summary>
    /// 标记所有消息为已读
    /// </summary>
    [HttpPost]
    [ActionName("MarkAllAsReadAsync")]
    [ApiSearch(Name = "全部标记已读", Description = "将所有未读消息标记为已读", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> MarkAllAsReadAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        return await _messageService.MarkAllAsReadAsync(userId);
    }

    /// <summary>
    /// 删除消息
    /// </summary>
    [HttpDelete]
    [ActionName("DeleteMessageAsync")]
    [Permission("message:delete")]
    [ApiSearch(Name = "删除消息", Description = "删除指定消息", Category = ApiSearchCategory.Other)]
    public async Task<IActionResult> DeleteMessageAsync([FromQuery] Guid id)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        var result = await _messageService.DeleteMessageAsync(id, userId);
        if (!result.Success)
            return BadRequest(result);
        return NoContent();
    }

    /// <summary>
    /// 批量删除消息
    /// </summary>
    [HttpDelete]
    [ActionName("BatchDeleteMessagesAsync")]
    [Permission("message:delete")]
    [ApiSearch(Name = "批量删除消息", Description = "批量删除多条消息", Category = ApiSearchCategory.Other)]
    public async Task<IActionResult> BatchDeleteMessagesAsync([FromBody] List<Guid> ids)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        var result = await _messageService.BatchDeleteMessagesAsync(ids, userId);
        if (!result.Success)
            return BadRequest(result);
        return NoContent();
    }

    /// <summary>
    /// 获取消息统计
    /// </summary>
    [HttpGet]
    [ActionName("GetStatisticsAsync")]
    [ApiSearch(Name = "获取消息统计", Description = "获取当前用户的消息统计信息", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> GetStatisticsAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        return await _messageService.GetStatisticsAsync(userId);
    }

    /// <summary>
    /// 获取未读消息数量
    /// </summary>
    [HttpGet]
    [ActionName("GetUnreadCountAsync")]
    [ApiSearch(Name = "获取未读消息数", Description = "获取当前用户的未读消息数量", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> GetUnreadCountAsync()
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        return await _messageService.GetUnreadCountAsync(userId);
    }

    /// <summary>
    /// 更新消息内容（仅支持未读消息）
    /// </summary>
    [HttpPut]
    [ActionName("UpdateMessageAsync")]
    [Permission("message:edit")]
    [ApiSearch(Name = "更新消息", Description = "更新未读消息的内容", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> UpdateMessageAsync([FromQuery] Guid id, [FromBody] UpdateMessageRequest request)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        return await _messageService.UpdateMessageAsync(id, userId, request);
    }

    /// <summary>
    /// 推送系统消息给所有用户（管理员）
    /// </summary>
    [HttpPost]
    [ActionName("PushMessageToAllAsync")]
    [Permission("message:push")]
    [ApiSearch(Name = "推送消息给所有用户", Description = "推送系统消息给所有启用的用户", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> PushMessageToAllAsync([FromBody] PushMessageRequest request)
    {
        return await _messageService.PushMessageToAllAsync(request);
    }

    /// <summary>
    /// 推送系统消息给指定角色的用户（管理员）
    /// </summary>
    [HttpPost]
    [ActionName("PushMessageToRoleAsync")]
    [Permission("message:push")]
    [ApiSearch(Name = "推送消息给角色用户", Description = "推送系统消息给指定角色的用户", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> PushMessageToRoleAsync([FromBody] PushMessageToRoleRequest request)
    {
        return await _messageService.PushMessageToRoleAsync(request);
    }

    /// <summary>
    /// 推送已有消息给其他用户
    /// </summary>
    [HttpPost]
    [ActionName("PushExistingMessageAsync")]
    [Permission("message:push")]
    [ApiSearch(Name = "推送已有消息", Description = "将已有消息推送给其他用户", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> PushExistingMessageAsync([FromQuery] Guid messageId, [FromBody] PushExistingMessageRequest request)
    {
        var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("用户未登录");
        return await _messageService.PushExistingMessageAsync(messageId, userId, request);
    }
}