using DDDProject.Domain.Entities;

namespace DDDProject.Domain.Repositories;

/// <summary>
/// 用户仓储接口
/// </summary>
public interface IUserRepository : IRepository<User, Guid>
{
    /// <summary>
    /// 通过用户名获取用户
    /// </summary>
    Task<User?> GetByUserNameAsync(string userName);

    /// <summary>
    /// 通过邮箱获取用户
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// 获取用户总数
    /// </summary>
    Task<int> GetTotalCountAsync();

    /// <summary>
    /// 检查用户名是否存在
    /// </summary>
    Task<bool> UserNameExistsAsync(string userName, Guid? excludeId = null);

    /// <summary>
    /// 检查邮箱是否存在
    /// </summary>
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
}
