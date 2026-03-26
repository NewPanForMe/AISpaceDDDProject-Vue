namespace DDDProject.Application.Interfaces;

/// <summary>
/// 应用服务基接口
/// </summary>
public interface IApplicationService
{
}

/// <summary>
/// 当前用户上下文接口
/// </summary>
public interface ICurrentUserContext
{
    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// 获取当前用户名
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// 获取当前用户真实姓名
    /// </summary>
    string RealName { get; }

    /// <summary>
    /// 获取当前用户角色编码列表
    /// </summary>
    List<string> Roles { get; }

    /// <summary>
    /// 判断用户是否已认证
    /// </summary>
    bool IsAuthenticated { get; }
}
