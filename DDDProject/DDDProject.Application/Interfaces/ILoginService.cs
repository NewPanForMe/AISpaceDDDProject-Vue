using DDDProject.Application.DTOs;
using System.Threading.Tasks;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 登录服务接口
/// </summary>
public interface ILoginService : IApplicationService
{
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录结果</returns>
    Task<ApiRequestResult> LoginAsync(LoginRequest request);
}
