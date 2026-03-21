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
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Menu> _menuRepository;

    public RoleService(IRepository<User> userRepository, IRepository<Menu> menuRepository)
    {
        _userRepository = userRepository;
        _menuRepository = menuRepository;
    }

    /// <summary>
    /// 检查用户是否有访问特定菜单的权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="menuId">菜单ID</param>
    /// <returns></returns>
    public async Task<ApiRequestResult> HasMenuPermissionAsync(Guid userId, Guid menuId)
    {
        try
        {
            // 这里应该根据实际的角色/权限模型实现具体逻辑
            // 暂时返回true表示有权限，实际应用中应当查询用户与菜单的关联关系
            var user = await _userRepository.FindAsync(userId);
            if (user == null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户不存在",
                    Data = false
                };
            }

            var menu = await _menuRepository.FindAsync(menuId);
            if (menu == null)
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
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    public async Task<ApiRequestResult> GetUserMenusAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.FindAsync(userId);
            if (user == null)
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
}