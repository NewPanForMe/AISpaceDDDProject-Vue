using DDDProject.API.Extensions;
using DDDProject.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

namespace DDDProject.API.Middlewares
{
    /// <summary>
    /// 权限检查中间件
    /// </summary>
    public class PermissionCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PermissionCheckMiddleware> _logger;

        public PermissionCheckMiddleware(RequestDelegate next, ILogger<PermissionCheckMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            // 检查当前请求的控制器或方法是否有 [Authorize] 注解
            var actionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();

            // 如果没有找到 Action 描述符（例如路由不匹配），直接通过
            if (actionDescriptor == null)
            {
                _logger.LogDebug("无 Action 描述符，跳过权限检查");
                await _next(context);
                return;
            }

            // 检查是否有 [Authorize] 注解
            var hasAuthorizeAttribute = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), inherit: true).Length > 0
                || actionDescriptor.MethodInfo.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), inherit: true).Length > 0;

            _logger.LogDebug("控制器: {Controller}, 方法: {Method}, 有 Authorize 注解: {HasAuthorize}", 
                actionDescriptor.ControllerTypeInfo.FullName, actionDescriptor.MethodInfo.Name, hasAuthorizeAttribute);

            // 如果没有 [Authorize] 注解，跳过权限检查
            if (!hasAuthorizeAttribute)
            {
                _logger.LogDebug("无 Authorize 注解，跳过权限检查");
                await _next(context);
                return;
            }

            // 验证用户是否经过身份验证（有 [Authorize] 注解）
            if (!(context.User.Identity?.IsAuthenticated ?? false))
            {
                _logger.LogWarning("未授权访问: 控制器 {Controller}, 方法 {Method}",
                    actionDescriptor.ControllerTypeInfo.FullName, actionDescriptor.MethodInfo.Name);

                context.Response.StatusCode = 401; // Unauthorized
                var result = new ApiRequestResult
                {
                    Success = false,
                    Message = "未授权访问，请先登录"
                };
                await context.Response.WriteAsJsonAsync(result);
                return;
            }

            _logger.LogDebug("权限检查通过: 控制器 {Controller}, 方法 {Method}", 
                actionDescriptor.ControllerTypeInfo.FullName, actionDescriptor.MethodInfo.Name);

            // 调用下一个中间件
            await _next(context);
        }
    }
}