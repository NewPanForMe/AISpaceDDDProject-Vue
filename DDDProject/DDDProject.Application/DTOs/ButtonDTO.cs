namespace DDDProject.Application.DTOs;

/// <summary>
/// 按钮数据传输对象
/// </summary>
public class ButtonDto
{
    /// <summary>
    /// 按钮ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 按钮名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 按钮编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 所属菜单ID
    /// </summary>
    public Guid MenuId { get; set; }

    /// <summary>
    /// 所属菜单名称
    /// </summary>
    public string? MenuName { get; set; }

    /// <summary>
    /// 权限编码
    /// </summary>
    public string? PermissionCode { get; set; }

    /// <summary>
    /// 按钮图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 按钮状态: 0-禁用，1-启用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 创建按钮请求数据传输对象
/// </summary>
public class CreateButtonRequest
{
    /// <summary>
    /// 按钮名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 按钮编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 所属菜单ID
    /// </summary>
    public Guid MenuId { get; set; }

    /// <summary>
    /// 权限编码
    /// </summary>
    public string? PermissionCode { get; set; }

    /// <summary>
    /// 按钮图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// 更新按钮请求数据传输对象
/// </summary>
public class UpdateButtonRequest
{
    /// <summary>
    /// 按钮ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 按钮名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 按钮编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 所属菜单ID
    /// </summary>
    public Guid? MenuId { get; set; }

    /// <summary>
    /// 权限编码
    /// </summary>
    public string? PermissionCode { get; set; }

    /// <summary>
    /// 按钮图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int? SortOrder { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 更改按钮状态请求数据传输对象
/// </summary>
public class ChangeButtonStatusRequest
{
    /// <summary>
    /// 按钮ID
    /// </summary>
    public Guid Id { get; set; }
}
