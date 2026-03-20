namespace DDDProject.Domain.Repositories;

using DDDProject.Domain.Entities;
using System.Linq.Expressions;

/// <summary>
/// 仓储接口定义
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TId">主键类型</typeparam>
public interface IRepository<TEntity, in TId> where TEntity : Entity<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据条件获取列表
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns>实体列表</returns>
    IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate);
}
