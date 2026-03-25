using DDDProject.Infrastructure.Configuration;
using DDDProject.Infrastructure.Contexts;
using DDDProject.Domain.Repositories;
using DDDProject.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DDDProject.Infrastructure.Services;

namespace DDDProject.Infrastructure;

/// <summary>
/// 应用服务注册扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加数据库上下文
    /// </summary>
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    /// <summary>
    /// 添加仓储
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(Infrastructure.Repositories.Repository<,>));
        services.AddScoped(typeof(IRepository<>), typeof(Infrastructure.Repositories.Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();

        // 注册时间服务
        services.AddScoped<ITimeService, ChinaStandardTimeService>();

        return services;
    }
}