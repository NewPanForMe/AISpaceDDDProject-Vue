using Microsoft.EntityFrameworkCore;
using DDDProject.Domain.Repositories;
using DDDProject.Domain.Entities;
using DDDProject.Infrastructure.Contexts;
using System.Linq.Expressions;

namespace DDDProject.Infrastructure.Repositories;

/// <summary>
/// 仓储实现
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TId">主键类型</typeparam>
public class Repository<TEntity, TId> : IRepository<TEntity, TId> 
    where TEntity : Entity<TId>
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }
}
