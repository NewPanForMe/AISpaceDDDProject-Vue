namespace DDDProject.Application.DTOs;

/// <summary>
/// 菜单数据传输对象
/// </summary>
public class MenuDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 组件路径
    /// </summary>
    public string Component { get; set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 父级菜单ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 菜单状态: 0-禁用，1-启用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 子菜单列表
    /// </summary>
    public List<MenuDto>? Children { get; set; }
}

/// <summary>
/// 创建菜单请求数据传输对象
/// </summary>
public class CreateMenuRequestDto
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 组件路径
    /// </summary>
    public string Component { get; set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 父级菜单ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; } = 0;
}

/// <summary>
/// 更新菜单请求数据传输对象
/// </summary>
public class UpdateMenuRequestDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 父级菜单ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int? SortOrder { get; set; }
}

/// <summary>
/// 更改菜单状态请求数据传输对象
/// </summary>
public class ChangeMenuStatusRequestDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid Id { get; set; }
}