namespace DDDProject.Domain.Entities;

/// <summary>
/// 角色实体
/// </summary>
public class Role : AggregateRoot
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 角色状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; private set; } = 1;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; private set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected Role() { }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="name">角色名称</param>
    /// <param name="code">角色编码</param>
    /// <param name="description">角色描述</param>
    /// <param name="sortOrder">排序号</param>
    public static Role Create(string name, string code, string? description = null, int sortOrder = 0)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            Code = code,
            Description = description,
            SortOrder = sortOrder,
            Status = 1,
            CreatedAt = DateTime.Now
        };

        return role;
    }

    /// <summary>
    /// 更新角色信息
    /// </summary>
    /// <param name="name">角色名称</param>
    /// <param name="code">角色编码</param>
    /// <param name="description">角色描述</param>
    /// <param name="sortOrder">排序号</param>
    /// <param name="remark">备注</param>
    public void Update(string? name = null, string? code = null, string? description = null, int? sortOrder = null, string? remark = null)
    {
        if (!string.IsNullOrEmpty(name))
            Name = name;

        if (!string.IsNullOrEmpty(code))
            Code = code;

        if (description != null)
            Description = description;

        if (sortOrder.HasValue)
            SortOrder = sortOrder.Value;

        if (remark != null)
            Remark = remark;

        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 启用角色
    /// </summary>
    public void Enable()
    {
        Status = 1;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 禁用角色
    /// </summary>
    public void Disable()
    {
        Status = 0;
        UpdatedAt = DateTime.Now;
    }
}