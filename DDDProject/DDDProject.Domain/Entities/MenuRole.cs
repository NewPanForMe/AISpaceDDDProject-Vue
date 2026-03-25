namespace DDDProject.Domain.Entities;

/// <summary>
/// 菜单角色关联实体
/// </summary>
public class MenuRole : Entity
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid MenuId { get; private set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected MenuRole() { }

    /// <summary>
    /// 创建菜单角色关联
    /// </summary>
    /// <param name="menuId">菜单ID</param>
    /// <param name="roleId">角色ID</param>
    public static MenuRole Create(Guid menuId, Guid roleId)
    {
        return new MenuRole
        {
            Id = Guid.NewGuid(),
            MenuId = menuId,
            RoleId = roleId,
            CreatedAt = DateTime.Now
        };
    }
}