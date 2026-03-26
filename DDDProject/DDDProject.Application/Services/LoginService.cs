using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Helpers;
using DDDProject.Domain.Models;
using DDDProject.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DDDProject.Application.Services;

/// <summary>
/// 登录服务
/// </summary>
public class LoginService : ILoginService
{
    private readonly IRepository<User, Guid> _userRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<Setting> _settingRepository;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userRepository">用户仓储</param>
    /// <param name="userRoleRepository">用户角色关联仓储</param>
    /// <param name="roleRepository">角色仓储</param>
    /// <param name="settingRepository">设置仓储</param>
    public LoginService(
        IRepository<User, Guid> userRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<Role> roleRepository,
        IRepository<Setting> settingRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _settingRepository = settingRepository;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录结果</returns>
    public async Task<ApiRequestResult> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = "用户名或密码不能为空"
            };
        }

        // 解密密码
        var decryptedPassword = PasswordHelper.DecryptPassword(request.Password);
        
        // 计算密码哈希
        var passwordHash = PasswordHelper.ComputeHash(decryptedPassword);

        // 查询用户
        var users = await _userRepository.GetListAsync(u => u.UserName == request.UserName && u.PasswordHash == passwordHash);
        var user = await Task.Run(() => users.FirstOrDefault());

        if (user is null)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = "用户名或密码错误"
            };
        }

        // 检查用户状态
        if (user.Status == 0)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = "该账号已被禁用，请联系管理员"
            };
        }

        // 检查用户角色状态
        var userRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == user.Id);
        var userRoleList = userRoles.ToList();

        if (userRoleList.Count > 0)
        {
            var roleIds = userRoleList.Select(ur => ur.RoleId).ToList();
            var roles = await _roleRepository.GetListAsync(r => roleIds.Contains(r.Id));
            var roleList = roles.ToList();

            // 检查是否有禁用的角色
            var disabledRoles = roleList.Where(r => r.Status == 0 || r.Code== "DISABLED").ToList();
            if (disabledRoles.Count > 0)
            {
                var disabledRoleNames = string.Join("、", disabledRoles.Select(r => r.Name));
                return new ApiRequestResult
                {
                    Success = false,
                    Message = $"您的角色 [{disabledRoleNames}] 已被禁用，请联系管理员"
                };
            }
        }

        // 创建 Token
        var token = await CreateJwtTokenAsync(user);

        // 更新登录信息
        user.UpdateLoginInfo(GetClientIpAddress());
        
        // 保存到数据库
        await _userRepository.UpdateAsync(user);

        return new ApiRequestResult
        {
            Success = true,
            Message = "登录成功",
            Data = new LoginResponse
            {
                Token = token,
                UserId = user.Id,
                UserName = user.UserName,
                RealName = user.RealName
            }
        };
    }

    /// <summary>
    /// 获取客户端 IP 地址
    /// </summary>
    /// <returns>IP 地址字符串</returns>
    private string GetClientIpAddress()
    {
        // 这里需要通过依赖注入获取 HttpContext
        // 简化处理，返回空字符串
        return string.Empty;
    }

    /// <summary>
    /// 从数据库获取 JWT 配置
    /// </summary>
    /// <returns>JWT 配置对象</returns>
    private async Task<JwtSettings> GetJwtSettingsAsync()
    {
        var settings = await _settingRepository.GetListAsync(s =>
            s.Key == "JwtSettings_Issuer" ||
            s.Key == "JwtSettings_Audience" ||
            s.Key == "JwtSettings_Key" ||
            s.Key == "JwtSettings_ExpireMinutes");

        var settingList = settings.ToList();

        var jwtSettings = new JwtSettings
        {
            Issuer = settingList.FirstOrDefault(s => s.Key == "JwtSettings_Issuer")?.Value ?? "DDDProject",
            Audience = settingList.FirstOrDefault(s => s.Key == "JwtSettings_Audience")?.Value ?? "DDDProject",
            Key = settingList.FirstOrDefault(s => s.Key == "JwtSettings_Key")?.Value ?? "1fe277c55303f1c97e0d5861959039077",
            ExpireMinutes = int.TryParse(settingList.FirstOrDefault(s => s.Key == "JwtSettings_ExpireMinutes")?.Value, out var expireMinutes) ? expireMinutes : 720
        };

        return jwtSettings;
    }

    /// <summary>
    /// 创建 JWT Token
    /// </summary>
    /// <param name="user">用户实体</param>
    /// <returns>JWT Token 字符串</returns>
    private async Task<string> CreateJwtTokenAsync(User user)
    {
        // 创建 Claim 列表
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        // 如果有真实姓名，添加到 Claim
        if (!string.IsNullOrEmpty(user.RealName))
        {
            claims.Add(new Claim(ClaimTypes.GivenName, user.RealName));
        }

        // 获取用户角色并添加到 Claim
        var userRoles = await _userRoleRepository.GetListAsync(ur => ur.UserId == user.Id);
        var userRoleList = userRoles.ToList();

        if (userRoleList.Count > 0)
        {
            var roleIds = userRoleList.Select(ur => ur.RoleId).ToList();
            var roles = await _roleRepository.GetListAsync(r => roleIds.Contains(r.Id) && r.Status == 1);
            var roleList = roles.ToList();

            // 添加角色信息到 claims
            foreach (var role in roleList)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Code));
            }

            // 将角色编码列表添加到 unique_name（逗号分隔）
            var roleCodes = string.Join(",", roleList.Select(r => r.Code));
            claims.Add(new Claim("roles", roleCodes));
        }

        // 从数据库读取 JWT 设置
        var jwtSettings = await GetJwtSettingsAsync();

        // 创建签名密钥
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 创建 Token
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpireMinutes),
            signingCredentials: credentials
        );

        // 返回 Token 字符串
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
