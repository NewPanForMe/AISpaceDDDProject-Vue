using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Repositories;

namespace DDDProject.Application.Services;

/// <summary>
/// 按钮应用服务实现
/// </summary>
public class ButtonService : IButtonService
{
    private readonly IRepository<Button> _buttonRepository;
    private readonly IRepository<Menu> _menuRepository;

    public ButtonService(
        IRepository<Button> buttonRepository,
        IRepository<Menu> menuRepository)
    {
        _buttonRepository = buttonRepository;
        _menuRepository = menuRepository;
    }

    /// <summary>
    /// 获取按钮列表（分页）
    /// </summary>
    public async Task<ApiRequestResult> GetButtonsAsync(PagedRequest request, Guid? menuId = null)
    {
        try
        {
            var skipCount = (request.PageNumber - 1) * request.PageSize;

            // 构建筛选条件
            var buttons = await _buttonRepository.GetListAsync(
                b => menuId.HasValue ? b.MenuId == menuId.Value : true,
                q => q.OrderBy(b => b.MenuId).ThenBy(b => b.SortOrder),
                skipCount,
                request.PageSize
            );

            var buttonList = buttons.ToList();

            // 获取总数
            var total = await _buttonRepository.CountAsync(
                b => menuId.HasValue ? b.MenuId == menuId.Value : true
            );

            // 获取所有相关菜单
            var menuIds = buttonList.Select(b => b.MenuId).Distinct().ToList();
            var menus = await _menuRepository.GetListAsync(m => menuIds.Contains(m.Id));
            var menuDict = menus.ToDictionary(m => m.Id, m => m.Name);

            var dtos = buttonList.Select(b => new ButtonDto
            {
                Id = b.Id,
                Name = b.Name,
                Code = b.Code,
                MenuId = b.MenuId,
                MenuName = menuDict.GetValueOrDefault(b.MenuId),
                PermissionCode = b.PermissionCode,
                Icon = b.Icon,
                SortOrder = b.SortOrder,
                Status = b.Status,
                Description = b.Description,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList();

            var pagedResult = new PagedResult<ButtonDto>
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
                Message = $"获取按钮列表失败：{ex.Message}"
            };
        }
    }

    /// <summary>
    /// 根据ID获取按钮详情
    /// </summary>
    public async Task<ApiRequestResult> GetButtonByIdAsync(Guid id)
    {
        try
        {
            var button = await _buttonRepository.FindAsync(id);
            if (button is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "按钮不存在"
                };
            }

            // 获取所属菜单名称
            var menu = await _menuRepository.FindAsync(button.MenuId);

            var dto = new ButtonDto
            {
                Id = button.Id,
                Name = button.Name,
                Code = button.Code,
                MenuId = button.MenuId,
                MenuName = menu?.Name,
                PermissionCode = button.PermissionCode,
                Icon = button.Icon,
                SortOrder = button.SortOrder,
                Status = button.Status,
                Description = button.Description,
                CreatedAt = button.CreatedAt,
                UpdatedAt = button.UpdatedAt
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
                Message = $"获取按钮详情失败：{ex.Message}"
            };
        }
    }

    /// <summary>
    /// 根据菜单ID获取按钮列表
    /// </summary>
    public async Task<ApiRequestResult> GetButtonsByMenuIdAsync(Guid menuId)
    {
        try
        {
            var buttons = await _buttonRepository.GetListAsync(
                b => b.MenuId == menuId && b.Status == 1
            );

            var dtos = buttons.Select(b => new ButtonDto
            {
                Id = b.Id,
                Name = b.Name,
                Code = b.Code,
                MenuId = b.MenuId,
                PermissionCode = b.PermissionCode,
                Icon = b.Icon,
                SortOrder = b.SortOrder,
                Status = b.Status,
                Description = b.Description,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList();

            return new ApiRequestResult
            {
                Success = true,
                Message = "操作成功",
                Data = dtos
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取按钮列表失败：{ex.Message}"
            };
        }
    }

    /// <summary>
    /// 创建按钮
    /// </summary>
    public async Task<ApiRequestResult> CreateButtonAsync(CreateButtonRequest request)
    {
        try
        {
            // 检查按钮编码是否已存在
            var existingButton = await _buttonRepository.GetFirstAsync(b => b.Code == request.Code);
            if (existingButton is not null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "按钮编码已存在"
                };
            }

            // 检查菜单是否存在
            var menu = await _menuRepository.FindAsync(request.MenuId);
            if (menu is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "所属菜单不存在"
                };
            }

            var button = Button.Create(
                request.Name,
                request.Code,
                request.MenuId,
                request.PermissionCode,
                request.Icon,
                request.SortOrder,
                request.Description
            );

            await _buttonRepository.AddAsync(button);

            return new ApiRequestResult
            {
                Success = true,
                Message = "按钮创建成功",
                Data = new { Id = button.Id }
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"创建按钮失败：{ex.Message}"
            };
        }
    }

    /// <summary>
    /// 更新按钮
    /// </summary>
    public async Task<ApiRequestResult> UpdateButtonAsync(UpdateButtonRequest request)
    {
        try
        {
            var button = await _buttonRepository.FindAsync(request.Id);
            if (button is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "按钮不存在"
                };
            }

            // 如果修改了编码，检查新编码是否已存在
            if (!string.IsNullOrEmpty(request.Code) && request.Code != button.Code)
            {
                var existingButton = await _buttonRepository.GetFirstAsync(b => b.Code == request.Code);
                if (existingButton is not null)
                {
                    return new ApiRequestResult
                    {
                        Success = false,
                        Message = "按钮编码已存在"
                    };
                }
            }

            // 如果修改了菜单ID，检查菜单是否存在
            if (request.MenuId.HasValue && request.MenuId.Value != button.MenuId)
            {
                var menu = await _menuRepository.FindAsync(request.MenuId.Value);
                if (menu is null)
                {
                    return new ApiRequestResult
                    {
                        Success = false,
                        Message = "所属菜单不存在"
                    };
                }
            }

            button.Update(
                request.Name,
                request.Code,
                request.MenuId,
                request.PermissionCode,
                request.Icon,
                request.SortOrder,
                request.Description,
                request.Status
            );

            _buttonRepository.Update(button);
            await _buttonRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "按钮更新成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"更新按钮失败：{ex.Message}"
            };
        }
    }

    /// <summary>
    /// 删除按钮
    /// </summary>
    public async Task<ApiRequestResult> DeleteButtonAsync(Guid id)
    {
        try
        {
            var button = await _buttonRepository.FindAsync(id);
            if (button is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "按钮不存在"
                };
            }

            _buttonRepository.Remove(button);
            await _buttonRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "按钮删除成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"删除按钮失败：{ex.Message}"
            };
        }
    }

    /// <summary>
    /// 启用按钮
    /// </summary>
    public async Task<ApiRequestResult> EnableButtonAsync(Guid id)
    {
        try
        {
            var button = await _buttonRepository.FindAsync(id);
            if (button is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "按钮不存在"
                };
            }

            button.Enable();
            _buttonRepository.Update(button);
            await _buttonRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "按钮已启用"
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"启用按钮失败：{ex.Message}"
            };
        }
    }

    /// <summary>
    /// 禁用按钮
    /// </summary>
    public async Task<ApiRequestResult> DisableButtonAsync(Guid id)
    {
        try
        {
            var button = await _buttonRepository.FindAsync(id);
            if (button is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "按钮不存在"
                };
            }

            button.Disable();
            _buttonRepository.Update(button);
            await _buttonRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "按钮已禁用"
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"禁用按钮失败：{ex.Message}"
            };
        }
    }
}