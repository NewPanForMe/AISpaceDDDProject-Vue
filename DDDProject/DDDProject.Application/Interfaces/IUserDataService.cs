using DDDProject.Application.DTOs;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 用户数据应用服务接口
/// </summary>
public interface IUserDataService : IApplicationService
{
    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    Task<ApiRequestResult> GetUsersAsync(PagedRequest request);

    /// <summary>
    /// 获取用户详情
    /// </summary>
    Task<ApiRequestResult> GetUserByIdAsync(Guid id);

    /// <summary>
    /// 创建用户
    /// </summary>
    Task<ApiRequestResult> CreateUserAsync(CreateUserRequest request);

    /// <summary>
    /// 更新用户
    /// </summary>
    Task<ApiRequestResult> UpdateUserAsync(UpdateUserRequest request);

    /// <summary>
    /// 删除用户
    /// </summary>
    Task<ApiRequestResult> DeleteUserAsync(Guid id);

    /// <summary>
    /// 启用用户
    /// </summary>
    Task<ApiRequestResult> EnableUserAsync(Guid id);

    /// <summary>
    /// 禁用用户
    /// </summary>
    Task<ApiRequestResult> DisableUserAsync(Guid id);

    /// <summary>
    /// 重置用户密码
    /// </summary>
    Task<ApiRequestResult> ResetPasswordAsync(ResetPasswordRequest request);
}
