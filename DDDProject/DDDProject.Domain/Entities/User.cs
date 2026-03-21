namespace DDDProject.Domain.Entities;

/// <summary>
/// 用户实体
/// </summary>
public class User : AggregateRoot
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; private set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// 手机号码
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// 密码哈希
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? RealName { get; private set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string? Avatar { get; private set; }

    /// <summary>
    /// 用户状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; private set; } = 1;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; private set; }

    /// <summary>
    /// 最后登录IP
    /// </summary>
    public string? LastLoginIp { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected User() { }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="email">邮箱</param>
    /// <param name="passwordHash">密码哈希</param>
    /// <param name="realName">真实姓名</param>
    public static User Create(string userName, string email, string passwordHash, string? realName = null)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = email,
            PasswordHash = passwordHash,
            RealName = realName,
            CreatedAt = DateTime.Now  // 使用本地时间（中国标准时间）
        };

        return user;
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="phoneNumber">手机号码</param>
    /// <param name="realName">真实姓名</param>
    /// <param name="avatar">头像</param>
    /// <param name="remark">备注</param>
    public void Update(string? email = null, string? phoneNumber = null, string? realName = null, string? avatar = null, string? remark = null)
    {
        if (!string.IsNullOrEmpty(email))
            Email = email;

        if (!string.IsNullOrEmpty(phoneNumber))
            PhoneNumber = phoneNumber;

        if (!string.IsNullOrEmpty(realName))
            RealName = realName;

        if (!string.IsNullOrEmpty(avatar))
            Avatar = avatar;

        if (!string.IsNullOrEmpty(remark))
            Remark = remark;

        UpdatedAt = DateTime.Now; // 使用本地时间（中国标准时间）
    }

    /// <summary>
    /// 更新密码
    /// </summary>
    /// <param name="newPasswordHash">新密码哈希</param>
    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.Now; // 使用本地时间（中国标准时间）
    }

    /// <summary>
    /// 更新登录信息
    /// </summary>
    /// <param name="ip">登录IP</param>
    public void UpdateLoginInfo(string ip)
    {
        LastLoginTime = DateTime.Now; // 使用本地时间（中国标准时间）
        LastLoginIp = ip;
        UpdatedAt = DateTime.Now; // 使用本地时间（中国标准时间）
    }

    /// <summary>
    /// 启用用户
    /// </summary>
    public void Enable()
    {
        Status = 1;
        UpdatedAt = DateTime.Now; // 使用本地时间（中国标准时间）
    }

    /// <summary>
    /// 禁用用户
    /// </summary>
    public void Disable()
    {
        Status = 0;
        UpdatedAt = DateTime.Now; // 使用本地时间（中国标准时间）
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="passwordHash">待验证的密码哈希</param>
    /// <returns>是否匹配</returns>
    public bool VerifyPassword(string passwordHash)
    {
        return PasswordHash == passwordHash;
    }
}
