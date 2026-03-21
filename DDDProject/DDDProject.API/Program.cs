
using DDDProject.API;

var builder = WebApplication.CreateBuilder(args);
var startup = new Startup(builder.Configuration);

// 配置服务
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// 配置应用管道
startup.Configure(app, app.Environment, app.Services.GetRequiredService<ILogger<Program>>());

app.Run();
