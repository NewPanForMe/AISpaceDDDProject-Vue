using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using DDDProject.Application.Interfaces;

namespace DDDProject.API.Extensions
{
    /// <summary>
    /// 当前用户信息提供者
    /// </summary>
    public class CurrentUser : ICurrentUserContext
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
                // 从 sub claim 获取用户ID
                var userIdClaim = _contextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
            }
        }

        /// <summary>
        /// 获取当前用户的用户名
        /// </summary>
        /// <remarks>
        /// 从 JWT Token 的 unique_name claim 中获取
        /// </remarks>
        public string UserName
        {
            get
            {
                // 从 unique_name claim 获取用户名
                return _contextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value
                    ?? _contextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Name)?.Value
                    ?? string.Empty;
            }
        }

        /// <summary>
        /// 获取当前用户的真实姓名
        /// </summary>
        /// <remarks>
        /// 从 JWT Token 的 ClaimTypes.GivenName claim 中获取
        /// </remarks>
        public string RealName
        {
            get
            {
                // 从 ClaimTypes.GivenName claim 获取真实姓名
                return _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value
                    ?? string.Empty;
            }
        }

        /// <summary>
        /// 获取当前用户的角色编码列表
        /// </summary>
        /// <remarks>
        /// 从 JWT Token 的 roles claim 中获取
        /// </remarks>
        public List<string> Roles
        {
            get
            {
                // 从 roles claim 获取角色列表（逗号分隔）
                var rolesClaim = _contextAccessor.HttpContext?.User?.FindFirst("roles")?.Value;
                if (string.IsNullOrEmpty(rolesClaim))
                {
                    return new List<string>();
                }
                return rolesClaim.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
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