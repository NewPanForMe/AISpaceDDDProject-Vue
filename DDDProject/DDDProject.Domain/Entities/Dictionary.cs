namespace DDDProject.Domain.Entities;

/// <summary>
/// 字典实体
/// </summary>
public class Dictionary : AggregateRoot
{
    /// <summary>
    /// 字典编码
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 字典名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 字典值
    /// </summary>
    public string Value { get; private set; } = string.Empty;

    /// <summary>
    /// 字典类型/分组
    /// </summary>
    public string Type { get; private set; } = string.Empty;

    /// <summary>
    /// 字典状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; private set; } = 1;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; private set; } = 0;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected Dictionary() { }

    /// <summary>
    /// 创建字典
    /// </summary>
    /// <param name="code">字典编码</param>
    /// <param name="name">字典名称</param>
    /// <param name="value">字典值</param>
    /// <param name="type">字典类型</param>
    /// <param name="sortOrder">排序号</param>
    /// <param name="description">描述</param>
    public static Dictionary Create(string code, string name, string value, string type, int sortOrder = 0, string? description = null)
    {
        var dictionary = new Dictionary
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            Value = value,
            Type = type,
            SortOrder = sortOrder,
            Status = 1,
            Description = description,
            CreatedAt = DateTime.Now
        };

        return dictionary;
    }

    /// <summary>
    /// 更新字典信息
    /// </summary>
    /// <param name="code">字典编码</param>
    /// <param name="name">字典名称</param>
    /// <param name="value">字典值</param>
    /// <param name="type">字典类型</param>
    /// <param name="sortOrder">排序号</param>
    /// <param name="description">描述</param>
    /// <param name="remark">备注</param>
    public void Update(string? code = null, string? name = null, string? value = null, string? type = null, int? sortOrder = null, string? description = null, string? remark = null)
    {
        if (!string.IsNullOrEmpty(code))
            Code = code;

        if (!string.IsNullOrEmpty(name))
            Name = name;

        if (!string.IsNullOrEmpty(value))
            Value = value;

        if (!string.IsNullOrEmpty(type))
            Type = type;

        if (sortOrder.HasValue)
            SortOrder = sortOrder.Value;

        if (description != null)
            Description = description;

        if (remark != null)
            Remark = remark;

        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 启用字典
    /// </summary>
    public void Enable()
    {
        Status = 1;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 禁用字典
    /// </summary>
    public void Disable()
    {
        Status = 0;
        UpdatedAt = DateTime.Now;
    }
}