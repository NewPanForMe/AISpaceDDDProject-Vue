namespace DDDProject.Domain.ValueObjects;

/// <summary>
/// 值对象基类
/// </summary>
public abstract class ValueObject
{
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, component) =>
            {
                var hash = component?.GetHashCode() ?? 0;
                return current * 23 + hash;
            });
    }

    public static bool operator ==(ValueObject a, ValueObject b)
    {
        if (a is null && b is null)
            return true;
        if (a is null || b is null)
            return false;
        return a.Equals(b);
    }

    public static bool operator !=(ValueObject a, ValueObject b)
    {
        return !(a == b);
    }

    protected abstract IEnumerable<object> GetEqualityComponents();
}
