using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Repositories;

namespace DDDProject.Application.Services;

/// <summary>
/// 菜单应用服务实现
/// </summary>
public class MenuService : IMenuService
{
    private readonly IRepository<Menu> _menuRepository;

    public MenuService(IRepository<Menu> menuRepository)
    {
        _menuRepository = menuRepository;
    }

    /// <summary>
    /// 获取菜单列表（分页）
    /// </summary>
    public async Task<ApiRequestResult> GetMenusAsync(PagedRequest request)
    {
        try
        {
            // 计算跳过的记录数
            var skipCount = (request.PageNumber - 1) * request.PageSize;

            // 获取总记录数
            var total = await _menuRepository.CountAsync(m => true);

            // 获取当前页的数据
            var menus = await _menuRepository.GetListAsync(
                m => true,
                q => q.OrderBy(m => m.SortOrder),
                skipCount,
                request.PageSize
            );

            var menuDtos = menus.Select(m => new MenuDto
            {
                Id = m.Id,
                Name = m.Name,
                Path = m.Path,
                Component = m.Component,
                Icon = m.Icon,
                ParentId = m.ParentId,
                SortOrder = m.SortOrder,
                Status = m.Status
            }).ToList();

            var pagedResult = new PagedResult<MenuDto>
            {
                List = menuDtos.OrderBy(m => m.SortOrder).ToList(),
                Total = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return new ApiRequestResult
            {
                Success = true,
                Message = "操作成功",
                Data = pagedResult
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取菜单列表失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 获取菜单详情
    /// </summary>
    public async Task<ApiRequestResult> GetMenuByIdAsync(Guid id)
    {
        try
        {
            var menu = await _menuRepository.FindAsync(id);

            if (menu == null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "菜单不存在",
                    Data = null
                };
            }

            var menuDto = new MenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Path = menu.Path,
                Component = menu.Component,
                Icon = menu.Icon,
                ParentId = menu.ParentId,
                SortOrder = menu.SortOrder,
                Status = menu.Status
            };

            return new ApiRequestResult
            {
                Success = true,
                Message = "操作成功",
                Data = menuDto
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取菜单详情失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 获取树形结构的菜单（用于侧边栏菜单，无需分页）
    /// </summary>
    public async Task<ApiRequestResult> GetSidebarMenusAsync()
    {
        try
        {
            var menus = await _menuRepository.GetListAsync(m => true);
            var menuDtos = BuildTreeMenu(menus.ToList());

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
    /// 根据用户获取菜单树形结构
    /// </summary>
    public async Task<ApiRequestResult> GetUserMenuTreeAsync(Guid userId)
    {
        try
        {
            // 这里可以根据用户ID获取用户的菜单权限
            // 暂时返回所有启用的菜单，实际项目中应当实现基于用户角色或权限的过滤
            var menus = await _menuRepository.GetListAsync(m => m.Status == 1); // 只获取启用的菜单
            var menuDtos = BuildTreeMenu(menus.ToList());

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
                Message = $"获取用户菜单树失败: {ex.Message}",
                Data = null
            };
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

            // 构建树形结构
            var menuDtos = BuildTreeMenu(allMenus);

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
            if (existingMenu != null)
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
            if (existingMenu == null)
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

            if (otherMenu != null)
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
                menuDto.SortOrder
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
            if (menu == null)
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
            if (menu == null)
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
            if (menu == null)
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
    /// 构建树形菜单结构
    /// </summary>
    private List<MenuDto> BuildTreeMenu(List<Menu> allMenus)
    {
        // 获取根节点
        var rootMenus = allMenus.Where(m => m.ParentId == null || m.ParentId == Guid.Empty)
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
            Children = children.Any() ? children.Select(m => BuildMenuDto(m, allMenus)).ToList() : new List<MenuDto>()
        };
    }
}
