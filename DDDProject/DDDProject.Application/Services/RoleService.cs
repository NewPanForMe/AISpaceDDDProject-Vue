using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Repositories;

namespace DDDProject.Application.Services;

/// <summary>
/// 角色应用服务实现
/// </summary>
public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Menu> _menuRepository;
    private readonly IRepository<UserRole> _userRoleRepository;

    public RoleService(
        IRepository<Role> roleRepository,
        IRepository<User> userRepository,
        IRepository<Menu> menuRepository,
        IRepository<UserRole> userRoleRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _menuRepository = menuRepository;
        _userRoleRepository = userRoleRepository;
    }

    /// <summary>
    /// 获取角色列表（分页）
    /// </summary>
    public async Task<ApiRequestResult> GetRolesAsync(PagedRequest request)
    {
        try
        {
            var skipCount = (request.PageNumber - 1) * request.PageSize;
            var total = await _roleRepository.CountAsync(m => true);
            var roles = await _roleRepository.GetListAsync(
                m => true,
                q => q.OrderBy(m => m.SortOrder),
                skipCount,
                request.PageSize
            );

            var dtos = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Code = r.Code,
                Description = r.Description,
                Status = r.Status,
                SortOrder = r.SortOrder,
                Remark = r.Remark,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();

            var pagedResult = new PagedResult<RoleDto>
            {
                List = dtos,
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
                Message = $"获取角色列表失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    public async Task<ApiRequestResult> GetRoleByIdAsync(Guid id)
    {
        try
        {
            var role = await _roleRepository.FindAsync(id);
            if (role is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "角色不存在",
                    Data = null
                };
            }

            var dto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Code = role.Code,
                Description = role.Description,
                Status = role.Status,
                SortOrder = role.SortOrder,
                Remark = role.Remark,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            };

            return new ApiRequestResult
            {
                Success = true,
                Message = "操作成功",
                Data = dto
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取角色详情失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    public async Task<ApiRequestResult> CreateRoleAsync(CreateRoleRequest request)
    {
        try
        {
            // 检查角色名称是否存在
            var existingRole = await _roleRepository.GetFirstAsync(r => r.Name == request.Name);
            if (existingRole is not null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "角色名称已存在",
                    Data = null
                };
            }

            // 检查角色编码是否存在
            existingRole = await _roleRepository.GetFirstAsync(r => r.Code == request.Code);
            if (existingRole is not null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "角色编码已存在",
                    Data = null
                };
            }

            var role = Role.Create(
                request.Name,
                request.Code,
                request.Description,
                request.SortOrder
            );

            role.Update(remark: request.Remark);

            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "创建成功",
                Data = role.Id
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"创建角色失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    public async Task<ApiRequestResult> UpdateRoleAsync(UpdateRoleRequest request)
    {
        try
        {
            var role = await _roleRepository.FindAsync(request.Id);
            if (role is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "角色不存在",
                    Data = null
                };
            }

            // 检查角色名称是否重复
            if (!string.IsNullOrEmpty(request.Name) && request.Name != role.Name)
            {
                var existingRole = await _roleRepository.GetFirstAsync(r => r.Name == request.Name);
                if (existingRole is not null)
                {
                    return new ApiRequestResult
                    {
                        Success = false,
                        Message = "角色名称已存在",
                        Data = null
                    };
                }
            }

            // 检查角色编码是否重复
            if (!string.IsNullOrEmpty(request.Code) && request.Code != role.Code)
            {
                var existingRole = await _roleRepository.GetFirstAsync(r => r.Code == request.Code);
                if (existingRole is not null)
                {
                    return new ApiRequestResult
                    {
                        Success = false,
                        Message = "角色编码已存在",
                        Data = null
                    };
                }
            }

            role.Update(
                request.Name,
                request.Code,
                request.Description,
                request.SortOrder,
                request.Remark
            );

            if (request.Status.HasValue)
            {
                if (request.Status.Value == 1)
                    role.Enable();
                else
                    role.Disable();
            }

            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "更新成功",
                Data = role.Id
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"更新角色失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    public async Task<ApiRequestResult> DeleteRoleAsync(Guid id)
    {
        try
        {
            var role = await _roleRepository.FindAsync(id);
            if (role is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "角色不存在",
                    Data = null
                };
            }

            _roleRepository.Remove(role);
            await _roleRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "删除成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"删除角色失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 启用角色
    /// </summary>
    public async Task<ApiRequestResult> EnableRoleAsync(Guid id)
    {
        try
        {
            var role = await _roleRepository.FindAsync(id);
            if (role is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "角色不存在",
                    Data = null
                };
            }

            role.Enable();
            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "启用成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"启用角色失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 禁用角色
    /// </summary>
    public async Task<ApiRequestResult> DisableRoleAsync(Guid id)
    {
        try
        {
            var role = await _roleRepository.FindAsync(id);
            if (role is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "角色不存在",
                    Data = null
                };
            }

            role.Disable();
            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "禁用成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"禁用角色失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 检查用户是否有访问特定菜单的权限
    /// </summary>
    public async Task<ApiRequestResult> HasMenuPermissionAsync(Guid userId, Guid menuId)
    {
        try
        {
            var user = await _userRepository.FindAsync(userId);
            if (user is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户不存在",
                    Data = false
                };
            }

            var menu = await _menuRepository.FindAsync(menuId);
            if (menu is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "菜单不存在",
                    Data = false
                };
            }

            // 临时实现：假定所有用户都有所有菜单的权限
            // 在实际应用中，应该基于用户角色或权限表进行检查
            var hasPermission = true;

            return new ApiRequestResult
            {
                Success = true,
                Message = "权限检查完成",
                Data = hasPermission
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"权限检查失败: {ex.Message}",
                Data = false
            };
        }
    }

    /// <summary>
    /// 获取用户的菜单权限列表
    /// </summary>
    public async Task<ApiRequestResult> GetUserMenusAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.FindAsync(userId);
            if (user is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户不存在",
                    Data = null
                };
            }

            // 在实际应用中，这里应当查询用户权限表以获取用户有权访问的菜单列表
            // 暂时简单返回所有启用的菜单
            var userMenus = await _menuRepository.GetListAsync(m => m.Status == 1);

            return new ApiRequestResult
            {
                Success = true,
                Message = "获取用户菜单成功",
                Data = userMenus
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
    /// 获取用户的角色ID列表
    /// </summary>
    public async Task<ApiRequestResult> GetUserRoleIdsAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.FindAsync(userId);
            if (user is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户不存在",
                    Data = null
                };
            }

            var userRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == userId);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            return new ApiRequestResult
            {
                Success = true,
                Message = "获取用户角色成功",
                Data = roleIds
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取用户角色失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 配置用户角色
    /// </summary>
    public async Task<ApiRequestResult> AssignUserRolesAsync(Guid userId, List<Guid> roleIds)
    {
        try
        {
            var user = await _userRepository.FindAsync(userId);
            if (user is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户不存在",
                    Data = null
                };
            }

            // 获取用户当前的角色关联
            var existingUserRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == userId);
            var existingRoleIds = existingUserRoles.Select(ur => ur.RoleId).ToList();

            // 需要删除的角色（原有但新列表中没有的）
            var roleIdsToRemove = existingRoleIds.Except(roleIds).ToList();
            // 需要添加的角色（新列表中有但原来没有的）
            var roleIdsToAdd = roleIds.Except(existingRoleIds).ToList();

            // 删除不再需要的角色关联
            foreach (var roleId in roleIdsToRemove)
            {
                var userRole = existingUserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
                if (userRole is not null)
                {
                    _userRoleRepository.Remove(userRole);
                }
            }

            // 添加新的角色关联
            foreach (var roleId in roleIdsToAdd)
            {
                // 验证角色是否存在且启用
                var role = await _roleRepository.FindAsync(roleId);
                if (role is not null && role.Status == 1)
                {
                    var newUserRole = UserRole.Create(userId, roleId);
                    await _userRoleRepository.AddAsync(newUserRole);
                }
            }

            await _userRoleRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "配置用户角色成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"配置用户角色失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 获取所有启用的角色列表（用于下拉选择）
    /// </summary>
    public async Task<ApiRequestResult> GetEnabledRolesAsync()
    {
        try
        {
            var roles = await _roleRepository.GetListAsync(r => r.Status == 1);

            // 按排序号排序
            var sortedRoles = roles.OrderBy(r => r.SortOrder).ToList();

            var dtos = sortedRoles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Code = r.Code,
                Description = r.Description,
                Status = r.Status,
                SortOrder = r.SortOrder,
                Remark = r.Remark,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();

            return new ApiRequestResult
            {
                Success = true,
                Message = "获取角色列表成功",
                Data = dtos
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取角色列表失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 获取角色的用户ID列表
    /// </summary>
    public async Task<ApiRequestResult> GetRoleUserIdsAsync(Guid roleId)
    {
        try
        {
            var userRoles = await _userRoleRepository.GetListAsync(ur => ur.RoleId == roleId);
            var userIds = userRoles.Select(ur => ur.UserId).ToList();

            return new ApiRequestResult
            {
                Success = true,
                Message = "获取角色用户列表成功",
                Data = userIds
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取角色用户列表失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 为角色分配用户
    /// </summary>
    public async Task<ApiRequestResult> AssignRoleUsersAsync(Guid roleId, List<Guid> userIds)
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

            // 获取该角色现有的用户关联
            var existingUserRoles = await _userRoleRepository.GetListAsync(ur => ur.RoleId == roleId);
            var existingUserIds = existingUserRoles.Select(ur => ur.UserId).ToHashSet();

            // 需要添加的用户
            var userIdsToAdd = userIds.Where(uid => !existingUserIds.Contains(uid)).ToList();

            // 需要删除的用户
            var userIdsToRemove = existingUserIds.Where(uid => !userIds.Contains(uid)).ToList();

            // 添加新的用户角色关联
            foreach (var userId in userIdsToAdd)
            {
                var userRole = UserRole.Create(userId, roleId);
                await _userRoleRepository.AddAsync(userRole);
            }

            // 删除不再需要的用户角色关联
            foreach (var userRole in existingUserRoles.Where(ur => userIdsToRemove.Contains(ur.UserId)))
            {
                _userRoleRepository.Remove(userRole);
            }

            await _userRoleRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "配置角色用户成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"配置角色用户失败: {ex.Message}",
                Data = null
            };
        }
    }
}