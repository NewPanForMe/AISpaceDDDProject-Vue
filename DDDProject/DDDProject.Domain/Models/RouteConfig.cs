namespace DDDProject.Domain.Models;

/// <summary>
/// 路由配置模型
/// </summary>
public class RouteConfig
{
    /// <summary>
    /// 路由路径
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 路由名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 组件路径
    /// </summary>
    public string Component { get; set; } = string.Empty;

    /// <summary>
    /// 图标名称（可选）
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 父级菜单ID（可选）
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 排序号（可选）
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 菜单状态：0-禁用，1-启用（可选）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 子菜单列表（可选）
    /// </summary>
    public List<RouteConfig>? Children { get; set; }
}
