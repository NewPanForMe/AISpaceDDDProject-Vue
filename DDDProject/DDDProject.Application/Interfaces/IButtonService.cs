using DDDProject.Application.DTOs;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 按钮应用服务接口
/// </summary>
public interface IButtonService : IApplicationService
{
    /// <summary>
    /// 获取按钮列表（分页）
    /// </summary>
    /// <param name="request">分页请求</param>
    /// <param name="menuId">菜单ID（可选，用于筛选）</param>
    Task<ApiRequestResult> GetButtonsAsync(PagedRequest request, Guid? menuId = null);

    /// <summary>
    /// 根据ID获取按钮详情
    /// </summary>
    /// <param name="id">按钮ID</param>
    Task<ApiRequestResult> GetButtonByIdAsync(Guid id);

    /// <summary>
    /// 根据菜单ID获取按钮列表
    /// </summary>
    /// <param name="menuId">菜单ID</param>
    Task<ApiRequestResult> GetButtonsByMenuIdAsync(Guid menuId);

    /// <summary>
    /// 创建按钮
    /// </summary>
    /// <param name="request">创建按钮请求</param>
    Task<ApiRequestResult> CreateButtonAsync(CreateButtonRequest request);

    /// <summary>
    /// 更新按钮
    /// </summary>
    /// <param name="request">更新按钮请求</param>
    Task<ApiRequestResult> UpdateButtonAsync(UpdateButtonRequest request);

    /// <summary>
    /// 删除按钮
    /// </summary>
    /// <param name="id">按钮ID</param>
    Task<ApiRequestResult> DeleteButtonAsync(Guid id);

    /// <summary>
    /// 启用按钮
    /// </summary>
    /// <param name="id">按钮ID</param>
    Task<ApiRequestResult> EnableButtonAsync(Guid id);

    /// <summary>
    /// 禁用按钮
    /// </summary>
    /// <param name="id">按钮ID</param>
    Task<ApiRequestResult> DisableButtonAsync(Guid id);
}
