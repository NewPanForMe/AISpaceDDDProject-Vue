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

/// <summary>
/// 系统设置 DTO
/// </summary>
public class SettingDto : DTO
{
    /// <summary>
    /// 设置ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 设置键
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 设置值
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 设置描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 设置分组
    /// </summary>
    public string Group { get; set; } = "General";

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
/// 更新设置请求
/// </summary>
public class UpdateSettingRequest : DTO
{
    /// <summary>
    /// 设置键
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 设置值
    /// </summary>
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// 批量更新设置请求
/// </summary>
public class BatchUpdateSettingsRequest : DTO
{
    /// <summary>
    /// 设置项列表
    /// </summary>
    public List<UpdateSettingRequest> Settings { get; set; } = new();
}

/// <summary>
/// 权限 DTO
/// </summary>
public class PermissionDto : DTO
{
    /// <summary>
    /// 权限ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 权限编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 权限名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 权限描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 所属模块
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 关联菜单ID
    /// </summary>
    public Guid? MenuId { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; }

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
/// 创建权限请求
/// </summary>
public class CreatePermissionRequest : DTO
{
    /// <summary>
    /// 权限编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 权限名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 权限描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 所属模块
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 关联菜单ID
    /// </summary>
    public Guid? MenuId { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// 更新权限请求
/// </summary>
public class UpdatePermissionRequest : DTO
{
    /// <summary>
    /// 权限ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 权限名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 权限描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int? SortOrder { get; set; }
}

/// <summary>
/// 分配角色权限请求
/// </summary>
public class AssignRolePermissionsRequest : DTO
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 权限ID列表
    /// </summary>
    public List<Guid> PermissionIds { get; set; } = new();
}