using DDDProject.Domain.Models;
using DDDProject.API.Extensions;
using DDDProject.API.Middlewares;
using DDDProject.Application.Interfaces;
using DDDProject.Infrastructure;
using DDDProject.Infrastructure.Contexts;
using DDDProject.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.OpenApi;

namespace DDDProject.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // 添加 CORS 策略
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // 添加服务到容器
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // 配置
            services.AddOpenApi(options =>
             {
                 options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
             });

            // 添加配置绑定
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));

            // 添加数据库上下文
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddApplicationDbContext(connectionString);
            services.AddRepositories();

            // 自动注册所有继承自 IApplicationService 的接口和实现类（Scoped）
            services.Scan(scan => scan
                .FromAssemblyOf<IApplicationService>()
                .AddClasses(classes => classes.AssignableTo<IApplicationService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );

            // 添加JWT认证
            var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddAuthentication(authen =>
            {
                authen.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authen.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false; // 在开发环境中可以设为false
                bearer.SaveToken = true;

                // 禁用 claims 映射，保留原始 JWT claim 名称
                bearer.MapInboundClaims = false;

                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // 设置为零以严格限制令牌生命周期
                };
            });

            services.AddHttpContextAccessor();
            services.AddScoped<CurrentUser>();
            services.AddScoped<ICurrentUserContext>(sp => sp.GetRequiredService<CurrentUser>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Program> logger)
        {

            // 应用数据库迁移并初始化数据
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    // 应用任何挂起的迁移
                    // 注意：如果出现待处理的模型更改错误，请先添加新迁移
                    context.Database.Migrate();

                    // 初始化种子数据（按依赖顺序执行）
                    // 1. 基础数据
                    context.SeedRoles();
                    context.SeedUsers();
                    context.SeedMenus();
                    context.SeedPermissions();
                    context.SeedSettings();

                    // 2. 关联数据
                    context.SeedUserRoles();
                    context.SeedMenuRoles();
                    context.SeedRolePermissions();

                    logger.LogInformation("数据库初始化完成，所有种子数据已同步。");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding the database.");
                    // 不抛出异常，而是继续启动
                    logger.LogWarning("数据库迁移或初始化失败，但应用程序将继续启动。");
                }
            }

            // 配置 HTTP 请求管道
            // 开发环境禁用 HTTPS 重定向，避免 OPTIONS 预检请求 307 重定向错误
            //app.UseHttpsRedirection();

            app.UseCors("AllowAll");
            app.UseRouting(); // 添加路由中间件

            // 必须在 UseRouting 之后，但在 UseEndpoints 之前使用认证中间件
            app.UseAuthentication();
            app.UseAuthorization();

            // 添加自定义权限检查中间件
            // 必须在 UseRouting 之后（以便获取 endpoint 信息），在 UseAuthorization 之后（以便认证完成）
            app.UseMiddleware<PermissionCheckMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", () => "Hello World");
                endpoints.MapScalarApiReference(options =>
                {
                    options.Layout = ScalarLayout.Modern;
                    options.WithTitle("My API Documentation");
                    options.Authentication = new ScalarAuthenticationOptions()
                    {
                        PreferredSecuritySchemes = new List<string>() { "Bearer" }
                    };
                });
                endpoints.MapOpenApi();

            });
        }

        internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
        {
            public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
            {
                var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
                if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
                {
                    // Add the security scheme at the document level
                    var requirements = new Dictionary<string, IOpenApiSecurityScheme>
                    {
                        ["Bearer"] = new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer", // "bearer" refers to the header name here
                            In = ParameterLocation.Header,
                            BearerFormat = "Json Web Token"
                        }
                    };
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes = requirements;


                }
            }
        }
    }

}