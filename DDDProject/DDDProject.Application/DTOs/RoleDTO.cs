namespace DDDProject.Application.DTOs;

/// <summary>
/// 角色 DTO
/// </summary>
public class RoleDto : DTO
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 角色状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 创建角色请求
/// </summary>
public class CreateRoleRequest : DTO
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 更新角色请求
/// </summary>
public class UpdateRoleRequest : DTO
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 角色编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int? SortOrder { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 角色状态：0-禁用，1-启用
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 分配用户角色请求
/// </summary>
public class AssignUserRolesRequest : DTO
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<Guid> RoleIds { get; set; } = new();
}

/// <summary>
/// 分配角色用户请求
/// </summary>
public class AssignRoleUsersRequest : DTO
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 用户ID列表
    /// </summary>
    public List<Guid> UserIds { get; set; } = new();
}