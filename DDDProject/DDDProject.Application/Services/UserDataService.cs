using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Helpers;
using DDDProject.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace DDDProject.Application.Services;

/// <summary>
/// 用户数据应用服务实现
/// </summary>
public class UserDataService : IUserDataService
{
    private readonly IRepository<User> _userRepository;

    public UserDataService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    public async Task<ApiRequestResult> GetUsersAsync(PagedRequest request)
    {
        try
        {
            var skipCount = (request.PageNumber - 1) * request.PageSize;
            var total = await _userRepository.CountAsync(m => true);
            var users = await _userRepository.GetListAsync(
                m => true,
                q => q.OrderBy(m => m.CreatedAt),
                skipCount,
                request.PageSize
            );

            var dtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                RealName = u.RealName,
                Avatar = u.Avatar,
                Status = u.Status,
                LastLoginTime = u.LastLoginTime,
                LastLoginIp = u.LastLoginIp,
                Remark = u.Remark,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToList();

            var pagedResult = new PagedResult<UserDto>
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
                Message = $"获取用户列表失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 获取用户详情
    /// </summary>
    public async Task<ApiRequestResult> GetUserByIdAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.FindAsync(id);
            if (user is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户不存在",
                    Data = null
                };
            }

            var dto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RealName = user.RealName,
                Avatar = user.Avatar,
                Status = user.Status,
                LastLoginTime = user.LastLoginTime,
                LastLoginIp = user.LastLoginIp,
                Remark = user.Remark,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
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
                Message = $"获取用户详情失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    public async Task<ApiRequestResult> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            // 检查用户名是否存在
            var existingUser = await _userRepository.GetFirstAsync(u => u.UserName == request.UserName);
            if (existingUser is not null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户名已存在",
                    Data = null
                };
            }

            // 检查邮箱是否存在
            existingUser = await _userRepository.GetFirstAsync(u => u.Email == request.Email);
            if (existingUser is not null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "邮箱已存在",
                    Data = null
                };
            }

            // 解密 AES 加密的密码
            var decryptedPassword = PasswordHelper.DecryptPassword(request.Password);
            
            // 计算密码哈希
            var passwordHash = PasswordHelper.ComputeHash(decryptedPassword);

            var user = User.Create(
                request.UserName,
                request.Email,
                passwordHash,
                request.RealName
            );

            user.PhoneNumber = request.PhoneNumber;
            user.Avatar = request.Avatar;
            user.Remark = request.Remark;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "创建成功",
                Data = user.Id
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"创建用户失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    public async Task<ApiRequestResult> UpdateUserAsync(UpdateUserRequest request)
    {
        try
        {
            var user = await _userRepository.FindAsync(request.Id);
            if (user is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户不存在",
                    Data = null
                };
            }

            user.Update(
                request.Email,
                request.PhoneNumber,
                request.RealName,
                request.Avatar,
                request.Remark
            );

            if (request.Status.HasValue)
            {
                if (request.Status.Value == 1)
                    user.Enable();
                else
                    user.Disable();
            }

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "更新成功",
                Data = user.Id
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"更新用户失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    public async Task<ApiRequestResult> DeleteUserAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.FindAsync(id);
            if (user is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户不存在",
                    Data = null
                };
            }

            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();

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
                Message = $"删除用户失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 启用用户
    /// </summary>
    public async Task<ApiRequestResult> EnableUserAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.FindAsync(id);
            if (user is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户不存在",
                    Data = null
                };
            }

            user.Enable();
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

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
                Message = $"启用用户失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 禁用用户
    /// </summary>
    public async Task<ApiRequestResult> DisableUserAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.FindAsync(id);
            if (user is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户不存在",
                    Data = null
                };
            }

            user.Disable();
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

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
                Message = $"禁用用户失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    public async Task<ApiRequestResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var user = await _userRepository.FindAsync(request.Id);
            if (user is null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "用户不存在",
                    Data = null
                };
            }

            // 解密 AES 加密的密码
            var decryptedPassword = PasswordHelper.DecryptPassword(request.NewPassword);
            
            // 计算密码哈希
            var newPasswordHash = PasswordHelper.ComputeHash(decryptedPassword);
            
            user.UpdatePassword(newPasswordHash);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "密码重置成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"重置密码失败: {ex.Message}",
                Data = null
            };
        }
    }
}
