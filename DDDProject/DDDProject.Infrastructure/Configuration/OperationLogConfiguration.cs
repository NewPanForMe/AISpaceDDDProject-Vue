using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDProject.Domain.Entities;

namespace DDDProject.Infrastructure.Configuration;

/// <summary>
/// 操作日志实体配置
/// </summary>
public class OperationLogConfiguration : IEntityTypeConfiguration<OperationLog>
{
    public void Configure(EntityTypeBuilder<OperationLog> builder)
    {
        // 表名
        builder.ToTable("OperationLogs");

        // 主键
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        // 操作用户ID
        builder.Property(l => l.UserId);

        // 操作用户名
        builder.Property(l => l.UserName)
            .HasMaxLength(50);

        // 操作用户真实姓名
        builder.Property(l => l.RealName)
            .HasMaxLength(50);

        // 操作类型
        builder.Property(l => l.OperationType)
            .IsRequired()
            .HasMaxLength(20);

        // 操作模块
        builder.Property(l => l.Module)
            .IsRequired()
            .HasMaxLength(50);

        // 操作描述
        builder.Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(500);

        // 请求方法
        builder.Property(l => l.RequestMethod)
            .IsRequired()
            .HasMaxLength(10);

        // 请求路径
        builder.Property(l => l.RequestPath)
            .IsRequired()
            .HasMaxLength(200);

        // 请求参数（可能较长）
        builder.Property(l => l.RequestParams)
            .HasMaxLength(4000);

        // 响应结果（可能较长）
        builder.Property(l => l.ResponseResult)
            .HasMaxLength(4000);

        // 客户端IP地址
        builder.Property(l => l.IpAddress)
            .IsRequired()
            .HasMaxLength(50);

        // 执行状态
        builder.Property(l => l.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Success");

        // 错误信息
        builder.Property(l => l.ErrorMessage)
            .HasMaxLength(2000);

        // 执行耗时
        builder.Property(l => l.Duration);

        // 浏览器信息
        builder.Property(l => l.Browser)
            .HasMaxLength(200);

        // 操作系统信息
        builder.Property(l => l.OsInfo)
            .HasMaxLength(200);

        // 创建时间
        builder.Property(l => l.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        // 更新时间
        builder.Property(l => l.UpdatedAt);

        // 索引
        builder.HasIndex(l => l.UserId);
        builder.HasIndex(l => l.OperationType);
        builder.HasIndex(l => l.Module);
        builder.HasIndex(l => l.Status);
        builder.HasIndex(l => l.CreatedAt);
        builder.HasIndex(l => l.UserName);
    }
}