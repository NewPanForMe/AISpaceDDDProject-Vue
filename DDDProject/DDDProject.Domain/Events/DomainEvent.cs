namespace DDDProject.Domain.Events;

/// <summary>
/// 领域事件基类
/// </summary>
public abstract class DomainEvent
{
    public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;

    protected DomainEvent()
    {
    }
}
