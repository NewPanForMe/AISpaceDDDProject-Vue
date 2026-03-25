namespace DDDProject.Application.DTOs;

/// <summary>
/// 菜单角色 DTO
/// </summary>
public class MenuRoleDto : DTO
{
    /// <summary>
    /// 关联ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid MenuId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string? MenuName { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 分配角色菜单请求
/// </summary>
public class AssignRoleMenusRequest : DTO
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<Guid> MenuIds { get; set; } = new();
}

/// <summary>
/// 分配菜单角色请求
/// </summary>
public class AssignMenuRolesRequest : DTO
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid MenuId { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<Guid> RoleIds { get; set; } = new();
}