# DDDNewProject - DDD 分层架构全栈项目

本项目采用 DDD（Domain-Driven Design）分层架构设计，包含后端 API（DDDProject）和前端 Vue（DDDVue）两个子项目。

## 项目结构

```
DDDNewProject/
├── DDDProject/               # DDD 后端项目（.NET Core WebAPI）
│   ├── DDDProject.Domain/          # 领域层（Domain Layer）
│   ├── DDDProject.Application/     # 应用层（Application Layer）
│   ├── DDDProject.Infrastructure/  # 基础设施层（Infrastructure Layer）
│   ├── DDDProject.API/             # API 层（Presentation Layer）
│   ├── DDDProject.sln              # 解决方案文件
│   └── README.md                   # 后端项目说明
└── DDDVue/                   # Vue 前端项目
    ├── src/
    │   ├── api/              # API 接口统一管理
    │   ├── assets/           # 静态资源
    │   ├── components/       # 公共组件
    │   ├── router/           # 路由配置
    │   ├── utils/            # 工具函数
    │   ├── views/            # 页面组件
    │   ├── App.vue           # 根组件
    │   └── main.ts          # 入口文件
    └── README.md             # 前端项目说明
```

## 技术栈

### 后端（DDDProject）
- .NET 10.0
- ASP.NET Core WebAPI
- Entity Framework Core 10.0.5
- SQL Server
- DDD 分层架构

### 前端（DDDVue）
- Vue 3
- TypeScript
- Vite
- Vue Router
- Element Plus
- Axios

## 数据库配置

**数据库连接字符串**（已配置）：
```
Data Source=.;Initial Catalog=AiSpace;Persist Security Info=True;User ID=sa;Password=123456;Encrypt=False
```

**数据库名称**: AiSpace

## DDD 分层架构说明

### 1. Domain Layer（领域层）
- **Entities**: 实体对象，具有唯一标识
  - `Entity<TId>`: 实体基类
  - `AggregateRoot<TId>`: 聚合根基类
- **ValueObjects**: 值对象，无唯一标识，不可变
- **Repositories**: 仓储接口，定义数据访问契约
- **Events**: 领域事件，用于跨领域模块通信
- **Models**: 领域模型实现

### 2. Application Layer（应用层）
- **DTOs**: 数据传输对象，用于层间数据传递
- **Interfaces**: 应用服务接口定义
- **Services**: 应用服务实现，协调领域对象完成业务操作

### 3. Infrastructure Layer（基础设施层）
- **Contexts**: 数据库上下文（ApplicationDbContext）
- **Repositories**: 仓储的具体实现（Repository）
- **Configuration**: 配置类
- **Migrations**: 数据库迁移文件

### 4. API Layer（表示层）
- **Controllers**: API 控制器，处理 HTTP 请求
- **Program**: 应用程序入口，配置服务

## 构建和运行

### 后端项目（DDDProject）

```bash
# 进入项目目录
cd DDDNewProject\DDDProject

# 构建解决方案
dotnet build

# 运行 API 项目
dotnet run --project DDDProject.API

# 创建数据库迁移
dotnet ef migrations add MigrationName --project DDDProject.Infrastructure --startup-project DDDProject.API --output-dir Contexts/Migrations

# 更新数据库
dotnet ef database update --project DDDProject.Infrastructure --startup-project DDDProject.API

# 移除最新迁移
dotnet ef migrations remove --project DDDProject.Infrastructure --startup-project DDDProject.API
```

### 前端项目（DDDVue）

```bash
# 进入项目目录
cd DDDNewProject\DDDVue

# 安装依赖
npm install

# 开发模式运行
npm run dev

# 构建生产版本
npm run build

# 预览构建结果
npm run preview
```

## 使用说明

### 后端开发

1. **添加新实体**：在 `DDDProject.Domain/Models` 中创建实体类
2. **添加仓储接口**：在 `DDDProject.Domain/Repositories` 中定义接口
3. **添加仓储实现**：在 `DDDProject.Infrastructure/Repositories` 中实现
4. **添加应用服务**：在 `DDDProject.Application/Services` 中实现业务逻辑
5. **添加 API 控制器**：在 `DDDProject.API/Controllers` 中添加端点
6. **创建数据库迁移**：当模型发生变化时，使用 EF Core 迁移命令

### 前端开发

1. **添加新页面**：在 `DDDVue/src/views` 中创建 Vue 组件
2. **添加路由**：在 `DDDVue/src/router` 中配置路由
3. **添加 API 接口**：在 `DDDVue/src/api` 中统一管理 API
4. **添加公共组件**：在 `DDDVue/src/components` 中创建组件

## API Search 功能说明

### 功能概述
API Search 功能用于自动扫描和检索项目中所有标记了 `[ApiSearch]` 注解的 API 方法，并生成 JSON 格式的 API 列表。

### 组件说明

#### 1. ApiSearchAttribute (注解)
**位置**: `DDDProject.Application\Common\ApiSearchAttribute.cs`

用于标记需要搜索的 API 方法。

**属性**:
- `Name`: API 名称
- `Description`: API 描述
- `Category`: API 分类

**使用示例**:
```csharp
[HttpGet]
[ApiSearch(Name = "获取示例列表", Description = "返回示例数据列表", Category = "示例")]
public IActionResult Get()
{
    return Ok(new { message = "示例控制器" });
}
```

#### 2. ApiSearchController (控制器)
**位置**: `DDDProject.API\Controllers\ApiSearchController.cs`

提供 API 搜索功能的控制器。

**可用接口**:

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/ApiSearch/Search` | GET | 获取所有标记了 ApiSearch 注解的 API |
| `/api/ApiSearch/SearchByCategory?category=xxx` | GET | 根据分类获取 API 列表 |
| `/api/ApiSearch/SearchByKeyWord?keyword=xxx` | GET | 根据关键词搜索 API |

#### 3. ApiSearchService (服务)
**位置**: `DDDProject.Application\Services\ApiSearchService.cs`

用于扫描控制器并生成 API 列表的服务。

**主要方法**:
- `GetApiSearchList()`: 获取所有 API
- `GetApiSearchListByCategory(string category)`: 按分类筛选
- `GetApiSearchListByKeyWord(string keyword)`: 按关键词搜索

### 使用方法

#### 1. 在控制器方法上添加注解
```csharp
using DDDProject.Application.Common;

[ApiController]
[Route("api/[controller]/[action]")]
public class YourController : BaseApiController
{
    [HttpGet]
    [ApiSearch(Name = "获取数据", Description = "获取用户数据列表", Category = "用户")]
    public IActionResult GetData()
    {
        // 业务逻辑
    }
}
```

#### 2. 调用搜索接口
```bash
# 获取所有 API
curl http://localhost:5000/api/ApiSearch/Search

# 按分类搜索
curl http://localhost:5000/api/ApiSearch/SearchByCategory?category=用户

# 按关键词搜索
curl http://localhost:5000/api/ApiSearch/SearchByKeyWord?keyword=用户
```

#### 3. 前端调用示例
```javascript
import api from '@/api'

// 获取所有 API
api.ApiSearch.Search()

// 按分类搜索
api.ApiSearch.SearchByCategory({ category: '用户' })

// 按关键词搜索
api.ApiSearch.SearchByKeyWord({ keyword: '用户' })
```

### Service 层规范

在本项目中，新建 Service 时应遵循以下规范：

1. **新建接口**: 在 `DDDProject.Application\Interfaces` 目录下创建接口文件
2. **新建实现类**: 在 `DDDProject.Application\Services` 目录下创建实现类
3. **继承关系**:
   - Service 实现类继承自对应的接口
   - 接口继承自 `IApplicationService`
   - `IApplicationService` 是所有应用服务的基接口

### 完整示例：添加新的 API 功能

#### 1. 创建接口和 Service
```csharp
// IUserDataService.cs (Interfaces 目录)
public interface IUserDataService : IApplicationService
{
    UserData GetUserData(Guid userId);
}

// UserDataService.cs (Services 目录)
public class UserDataService : IUserDataService
{
    public UserData GetUserData(Guid userId)
    {
        // 实现逻辑
    }
}
```

#### 2. 在控制器中使用
```csharp
public class UserDataController : BaseApiController
{
    private readonly IUserDataService _userDataService;

    public UserDataController(IUserDataService userDataService)
    {
        _userDataService = userDataService;
    }

    [HttpGet]
    [ApiSearch(Name = "获取用户数据", Category = "用户")]
    public IActionResult GetUserData(Guid userId)
    {
        return Ok(_userDataService.GetUserData(userId));
    }
}
```

## 编码规范

本项目所有文件均使用 **UTF-8 编码** 保存，包括但不限于：
- Vue 组件文件（`.vue`）
- TypeScript/JavaScript 文件（`.ts`, `.tsx`, `.js`, `.jsx`）
- C# 文件（`.cs`）
- 配置文件（`.json`, `.js`, `.ts`）
- 样式文件（`.css`, `.scss`, `.less`）
- 文档文件（`.md`）

请在您的编辑器中设置 UTF-8 编码以确保正常显示中文内容。

## 注意事项

- 遵循 DDD 设计原则
- 领域层不应依赖其他层
- 应用层协调领域层完成业务操作
- 基础设施层实现抽象接口
- API 层仅处理 HTTP 相关注
- 数据库连接字符串已配置，如需修改请更新 `DDDProject/API/appsettings.json`
- 前端 API 基础地址配置在 `DDDVue/src/utils/http.js`

## 已完成的功能

### 后端（DDDProject）
✅ DDD 四层架构搭建完成
✅ 数据库连接配置完成
✅ Entity Framework Core 集成完成
✅ 数据库迁移支持完成
✅ API Search 功能实现
✅ 示例代码已提供

### 前端（DDDVue）
✅ Vue 3 + TypeScript + Vite 项目搭建完成
✅ Vue Router 路由配置完成
✅ Element Plus UI 框架集成
✅ Axios HTTP 请求封装
✅ API 接口统一管理
✅ UTF-8 编码规范设置
