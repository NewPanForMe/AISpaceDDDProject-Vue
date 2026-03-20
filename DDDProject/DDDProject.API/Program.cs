
using DDDProject.Application.Interfaces;
using DDDProject.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 添加 CORS 策略
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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


var app = builder.Build();

// 配置 HTTP 请求管道
// 开发环境禁用 HTTPS 重定向，避免 OPTIONS 预检请求 307 重定向错误
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DDDProject API V1");
    });
}

//app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthorization();

// 映射控制器
app.MapControllers();
app.MapGet("/", () => "Hello World");
app.Run();
