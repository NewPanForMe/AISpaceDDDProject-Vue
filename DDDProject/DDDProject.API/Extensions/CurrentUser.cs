using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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
        /// <remarks>
        /// 从 JWT Token 的 sub claim 中获取，与 LoginService 创建 Token 时的 claim 保持一致
        /// </remarks>
        public Guid UserId
        {
            get
            {
                // 尝试从 sub claim 获取（JWT Registered Claim Names）
                var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
                    ?? _contextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
            }
        }

        /// <summary>
        /// 获取当前用户的用户名
        /// </summary>
        /// <remarks>
        /// 从 JWT Token 的 Name claim 中获取，与 LoginService 创建 Token 时的 claim 保持一致
        /// </remarks>
        public string UserName
        {
            get
            {
                // 尝试从多个可能的 claim 中获取用户名
                return _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
                    ?? _contextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Name)?.Value
                    ?? _contextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value
                    ?? _contextAccessor.HttpContext?.User?.Identity?.Name
                    ?? string.Empty;
            }
        }

        /// <summary>
        /// 获取当前用户的真实姓名
        /// </summary>
        /// <remarks>
        /// 从 JWT Token 的 givenname claim 中获取，与 LoginService 创建 Token 时的 claim 保持一致
        /// </remarks>
        public string RealName
        {
            get
            {
                // 尝试从多个可能的 claim 中获取真实姓名
                return _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value
                    ?? _contextAccessor.HttpContext?.User?.FindFirst("givenname")?.Value
                    ?? string.Empty;
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