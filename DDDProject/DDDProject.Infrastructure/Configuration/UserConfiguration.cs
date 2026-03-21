using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDProject.Domain.Entities;

namespace DDDProject.Infrastructure.Configuration;

/// <summary>
/// 用户实体配置
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // 表名
        builder.ToTable("Users");

        // 主键
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        // 用户名 - 必填，唯一
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        // 邮箱 - 必填，唯一
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        // 手机号码
        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique(false);

        // 密码哈希 - 必填
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        // 真实姓名
        builder.Property(u => u.RealName)
            .HasMaxLength(50);

        // 头像
        builder.Property(u => u.Avatar)
            .HasMaxLength(500);

        // 用户状态 - 必填，默认为1（启用）
        builder.Property(u => u.Status)
            .IsRequired()
            .HasDefaultValue(1);

        // 最后登录时间
        builder.Property(u => u.LastLoginTime);

        // 最后登录IP
        builder.Property(u => u.LastLoginIp)
            .HasMaxLength(50);

        // 备注
        builder.Property(u => u.Remark)
            .HasMaxLength(500);

        // 创建时间
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETDATE()");  // GETDATE() 返回服务器本地时间

        // 更新时间
        builder.Property(u => u.UpdatedAt);
    }
}
