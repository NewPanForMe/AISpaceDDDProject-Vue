using DDDProject.Application.DTOs;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 菜单应用服务接口
/// </summary>
public interface IMenuService : IApplicationService
{
    /// <summary>
    /// 获取菜单列表（分页）
    /// </summary>
    Task<ApiRequestResult> GetMenusAsync(PagedRequest request);

    /// <summary>
    /// 获取菜单详情
    /// </summary>
    Task<ApiRequestResult> GetMenuByIdAsync(Guid id);

    /// <summary>
    /// 获取树形结构的菜单（用于侧边栏菜单，无需分页）
    /// </summary>
    Task<ApiRequestResult> GetSidebarMenusAsync();

    /// <summary>
    /// 获取分页的树形菜单数据（用于大数据量场景）
    /// </summary>
    Task<ApiRequestResult> GetPagedTreeMenusAsync(PagedRequest request);

    /// <summary>
    /// 根据用户获取菜单树形结构
    /// </summary>
    Task<ApiRequestResult> GetUserMenuTreeAsync(Guid userId);

    /// <summary>
    /// 创建菜单
    /// </summary>
    Task<ApiRequestResult> CreateMenuAsync(MenuDto menuDto);

    /// <summary>
    /// 更新菜单
    /// </summary>
    Task<ApiRequestResult> UpdateMenuAsync(MenuDto menuDto);

    /// <summary>
    /// 删除菜单
    /// </summary>
    Task<ApiRequestResult> DeleteMenuAsync(Guid id);

    /// <summary>
    /// 启用菜单
    /// </summary>
    Task<ApiRequestResult> EnableMenuAsync(Guid id);

    /// <summary>
    /// 禁用菜单
    /// </summary>
    Task<ApiRequestResult> DisableMenuAsync(Guid id);
}