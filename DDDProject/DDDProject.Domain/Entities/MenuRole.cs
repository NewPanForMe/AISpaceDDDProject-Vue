namespace DDDProject.Domain.Entities;

/// <summary>
/// 菜单角色关联实体
/// </summary>
public class MenuRole : Entity
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid MenuId { get; private set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected MenuRole() { }

    /// <summary>
    /// 创建菜单角色关联
    /// </summary>
    /// <param name="menuId">菜单ID</param>
    /// <param name="roleId">角色ID</param>
    public static MenuRole Create(Guid menuId, Guid roleId)
    {
        return new MenuRole
        {
            Id = Guid.NewGuid(),
            MenuId = menuId,
            RoleId = roleId,
            CreatedAt = DateTime.Now
        };
    }
}

/// <summary>
/// 系统设置实体
/// </summary>
public class Setting : AggregateRoot
{
    /// <summary>
    /// 设置键
    /// </summary>
    public string Key { get; private set; } = string.Empty;

    /// <summary>
    /// 设置值
    /// </summary>
    public string Value { get; private set; } = string.Empty;

    /// <summary>
    /// 设置描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 设置分组
    /// </summary>
    public string Group { get; private set; } = "General";

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected Setting() { }

    /// <summary>
    /// 创建设置项
    /// </summary>
    /// <param name="key">设置键</param>
    /// <param name="value">设置值</param>
    /// <param name="description">设置描述</param>
    /// <param name="group">设置分组</param>
    public static Setting Create(string key, string value, string? description = null, string group = "General")
    {
        return new Setting
        {
            Id = Guid.NewGuid(),
            Key = key,
            Value = value,
            Description = description,
            Group = group,
            CreatedAt = DateTime.Now
        };
    }

    /// <summary>
    /// 更新设置值
    /// </summary>
    /// <param name="value">设置值</param>
    public void UpdateValue(string value)
    {
        Value = value;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 更新设置信息
    /// </summary>
    /// <param name="value">设置值</param>
    /// <param name="description">设置描述</param>
    public void Update(string? value = null, string? description = null)
    {
        if (value is not null)
            Value = value;

        if (description is not null)
            Description = description;

        UpdatedAt = DateTime.Now;
    }
}

/// <summary>
/// 权限实体（操作按钮权限）
/// </summary>
public class Permission : Entity
{
    /// <summary>
    /// 权限编码（唯一标识，如：user:add）
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 权限名称（显示名称，如：添加用户）
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 权限描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 所属模块（如：User、Role、Menu）
    /// </summary>
    public string Module { get; private set; } = string.Empty;

    /// <summary>
    /// 关联菜单ID（可选，用于权限分组）
    /// </summary>
    public Guid? MenuId { get; private set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; private set; } = 0;

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; private set; } = 1;

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected Permission() { }

    /// <summary>
    /// 创建权限
    /// </summary>
    /// <param name="code">权限编码</param>
    /// <param name="name">权限名称</param>
    /// <param name="module">所属模块</param>
    /// <param name="description">权限描述</param>
    /// <param name="menuId">关联菜单ID</param>
    /// <param name="sortOrder">排序号</param>
    public static Permission Create(
        string code,
        string name,
        string module,
        string? description = null,
        Guid? menuId = null,
        int sortOrder = 0)
    {
        return new Permission
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            Module = module,
            Description = description,
            MenuId = menuId,
            SortOrder = sortOrder,
            Status = 1,
            CreatedAt = DateTime.Now
        };
    }

    /// <summary>
    /// 更新权限信息
    /// </summary>
    /// <param name="name">权限名称</param>
    /// <param name="description">权限描述</param>
    /// <param name="sortOrder">排序号</param>
    public void Update(string? name = null, string? description = null, int? sortOrder = null)
    {
        if (!string.IsNullOrEmpty(name))
            Name = name;

        if (description is not null)
            Description = description;

        if (sortOrder.HasValue)
            SortOrder = sortOrder.Value;

        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 启用权限
    /// </summary>
    public void Enable()
    {
        Status = 1;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 禁用权限
    /// </summary>
    public void Disable()
    {
        Status = 0;
        UpdatedAt = DateTime.Now;
    }
}

/// <summary>
/// 角色权限关联实体
/// </summary>
public class RolePermission : Entity
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// 权限ID
    /// </summary>
    public Guid PermissionId { get; private set; }

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected RolePermission() { }

    /// <summary>
    /// 创建角色权限关联
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="permissionId">权限ID</param>
    public static RolePermission Create(Guid roleId, Guid permissionId)
    {
        return new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            PermissionId = permissionId,
            CreatedAt = DateTime.Now
        };
    }
}