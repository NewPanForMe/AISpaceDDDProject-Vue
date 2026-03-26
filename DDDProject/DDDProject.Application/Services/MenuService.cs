using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Models;
using DDDProject.Domain.Repositories;

namespace DDDProject.Application.Services;

/// <summary>
/// 菜单应用服务实现
/// </summary>
public class MenuService : IMenuService
{
    private readonly IRepository<Menu> _menuRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<MenuRole> _menuRoleRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public MenuService(
        IRepository<Menu> menuRepository,
        IRepository<Role> roleRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<MenuRole> menuRoleRepository,
        ICurrentUserContext currentUserContext)
    {
        _menuRepository = menuRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _menuRoleRepository = menuRoleRepository;
        _currentUserContext = currentUserContext;
    }

    /// <summary>
    /// 获取树形结构的菜单（用于侧边栏菜单，根据用户角色过滤）
    /// </summary>
    public async Task<ApiRequestResult> GetSidebarMenusAsync()
    {
        try
        {
            var userId = _currentUserContext.UserId;

            // 如果用户未登录，返回空菜单
            if (userId == Guid.Empty)
            {
                return new ApiRequestResult
                {
                    Success = true,
                    Message = "操作成功",
                    Data = new List<MenuDto>()
                };
            }

            // 获取用户角色
            var userRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == userId);
            var userRoleList = userRoles.ToList();

            // 如果用户没有角色，返回空菜单
            if (userRoleList.Count == 0)
            {
                return new ApiRequestResult
                {
                    Success = true,
                    Message = "操作成功",
                    Data = new List<MenuDto>()
                };
            }

            var roleIds = userRoleList.Select(ur => ur.RoleId).ToList();

            // 获取启用的角色
            var roles = await _roleRepository.GetListAsync(r => roleIds.Contains(r.Id) && r.Status == 1);
            var roleList = roles.ToList();

            // 检查是否是超级管理员
            var isSuperAdmin = roleList.Any(r => r.Code == "SUPER_ADMIN");

            if (isSuperAdmin)
            {
                // 超级管理员返回所有启用的菜单
                var allMenus = await _menuRepository.GetListAsync(m => m.Status == 1);
                var menuDtos = BuildTreeMenu(allMenus.ToList());

                return new ApiRequestResult
                {
                    Success = true,
                    Message = "操作成功",
                    Data = menuDtos
                };
            }

            var enabledRoleIds = roleList.Select(r => r.Id).ToList();

            // 如果没有启用的角色，返回空菜单
            if (enabledRoleIds.Count == 0)
            {
                return new ApiRequestResult
                {
                    Success = true,
                    Message = "操作成功",
                    Data = new List<MenuDto>()
                };
            }

            // 获取角色关联的菜单ID
            var menuRoles = await _menuRoleRepository.GetListAsync(mr => enabledRoleIds.Contains(mr.RoleId));
            var menuIds = menuRoles.Select(mr => mr.MenuId).Distinct().ToList();

            // 如果没有菜单权限，返回空菜单
            if (menuIds.Count == 0)
            {
                return new ApiRequestResult
                {
                    Success = true,
                    Message = "操作成功",
                    Data = new List<MenuDto>()
                };
            }

            // 获取所有启用的菜单
            var allMenus2 = await _menuRepository.GetListAsync(m => m.Status == 1);
            var menuList = allMenus2.ToList();

            // 获取用户有权限的菜单及其所有父级菜单
            var authorizedMenuIds = GetAuthorizedMenuIdsWithParents(menuList, menuIds);

            // 过滤出用户有权限的菜单
            var filteredMenus = menuList.Where(m => authorizedMenuIds.Contains(m.Id)).ToList();

            // 构建树形结构
            var menuDtos2 = BuildTreeMenu(filteredMenus);

            return new ApiRequestResult
            {
                Success = true,
                Message = "操作成功",
                Data = menuDtos2
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取侧边栏菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 根据菜单ID列表获取树形结构的菜单（用于侧边栏菜单，根据用户权限过滤）
    /// </summary>
    /// <param name="menuIds">菜单ID列表</param>
    public async Task<ApiRequestResult> GetSidebarMenusByMenuIdsAsync(List<Guid> menuIds)
    {
        try
        {
            // 如果没有菜单权限，返回空列表
            if (menuIds is null || !menuIds.Any())
            {
                return new ApiRequestResult
                {
                    Success = true,
                    Message = "操作成功",
                    Data = new List<MenuDto>()
                };
            }

            // 获取所有启用的菜单
            var allMenus = await _menuRepository.GetListAsync(m => m.Status == 1);
            var menuList = allMenus.ToList();

            // 获取用户有权限的菜单及其所有父级菜单
            var authorizedMenuIds = GetAuthorizedMenuIdsWithParents(menuList, menuIds);

            // 过滤出用户有权限的菜单
            var filteredMenus = menuList.Where(m => authorizedMenuIds.Contains(m.Id)).ToList();

            // 构建树形结构
            var menuDtos = BuildTreeMenu(filteredMenus);

            return new ApiRequestResult
            {
                Success = true,
                Message = "操作成功",
                Data = menuDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取侧边栏菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 获取用户有权限的菜单ID及其所有父级菜单ID
    /// </summary>
    /// <param name="allMenus">所有菜单列表</param>
    /// <param name="authorizedMenuIds">用户有权限的菜单ID列表</param>
    /// <returns>包含父级菜单的完整菜单ID集合</returns>
    private HashSet<Guid> GetAuthorizedMenuIdsWithParents(List<Menu> allMenus, List<Guid> authorizedMenuIds)
    {
        var result = new HashSet<Guid>(authorizedMenuIds);

        // 递归添加所有父级菜单
        foreach (var menuId in authorizedMenuIds)
        {
            AddParentMenuIds(allMenus, menuId, result);
        }

        return result;
    }

    /// <summary>
    /// 递归添加父级菜单ID
    /// </summary>
    /// <param name="allMenus">所有菜单列表</param>
    /// <param name="menuId">当前菜单ID</param>
    /// <param name="result">结果集合</param>
    private void AddParentMenuIds(List<Menu> allMenus, Guid menuId, HashSet<Guid> result)
    {
        var menu = allMenus.FirstOrDefault(m => m.Id == menuId);
        if (menu is not null && menu.ParentId.HasValue && menu.ParentId.Value != Guid.Empty)
        {
            // 添加父级菜单ID
            if (result.Add(menu.ParentId.Value))
            {
                // 递归添加更上层的父级
                AddParentMenuIds(allMenus, menu.ParentId.Value, result);
            }
        }
    }

    /// <summary>
    /// 获取分页的树形菜单数据（用于大数据量场景）
    /// </summary>
    public async Task<ApiRequestResult> GetPagedTreeMenusAsync(PagedRequest request)
    {
        try
        {
            var allMenus = new List<Menu>();
            var currentPage = request.PageNumber;

            // 循环分页获取所有数据
            while (true)
            {
                var menus = await _menuRepository.GetListAsync(
                    m => true,
                    q => q.OrderBy(x => x.SortOrder),
                    (currentPage - 1) * request.PageSize,
                    request.PageSize
                );

                if (!menus.Any()) break;

                allMenus.AddRange(menus);

                if (menus.Count() < request.PageSize) break;
                currentPage++;
            }

            // 构建树形结构（包含禁用的菜单）
            var menuDtos = BuildTreeMenuWithDisabled(allMenus);

            return new ApiRequestResult
            {
                Success = true,
                Message = "操作成功",
                Data = new
                {
                    List = menuDtos,
                    Total = allMenus.Count,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取分页树形菜单失败: {ex.Message}",
                Data = null
            };
        }
    }



    /// <summary>
    /// 创建菜单
    /// </summary>
    public async Task<ApiRequestResult> CreateMenuAsync(MenuDto menuDto)
    {
        try
        {
            // 验证菜单名称是否已存在
            var existingMenu = await _menuRepository.GetFirstAsync(m => m.Name == menuDto.Name && m.ParentId == menuDto.ParentId);
            if (existingMenu is not null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "同级别菜单名称已存在",
                    Data = null
                };
            }

            var menu = Menu.Create(
                menuDto.Name,
                menuDto.Path,
                menuDto.Component,
                menuDto.Icon,
                menuDto.ParentId,
                menuDto.SortOrder,
                menuDto.Status
            );

            await _menuRepository.AddAsync(menu);
            await _menuRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "菜单创建成功",
                Data = menu.Id
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"创建菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    public async Task<ApiRequestResult> UpdateMenuAsync(MenuDto menuDto)
    {
        try
        {
            if (!menuDto.Id.HasValue)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "菜单ID不能为空",
                    Data = null
                };
            }

            var existingMenu = await _menuRepository.FindAsync(menuDto.Id.Value);
            if (existingMenu is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "菜单不存在",
                    Data = null
                };
            }

            // 验证菜单名称是否已被其他菜单使用
            var otherMenu = await _menuRepository.GetFirstAsync(m =>
                m.Name == menuDto.Name &&
                m.Id != menuDto.Id.Value &&
                m.ParentId == menuDto.ParentId);

            if (otherMenu is not  null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "同级别菜单名称已存在",
                    Data = null
                };
            }

            existingMenu.Update(
                menuDto.Name,
                menuDto.Path,
                menuDto.Component,
                menuDto.Icon,
                menuDto.ParentId,
                menuDto.SortOrder,
                menuDto.Status
            );

            _menuRepository.Update(existingMenu);
            await _menuRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "菜单更新成功",
                Data = existingMenu.Id
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"更新菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    public async Task<ApiRequestResult> DeleteMenuAsync(Guid id)
    {
        try
        {
            var menu = await _menuRepository.FindAsync(id);
            if (menu is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "菜单不存在",
                    Data = null
                };
            }

            // 检查是否存在子菜单
            var childMenus = await _menuRepository.GetListAsync(m => m.ParentId == id);
            if (childMenus.Any())
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "存在子菜单，无法删除",
                    Data = null
                };
            }

            _menuRepository.Remove(menu);
            await _menuRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "菜单删除成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"删除菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 启用菜单
    /// </summary>
    public async Task<ApiRequestResult> EnableMenuAsync(Guid id)
    {
        try
        {
            var menu = await _menuRepository.FindAsync(id);
            if (menu is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "菜单不存在",
                    Data = null
                };
            }

            menu.Enable();
            _menuRepository.Update(menu);
            await _menuRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "菜单启用成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"启用菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 禁用菜单
    /// </summary>
    public async Task<ApiRequestResult> DisableMenuAsync(Guid id)
    {
        try
        {
            var menu = await _menuRepository.FindAsync(id);
            if (menu is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "菜单不存在",
                    Data = null
                };
            }

            // 检查是否存在子菜单
            var childMenus = await _menuRepository.GetListAsync(m => m.ParentId == id && m.Status == 1);
            if (childMenus.Any())
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "存在启用的子菜单，无法禁用该菜单",
                    Data = null
                };
            }

            menu.Disable();
            _menuRepository.Update(menu);
            await _menuRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "菜单禁用成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"禁用菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 构建扁平菜单列表（用于单层路由）
    /// </summary>
    private List<MenuDto> BuildFlatMenu(List<Menu> allMenus)
    {
        // 获取所有菜单并排序
        var menuDtos = allMenus.OrderBy(m => m.SortOrder).Select(m => new MenuDto
        {
            Id = m.Id,
            Name = m.Name,
            Path = m.Path,
            Component = m.Component,
            Icon = m.Icon,
            ParentId = m.ParentId,
            SortOrder = m.SortOrder,
            Status = m.Status,
            Children = null  // 扁平结构，不包含子菜单
        }).ToList();

        return menuDtos;
    }

    /// <summary>
    /// 构建树形菜单结构（用于列表显示，包含禁用的菜单）
    /// </summary>
    private List<MenuDto> BuildTreeMenuWithDisabled(List<Menu> allMenus)
    {
        // 获取根节点（包含所有菜单）
        var rootMenus = allMenus.Where(m => m.ParentId is null || m.ParentId == Guid.Empty)
                               .OrderBy(m => m.SortOrder)
                               .ToList();

        var result = new List<MenuDto>();

        foreach (var menu in rootMenus)
        {
            var menuDto = BuildMenuDtoWithDisabled(menu, allMenus);
            result.Add(menuDto);
        }

        return result;
    }

    /// <summary>
    /// 递归构建菜单 DTO（包含禁用的菜单）
    /// </summary>
    private MenuDto BuildMenuDtoWithDisabled(Menu menu, List<Menu> allMenus)
    {
        // 获取所有子菜单（包含禁用的）
        var children = allMenus.Where(m => m.ParentId == menu.Id).OrderBy(m => m.SortOrder).ToList();

        return new MenuDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Path = menu.Path,
            Component = menu.Component,
            Icon = menu.Icon,
            ParentId = menu.ParentId,
            SortOrder = menu.SortOrder,
            Status = menu.Status,
            Children = children.Any() ? children.Select(m => BuildMenuDtoWithDisabled(m, allMenus)).ToList() : new List<MenuDto>()
        };
    }

    /// <summary>
    /// 构建树形菜单结构
    /// </summary>
    private List<MenuDto> BuildTreeMenu(List<Menu> allMenus)
    {
        // 获取根节点（只包含启用的菜单）
        var rootMenus = allMenus.Where(m => (m.ParentId is null || m.ParentId == Guid.Empty) && m.Status == 1)
                               .OrderBy(m => m.SortOrder)
                               .ToList();

        var result = new List<MenuDto>();

        foreach (var menu in rootMenus)
        {
            var menuDto = BuildMenuDto(menu, allMenus);
            result.Add(menuDto);
        }

        return result;
    }

    /// <summary>
    /// 递归构建菜单 DTO
    /// </summary>
    private MenuDto BuildMenuDto(Menu menu, List<Menu> allMenus)
    {
        // 只获取启用的子菜单
        var children = allMenus.Where(m => m.ParentId == menu.Id && m.Status == 1).OrderBy(m => m.SortOrder).ToList();

        return new MenuDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Path = menu.Path,
            Component = menu.Component,
            Icon = menu.Icon,
            ParentId = menu.ParentId,
            SortOrder = menu.SortOrder,
            Status = menu.Status,
            Children = children.Any() ? children.Select(m => BuildMenuDto(m, allMenus)).ToList() : new List<MenuDto>()
        };
    }

    /// <summary>
    /// 获取路由配置（用于前端动态路由）
    /// </summary>
    public async Task<ApiRequestResult> GetRoutesAsync()
    {
        try
        {
            // 只获取启用的菜单
            var menus = await _menuRepository.GetListAsync(m => m.Status == 1);

            // 构建路由配置列表
            var routeConfigs = BuildRouteConfigs(menus.ToList());

            return new ApiRequestResult
            {
                Success = true,
                Message = "操作成功",
                Data = routeConfigs
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取路由配置失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 构建路由配置列表
    /// </summary>
    private List<RouteConfig> BuildRouteConfigs(List<Menu> allMenus)
    {
        // 获取根节点（只包含启用的菜单）
        var rootMenus = allMenus.Where(m => (m.ParentId is null || m.ParentId == Guid.Empty) && m.Status == 1)
                               .OrderBy(m => m.SortOrder)
                               .ToList();

        var result = new List<RouteConfig>();

        foreach (var menu in rootMenus)
        {
            var routeConfig = BuildRouteConfig(menu, allMenus);
            result.Add(routeConfig);
        }

        return result;
    }

    /// <summary>
    /// 递归构建路由配置
    /// </summary>
    private RouteConfig BuildRouteConfig(Menu menu, List<Menu> allMenus)
    {
        // 只获取启用的子菜单
        var children = allMenus.Where(m => m.ParentId == menu.Id && m.Status == 1).OrderBy(m => m.SortOrder).ToList();

        return new RouteConfig
        {
            Path = menu.Path,
            Name = menu.Name,
            Component = menu.Component,
            Icon = menu.Icon,
            ParentId = menu.ParentId,
            SortOrder = menu.SortOrder,
            Status = menu.Status,
            Children = children.Any() ? children.Select(m => BuildRouteConfig(m, allMenus)).ToList() : null
        };
    }
}
