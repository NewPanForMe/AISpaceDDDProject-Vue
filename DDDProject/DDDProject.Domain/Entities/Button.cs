using System.ComponentModel.DataAnnotations;

namespace DDDProject.Domain.Entities;

/// <summary>
/// 按钮实体 - 用于管理页面按钮及其权限
/// </summary>
public class Button : AggregateRoot
{
    /// <summary>
    /// 按钮名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 按钮编码（唯一标识）
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 所属菜单ID
    /// </summary>
    public Guid MenuId { get; private set; }

    /// <summary>
    /// 权限编码（关联权限）
    /// </summary>
    public string? PermissionCode { get; private set; }

    /// <summary>
    /// 按钮图标
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; private set; } = 0;

    /// <summary>
    /// 按钮状态: 0-禁用，1-启用
    /// </summary>
    public int Status { get; private set; } = 1;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 所属菜单（导航属性）
    /// </summary>
    public Menu? Menu { get; set; }

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected Button() { }

    /// <summary>
    /// 创建按钮
    /// </summary>
    public static Button Create(
        string name,
        string code,
        Guid menuId,
        string? permissionCode = null,
        string? icon = null,
        int sortOrder = 0,
        string? description = null,
        int status = 1)
    {
        return new Button
        {
            Id = Guid.NewGuid(),
            Name = name,
            Code = code,
            MenuId = menuId,
            PermissionCode = permissionCode,
            Icon = icon,
            SortOrder = sortOrder,
            Description = description,
            Status = status,
            CreatedAt = DateTime.Now
        };
    }

    /// <summary>
    /// 更新按钮信息
    /// </summary>
    public void Update(
        string? name = null,
        string? code = null,
        Guid? menuId = null,
        string? permissionCode = null,
        string? icon = null,
        int? sortOrder = null,
        string? description = null,
        int? status = null)
    {
        if (!string.IsNullOrEmpty(name))
            Name = name;

        if (!string.IsNullOrEmpty(code))
            Code = code;

        if (menuId.HasValue)
            MenuId = menuId.Value;

        if (permissionCode != null)
            PermissionCode = permissionCode;

        if (icon != null)
            Icon = icon;

        if (sortOrder.HasValue)
            SortOrder = sortOrder.Value;

        if (description != null)
            Description = description;

        if (status.HasValue)
            Status = status.Value;

        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 启用按钮
    /// </summary>
    public void Enable()
    {
        Status = 1;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 禁用按钮
    /// </summary>
    public void Disable()
    {
        Status = 0;
        UpdatedAt = DateTime.Now;
    }
}
