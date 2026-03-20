namespace DDDProject.Domain.Entities;

/// <summary>
/// 聚合根基类
/// </summary>
/// <typeparam name="TId">主键类型</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
{
}

/// <summary>
/// 聚合根基类（使用 Guid 作为主键）
/// </summary>
public abstract class AggregateRoot : AggregateRoot<Guid>
{
}
