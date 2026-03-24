namespace DDDProject.Domain.Repositories;

using DDDProject.Domain.Entities;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体列表</returns>
    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有数据（可选条件和排序）
    /// </summary>
    IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null, 
        Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy = null,
        params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// 获取单条数据
    /// </summary>
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取列表（支持分页）
    /// </summary>
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, 
        Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取数量
    /// </summary>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
}
