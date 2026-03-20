namespace DDDProject.API.Common;

/// <summary>
/// API 请求结果返回模型
/// </summary>
public class ApiRequestResult
{
    /// <summary>
    /// 是否执行成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 返回提示文字
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 返回数据内容
    /// </summary>
    public object? Data { get; set; }
}
