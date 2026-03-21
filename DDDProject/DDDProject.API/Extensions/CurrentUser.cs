using Microsoft.AspNetCore.Http;

namespace DDDProject.API.Extensions
{
    /// <summary>
    /// 当前用户信息提供者
    /// </summary>
    public class CurrentUser
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUser(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// 获取当前用户的ID
        /// </summary>
        public Guid UserId
        {
            get
            {
                var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
                return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
            }
        }

        /// <summary>
        /// 获取当前用户的用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return _contextAccessor.HttpContext?.User?.Identity?.Name ?? string.Empty;
            }
        }

        /// <summary>
        /// 获取当前用户的真实姓名
        /// </summary>
        public string RealName
        {
            get
            {
                return _contextAccessor.HttpContext?.User?.FindFirst("givenname")?.Value ?? string.Empty;
            }
        }

        /// <summary>
        /// 判断用户是否已认证
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return _contextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
            }
        }
    }
}