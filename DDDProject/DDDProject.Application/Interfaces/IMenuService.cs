using DDDProject.Application.DTOs;
using DDDProject.Domain.Models;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 菜单应用服务接口
/// </summary>
public interface IMenuService : IApplicationService
{


    /// <summary>
    /// 获取树形结构的菜单（用于侧边栏菜单，无需分页）
    /// </summary>
    Task<ApiRequestResult> GetSidebarMenusAsync();

    /// <summary>
    /// 根据菜单ID列表获取树形结构的菜单（用于侧边栏菜单，根据用户权限过滤）
    /// </summary>
    /// <param name="menuIds">菜单ID列表</param>
    Task<ApiRequestResult> GetSidebarMenusByMenuIdsAsync(List<Guid> menuIds);

    /// <summary>
    /// 获取分页的树形菜单数据（用于大数据量场景）
    /// </summary>
    Task<ApiRequestResult> GetPagedTreeMenusAsync(PagedRequest request);
    /// <summary>
    /// 获取路由配置（用于前端动态路由）
    /// </summary>
    Task<ApiRequestResult> GetRoutesAsync();

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