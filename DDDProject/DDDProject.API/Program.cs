
using DDDProject.Application.Interfaces;
using DDDProject.Application.Services;
using DDDProject.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 添加数据库上下文
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddApplicationDbContext(connectionString);
builder.Services.AddRepositories();

// 自动注册所有继承自 IApplicationService 的接口和实现类（Scoped）
builder.Services.Scan(scan => scan
    .FromAssemblyOf<IApplicationService>()
    .AddClasses(classes => classes.AssignableTo<IApplicationService>())
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

// 注册 ApiSearchService
builder.Services.AddScoped<ApiSearchService>();

var app = builder.Build();

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DDDProject API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

// 映射控制器
app.MapControllers();
app.MapGet("/", () => "HelloWorld");
app.Run();
