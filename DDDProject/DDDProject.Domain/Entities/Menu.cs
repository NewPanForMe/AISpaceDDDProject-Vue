using System.ComponentModel.DataAnnotations;

namespace DDDProject.Domain.Entities;

/// <summary>
/// 菜单实体
/// </summary>
public class Menu : AggregateRoot
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    public string Path { get; private set; } = string.Empty;

    /// <summary>
    /// 组件路径
    /// </summary>
    public string Component { get; private set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// 父级菜单ID
    /// </summary>
    public Guid? ParentId { get; private set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; private set; } = 0;

    /// <summary>
    /// 菜单状态: 0-禁用，1-启用
    /// </summary>
    public int Status { get; private set; } = 1;

    /// <summary>
    /// 子菜单列表
    /// </summary>
    public ICollection<Menu> Children { get; set; } = new List<Menu>();

    /// <summary>
    /// 父级菜单（导航属性）
    /// </summary>
    public Menu? Parent { get; set; }

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected Menu() { }

    /// <summary>
    /// 创建菜单
    /// </summary>
    /// <param name="name">菜单名称</param>
    /// <param name="path">路由路径</param>
    /// <param name="component">组件路径</param>
    /// <param name="icon">图标</param>
    /// <param name="parentId">父级菜单ID</param>
    /// <param name="sortOrder">排序号</param>
    /// <param name="status">状态</param>
    public static Menu Create(
        string name, 
        string path, 
        string component, 
        string? icon = null, 
        Guid? parentId = null, 
        int sortOrder = 0, 
        int status = 1)
    {
        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            Name = name,
            Path = path,
            Component = component,
            Icon = icon,
            ParentId = parentId,
            SortOrder = sortOrder,
            Status = status,
            CreatedAt = DateTime.Now
        };

        return menu;
    }

    /// <summary>
    /// 更新菜单信息
    /// </summary>
    /// <param name="name">菜单名称</param>
    /// <param name="path">路由路径</param>
    /// <param name="component">组件路径</param>
    /// <param name="icon">图标</param>
    /// <param name="parentId">父级菜单ID</param>
    /// <param name="sortOrder">排序号</param>
    public void Update(
        string? name = null, 
        string? path = null, 
        string? component = null, 
        string? icon = null, 
        Guid? parentId = null, 
        int? sortOrder = null)
    {
        if (!string.IsNullOrEmpty(name))
            Name = name;

        if (!string.IsNullOrEmpty(path))
            Path = path;

        if (!string.IsNullOrEmpty(component))
            Component = component;

        if (icon != null)
            Icon = icon;

        if (parentId.HasValue)
            ParentId = parentId.Value;

        if (sortOrder.HasValue)
            SortOrder = sortOrder.Value;

        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 更新菜单状态
    /// </summary>
    /// <param name="status">状态</param>
    public void UpdateStatus(int status)
    {
        Status = status;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 启用菜单
    /// </summary>
    public void Enable()
    {
        Status = 1;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 禁用菜单
    /// </summary>
    public void Disable()
    {
        Status = 0;
        UpdatedAt = DateTime.Now;
    }
}