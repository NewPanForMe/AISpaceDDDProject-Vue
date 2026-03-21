using DDDProject.API.Extensions;
using DDDProject.Application.DTOs;

namespace DDDProject.API.Middlewares
{
    /// <summary>
    /// 权限检查中间件
    /// </summary>
    public class PermissionCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {

            // 验证用户是否经过身份验证
            if (!context!.User!.Identity!.IsAuthenticated)
            {
                context.Response.StatusCode = 401; // Unauthorized
                var result = new ApiRequestResult
                {
                    Success = false,
                    Message = "未授权访问，请先登录"
                };
                await context.Response.WriteAsJsonAsync(result);
                return;
            }

            // 获取CurrentUser实例进行进一步检查（如果需要）
            var currentUser = serviceProvider.GetService<CurrentUser>();
            if (currentUser != null && !currentUser.IsAuthenticated)
            {
                context.Response.StatusCode = 401;
                var result = new ApiRequestResult
                {
                    Success = false,
                    Message = "身份验证失败"
                };
                await context.Response.WriteAsJsonAsync(result);
                return;
            }

            // 调用下一个中间件
            await _next(context);
        }
    }
}