using Microsoft.EntityFrameworkCore;
using DDDProject.Domain.Repositories;
using DDDProject.Domain.Entities;
using DDDProject.Infrastructure.Contexts;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDProject.Infrastructure.Repositories;

/// <summary>
/// 针对使用Guid作为主键的实体的仓储实现
/// </summary>
/// <typeparam name="TEntity">实体类型（必须继承自Entity&lt;Guid&gt;）</typeparam>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity<Guid>
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IOrderedQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(predicate);
        var orderedQuery = orderBy(query.OrderBy(x => x.Id));
        return await orderedQuery.Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}