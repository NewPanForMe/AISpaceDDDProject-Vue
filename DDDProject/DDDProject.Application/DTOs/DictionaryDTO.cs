namespace DDDProject.Application.DTOs;

/// <summary>
/// 字典DTO
/// </summary>
public class DictionaryDto
{
    /// <summary>
    /// 字典ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 字典编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 字典名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 字典值
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型/分组
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 字典状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

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
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// 字典查询请求
/// </summary>
public class DictionaryQueryRequest : PagedRequest
{
    /// <summary>
    /// 字典编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 字典名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 字典类型
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 创建字典请求
/// </summary>
public class CreateDictionaryRequest
{
    /// <summary>
    /// 字典编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 字典名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 字典值
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 更新字典请求
/// </summary>
public class UpdateDictionaryRequest
{
    /// <summary>
    /// 字典ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 字典编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 字典名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 字典值
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// 字典类型
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int? SortOrder { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}