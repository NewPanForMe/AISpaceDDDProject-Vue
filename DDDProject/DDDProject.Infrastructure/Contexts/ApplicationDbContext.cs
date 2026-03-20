using Microsoft.EntityFrameworkCore;
using DDDProject.Domain.Repositories;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Models;
using DDDProject.Infrastructure.Repositories;
using DDDProject.Infrastructure.Configuration;

namespace DDDProject.Infrastructure.Contexts;

/// <summary>
/// 数据库上下文
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    /// <summary>
    /// 仓储集合
    /// </summary>
    public DbSet<ExampleAggregate> Examples => Set<ExampleAggregate>();

    /// <summary>
    /// 用户集合
    /// </summary>
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置示例聚合
        modelBuilder.Entity<ExampleAggregate>(entity =>
        {
            entity.ToTable("Examples");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        // 配置用户实体
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
