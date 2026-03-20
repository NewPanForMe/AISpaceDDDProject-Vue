namespace DDDProject.Domain.Entities;

/// <summary>
/// 实体基类
/// </summary>
/// <typeparam name="TId">主键类型</typeparam>
public abstract class Entity<TId>
{
    public virtual TId Id { get; protected set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (Entity<TId>)obj;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId> a, Entity<TId> b)
    {
        if (a is null && b is null)
            return true;
        if (a is null || b is null)
            return false;
        return a.Equals(b);
    }

    public static bool operator !=(Entity<TId> a, Entity<TId> b)
    {
        return !(a == b);
    }
}

/// <summary>
/// 实体基类（使用 Guid 作为主键）
/// </summary>
public abstract class Entity : Entity<Guid>
{
}
