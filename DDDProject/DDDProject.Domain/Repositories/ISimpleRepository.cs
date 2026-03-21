using System.Linq.Expressions;
using DDDProject.Domain.Entities;

namespace DDDProject.Domain.Repositories;

/// <summary>
/// 简化的仓储接口（针对使用Guid作为主键的实体）
/// </summary>
/// <typeparam name="TEntity">实体类型（必须使用Guid作为主键）</typeparam>
public interface IRepository<TEntity> where TEntity : Entity<Guid>
{
    /// <summary>
    /// 根据ID查找实体
    /// </summary>
    Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件获取第一个匹配的实体
    /// </summary>
    Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件获取实体列表
    /// </summary>
    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加实体
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量添加实体
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新实体
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// 删除实体
    /// </summary>
    void Remove(TEntity entity);

    /// <summary>
    /// 保存更改
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}