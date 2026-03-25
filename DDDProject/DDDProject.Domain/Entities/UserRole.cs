namespace DDDProject.Domain.Entities;

/// <summary>
/// 用户角色关联实体
/// </summary>
public class UserRole : Entity
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected UserRole() { }

    /// <summary>
    /// 创建用户角色关联
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleId">角色ID</param>
    public static UserRole Create(Guid userId, Guid roleId)
    {
        return new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            CreatedAt = DateTime.Now
        };
    }
}