namespace DDDProject.Application.DTOs;

/// <summary>
/// 分页请求参数
/// </summary>
public class PagedRequest
{
    /// <summary>
    /// 页码，默认为1
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每页大小，默认为10
    /// </summary>
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// 分页响应结果
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> List { get; set; } = new List<T>();

    /// <summary>
    /// 总记录数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => PageSize > 0 ? (Total + PageSize - 1) / PageSize : 0;
}
