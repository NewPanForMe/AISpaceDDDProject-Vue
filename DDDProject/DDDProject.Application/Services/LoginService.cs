using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Helpers;
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

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userRepository">用户仓储</param>
    public LoginService(IRepository<User, Guid> userRepository)
    {
        _userRepository = userRepository;
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

        if (user == null)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = "用户名或密码错误"
            };
        }

        // 创建 Token
        var token = await CreateJwtTokenAsync(user);

        // 更新登录信息
        user.UpdateLoginInfo(GetClientIpAddress());

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
            new Claim(ClaimTypes.Name, user.UserName)
        };

        // 如果有真实姓名，添加到 Claim
        if (!string.IsNullOrEmpty(user.RealName))
        {
            claims.Add(new Claim(ClaimTypes.GivenName, user.RealName));
        }

        // 从配置读取 JWT 设置（这里使用硬编码，实际应从配置文件读取）
        var jwtSettings = new
        {
            Issuer = "DDDProject",
            Audience = "DDDProject",
            Key = "YourSuperSecretKey12345678901234567890" // 32字节以上
        };

        // 创建签名密钥
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 创建 Token
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: credentials
        );

        // 返回 Token 字符串
        return await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(token));
    }
}
