using DDDProject.Domain.Repositories;
using DDDProject.Domain.Entities;
using DDDProject.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDProject.Infrastructure.Repositories;

/// <summary>
/// 用户仓储实现
/// </summary>
public class UserRepository : Repository<User, Guid>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// 通过用户名获取用户
    /// </summary>
    public Task<User?> GetByUserNameAsync(string userName)
    {
        return _dbSet.FirstOrDefaultAsync(u => u.UserName == userName);
    }

    /// <summary>
    /// 通过邮箱获取用户
    /// </summary>
    public Task<User?> GetByEmailAsync(string email)
    {
        return _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// 获取用户总数
    /// </summary>
    public Task<int> GetTotalCountAsync()
    {
        return _dbSet.CountAsync();
    }

    /// <summary>
    /// 检查用户名是否存在
    /// </summary>
    public Task<bool> UserNameExistsAsync(string userName, Guid? excludeId = null)
    {
        var query = _dbSet.Where(u => u.UserName == userName);
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }
        return query.AnyAsync();
    }

    /// <summary>
    /// 检查邮箱是否存在
    /// </summary>
    public Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Email == email);
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }
        return query.AnyAsync();
    }
}
