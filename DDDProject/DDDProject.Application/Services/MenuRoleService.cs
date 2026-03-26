using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Repositories;

namespace DDDProject.Application.Services;

/// <summary>
/// 菜单角色应用服务实现
/// </summary>
public class MenuRoleService : IMenuRoleService
{
    private readonly IRepository<MenuRole> _menuRoleRepository;
    private readonly IRepository<Menu> _menuRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<UserRole> _userRoleRepository;

    public MenuRoleService(
        IRepository<MenuRole> menuRoleRepository,
        IRepository<Menu> menuRepository,
        IRepository<Role> roleRepository,
        IRepository<UserRole> userRoleRepository)
    {
        _menuRoleRepository = menuRoleRepository;
        _menuRepository = menuRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
    }

    /// <summary>
    /// 获取角色的菜单ID列表
    /// </summary>
    public async Task<ApiRequestResult> GetRoleMenuIdsAsync(Guid roleId)
    {
        try
        {
            var menuRoles = await _menuRoleRepository.GetListAsync(mr => mr.RoleId == roleId);
            var menuIds = menuRoles.Select(mr => mr.MenuId).ToList();

            return new ApiRequestResult
            {
                Success = true,
                Message = "获取角色菜单成功",
                Data = menuIds
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取角色菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 根据用户ID获取菜单ID列表（通过用户角色关联）
    /// </summary>
    public async Task<ApiRequestResult> GetRoleMenuIdsByUserIdAsync(Guid userId)
    {
        try
        {
            // 获取用户的所有角色
            var userRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == userId);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            if (!roleIds.Any())
            {
                return new ApiRequestResult
                {
                    Success = true,
                    Message = "用户无角色",
                    Data = new List<Guid>()
                };
            }

            // 获取启用的角色
            var roles = await _roleRepository.GetListAsync(r => roleIds.Contains(r.Id) && r.Status == 1);
            var enabledRoleIds = roles.Select(r => r.Id).ToList();

            // 检查是否是超级管理员
            var isSuperAdmin = roles.Any(r => r.Code == "SUPER_ADMIN");

            if (isSuperAdmin)
            {
                // 超级管理员返回所有启用的菜单
                var allMenus = await _menuRepository.GetListAsync(m => m.Status == 1);
                var allMenuIds = allMenus.Select(m => m.Id).ToList();

                return new ApiRequestResult
                {
                    Success = true,
                    Message = "获取用户菜单成功",
                    Data = allMenuIds
                };
            }

            if (!enabledRoleIds.Any())
            {
                return new ApiRequestResult
                {
                    Success = true,
                    Message = "用户无启用的角色",
                    Data = new List<Guid>()
                };
            }

            // 获取这些角色的所有菜单ID
            var menuRoles = await _menuRoleRepository.GetListAsync(mr => enabledRoleIds.Contains(mr.RoleId));
            var menuIds = menuRoles.Select(mr => mr.MenuId).Distinct().ToList();

            return new ApiRequestResult
            {
                Success = true,
                Message = "获取用户菜单成功",
                Data = menuIds
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取用户菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 获取菜单的角色ID列表
    /// </summary>
    public async Task<ApiRequestResult> GetMenuRoleIdsAsync(Guid menuId)
    {
        try
        {
            var menuRoles = await _menuRoleRepository.GetListAsync(mr => mr.MenuId == menuId);
            var roleIds = menuRoles.Select(mr => mr.RoleId).ToList();

            return new ApiRequestResult
            {
                Success = true,
                Message = "获取菜单角色成功",
                Data = roleIds
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取菜单角色失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 为角色分配菜单
    /// </summary>
    public async Task<ApiRequestResult> AssignRoleMenusAsync(Guid roleId, List<Guid> menuIds)
    {
        try
        {
            // 检查角色是否存在
            var role = await _roleRepository.FindAsync(roleId);
            if (role is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "角色不存在",
                    Data = null
                };
            }

            // 获取该角色现有的菜单关联
            var existingMenuRoles = await _menuRoleRepository.GetListAsync(mr => mr.RoleId == roleId);
            var existingMenuIds = existingMenuRoles.Select(mr => mr.MenuId).ToHashSet();

            // 需要添加的菜单
            var menuIdsToAdd = menuIds.Where(mid => !existingMenuIds.Contains(mid)).ToList();

            // 需要删除的菜单
            var menuIdsToRemove = existingMenuIds.Where(mid => !menuIds.Contains(mid)).ToList();

            // 添加新的菜单角色关联
            foreach (var menuId in menuIdsToAdd)
            {
                // 验证菜单是否存在且启用
                var menu = await _menuRepository.FindAsync(menuId);
                if (menu is not null && menu.Status == 1)
                {
                    var menuRole = MenuRole.Create(menuId, roleId);
                    await _menuRoleRepository.AddAsync(menuRole);
                }
            }

            // 删除不再需要的菜单角色关联
            foreach (var menuRole in existingMenuRoles.Where(mr => menuIdsToRemove.Contains(mr.MenuId)))
            {
                _menuRoleRepository.Remove(menuRole);
            }

            await _menuRoleRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "分配角色菜单成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"分配角色菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 为菜单分配角色
    /// </summary>
    public async Task<ApiRequestResult> AssignMenuRolesAsync(Guid menuId, List<Guid> roleIds)
    {
        try
        {
            // 检查菜单是否存在
            var menu = await _menuRepository.FindAsync(menuId);
            if (menu is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "菜单不存在",
                    Data = null
                };
            }

            // 获取该菜单现有的角色关联
            var existingMenuRoles = await _menuRoleRepository.GetListAsync(mr => mr.MenuId == menuId);
            var existingRoleIds = existingMenuRoles.Select(mr => mr.RoleId).ToHashSet();

            // 需要添加的角色
            var roleIdsToAdd = roleIds.Where(rid => !existingRoleIds.Contains(rid)).ToList();

            // 需要删除的角色
            var roleIdsToRemove = existingRoleIds.Where(rid => !roleIds.Contains(rid)).ToList();

            // 添加新的菜单角色关联
            foreach (var roleId in roleIdsToAdd)
            {
                // 验证角色是否存在且启用
                var role = await _roleRepository.FindAsync(roleId);
                if (role is not null && role.Status == 1)
                {
                    var menuRole = MenuRole.Create(menuId, roleId);
                    await _menuRoleRepository.AddAsync(menuRole);
                }
            }

            // 删除不再需要的菜单角色关联
            foreach (var menuRole in existingMenuRoles.Where(mr => roleIdsToRemove.Contains(mr.RoleId)))
            {
                _menuRoleRepository.Remove(menuRole);
            }

            await _menuRoleRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "分配菜单角色成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"分配菜单角色失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 获取角色的菜单列表（树形结构）
    /// </summary>
    public async Task<ApiRequestResult> GetRoleMenusAsync(Guid roleId)
    {
        try
        {
            // 获取角色的所有菜单ID
            var menuRoles = await _menuRoleRepository.GetListAsync(mr => mr.RoleId == roleId);
            var menuIds = menuRoles.Select(mr => mr.MenuId).ToList();

            // 获取菜单详情
            var menus = await _menuRepository.GetListAsync(m => menuIds.Contains(m.Id) && m.Status == 1);
            var menuList = menus.ToList();

            // 构建树形结构
            var treeMenus = BuildMenuTree(menuList, null);

            return new ApiRequestResult
            {
                Success = true,
                Message = "获取角色菜单成功",
                Data = treeMenus
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取角色菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 获取用户的所有菜单（通过角色关联）
    /// </summary>
    public async Task<ApiRequestResult> GetUserMenusByRolesAsync(Guid userId)
    {
        try
        {
            // 获取用户的所有角色
            var userRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == userId);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            if (!roleIds.Any())
            {
                return new ApiRequestResult
                {
                    Success = true,
                    Message = "用户无角色",
                    Data = new List<MenuDto>()
                };
            }

            // 获取这些角色的所有菜单ID
            var menuRoles = await _menuRoleRepository.GetListAsync(mr => roleIds.Contains(mr.RoleId));
            var menuIds = menuRoles.Select(mr => mr.MenuId).Distinct().ToList();

            // 获取菜单详情
            var menus = await _menuRepository.GetListAsync(m => menuIds.Contains(m.Id) && m.Status == 1);
            var menuList = menus.ToList();

            // 构建树形结构
            var treeMenus = BuildMenuTree(menuList, null);

            return new ApiRequestResult
            {
                Success = true,
                Message = "获取用户菜单成功",
                Data = treeMenus
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取用户菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 清除角色的所有菜单权限
    /// </summary>
    public async Task<ApiRequestResult> ClearRoleMenusAsync(Guid roleId)
    {
        try
        {
            var menuRoles = await _menuRoleRepository.GetListAsync(mr => mr.RoleId == roleId);

            foreach (var menuRole in menuRoles)
            {
                _menuRoleRepository.Remove(menuRole);
            }

            await _menuRoleRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "清除角色菜单成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"清除角色菜单失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 清除菜单的所有角色关联
    /// </summary>
    public async Task<ApiRequestResult> ClearMenuRolesAsync(Guid menuId)
    {
        try
        {
            var menuRoles = await _menuRoleRepository.GetListAsync(mr => mr.MenuId == menuId);

            foreach (var menuRole in menuRoles)
            {
                _menuRoleRepository.Remove(menuRole);
            }

            await _menuRoleRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "清除菜单角色成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"清除菜单角色失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 构建菜单树形结构
    /// </summary>
    private List<MenuDto> BuildMenuTree(List<Menu> allMenus, Guid? parentId)
    {
        var result = new List<MenuDto>();

        var children = allMenus
            .Where(m => m.ParentId == parentId)
            .OrderBy(m => m.SortOrder)
            .ToList();

        foreach (var menu in children)
        {
            var menuDto = new MenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Path = menu.Path,
                Component = menu.Component,
                Icon = menu.Icon,
                ParentId = menu.ParentId,
                SortOrder = menu.SortOrder,
                Status = menu.Status,
                Children = BuildMenuTree(allMenus, menu.Id)
            };

            result.Add(menuDto);
        }

        return result;
    }
}