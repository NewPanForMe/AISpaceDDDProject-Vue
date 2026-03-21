namespace DDDProject.Domain.Models;

using DDDProject.Domain.Entities;

/// <summary>
/// 示例聚合根
/// </summary>
public class ExampleAggregate : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private ExampleAggregate()
    {
    }

    public ExampleAggregate(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.Now; // 使用本地时间（中国标准时间）
    }
}
