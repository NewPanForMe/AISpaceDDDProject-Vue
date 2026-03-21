# DDDNewProject - DDD 分层架构全栈项目

本项目采用 DDD（Domain-Driven Design）分层架构设计，包含后端 API（DDDProject）和前端 Vue（DDDVue）两个子项目。

## Git 仓库说明

本项目使用 Git 进行版本控制，远程仓库地址：`https://github.com/.../AISpaceDDDProject-Vue.git`

### 当前分支
- **master**: 主分支，用于生产环境代码

### Git 配置
- 编码格式：UTF-8
- 换行符：Windows (CRLF)

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
- **Repositories**: 仓储的具体实现（Repository、RepositorySimple）
- **Configuration**: 配置类
- **Migrations**: 数据库迁移文件

### 4. API Layer（表示层）
- **Controllers**: API 控制器，处理 HTTP 请求
- **Program**: 应用程序入口，配置服务

## 构建和运行

### Git 基础操作

```bash
# 查看仓库状态
git status

# 查看提交历史
git log --oneline

# 查看分支
git branch -a

# 添加所有修改到暂存区
git add .

# 提交修改
git commit -m "提交信息"

# 推送到远程仓库
git push origin master

# 从远程仓库拉取最新代码
git pull origin master

# 创建并切换到新分支
git checkout -b feature/branch-name

# 切换分支
git checkout master

# 删除本地分支
git branch -d branch-name

# 查看文件变更
git diff
git diff --staged
```

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

## 异步编程规范

### 数据库交互必须使用异步方法

在本项目中，所有与数据库交互的操作**必须使用异步方法**，以避免阻塞线程和提高性能。

### EF Core 异步方法对照表

| 同步方法 | 异步方法 | 说明 |
|---------|---------|------|
| `ToList()` | `ToListAsync()` | 获取列表 |
| `FirstOrDefault()` | `FirstOrDefaultAsync()` | 获取第一个元素 |
| `SingleOrDefault()` | `SingleOrDefaultAsync()` | 获取唯一元素 |
| `Find()` | `FindAsync()` | 根据主键查找 |
| `Add()` | `AddAsync()` | 添加实体 |
| `RangeAdd()` | `AddRangeAsync()` | 批量添加 |
| `Update()` | `UpdateAsync()` | 更新实体 |
| `Remove()` | `RemoveAsync()` | 删除实体 |
| `SaveChanges()` | `SaveChangesAsync()` | 保存更改 |

### Service 层规范

在本项目中，新建 Service 时应遵循以下规范：

1. **新建接口**: 在 `DDDProject.Application\Interfaces` 目录下创建接口文件
2. **新建实现类**: 在 `DDDProject.Application\Services` 目录下创建实现类
3. **继承关系**:
   - Service 实现类继承自对应的接口
   - 接口继承自 `IApplicationService`
   - `IApplicationService` 是所有应用服务的基接口
4. **异步方法命名**: 所有异步方法名必须添加 `Async` 后缀
5. **数据库操作**: 所有数据库操作必须使用异步方法
6. **返回值处理**: Service方法返回ApiRequestResult时，应直接使用`new ApiRequestResult`创建实例，模仿LoginService中的风格：
   ```csharp
   // 正确方式：直接创建新实例
   return new ApiRequestResult
   {
       Success = true,
       Message = "操作成功",
       Data = result
   };
   
   return new ApiRequestResult
   {
       Success = false,
       Message = "操作失败信息",
       Data = null
   };
   ```
   例如，参照LoginService中方法的返回值设定：
   ```csharp
   // 登录服务示例 - 在LoginService.cs中
   public async Task<ApiRequestResult> LoginAsync(LoginRequest request)
   {
       try
       {
           var decryptedPassword = PasswordHelper.DecryptPassword(request.Password);
           var passwordHash = PasswordHelper.ComputeHash(decryptedPassword);
   
           var users = await _userRepository.GetListAsync(u => u.UserName == request.UserName && u.PasswordHash == passwordHash);
           var user = users.FirstOrDefault();
   
           if (user == null)
           {
               return new ApiRequestResult
               {
                   Success = false,
                   Message = "用户名或密码错误",
                   Data = null
               };
           }
   
           return new ApiRequestResult
           {
               Success = true,
               Message = "登录成功",
               Data = new { UserId = user.Id, UserName = user.UserName, NickName = user.NickName }
           };
       }
       catch (Exception ex)
       {
           return new ApiRequestResult
           {
               Success = false,
               Message = $"登录失败: {ex.Message}",
               Data = null
           };
       }
   }
   ```

### Service 层异步示例

```csharp
// IUserDataService.cs (Interfaces 目录)
public interface IUserDataService : IApplicationService
{
    Task<ApiRequestResult> GetUserByIdAsync(Guid userId);
    Task<ApiRequestResult> GetAllUsersAsync();
    Task<ApiRequestResult> CreateUserAsync(UserData user);
    Task<ApiRequestResult> UpdateUserAsync(UserData user);
    Task<ApiRequestResult> DeleteUserAsync(Guid userId);
    Task<ApiRequestResult> GetUserDataAsync(Guid userId);
}

// UserDataService.cs (Services 目录)
public class UserDataService : IUserDataService
{
    private readonly IRepository<UserData> _userRepository;

    public UserDataService(IRepository<UserData> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiRequestResult> GetUserByIdAsync(Guid userId)
    {
        // 使用 FindAsync 替代 Find
        var user = await _userRepository.FindAsync(userId);
        return new ApiRequestResult
        {
            Success = user != null,
            Message = user != null ? "获取用户成功" : "用户不存在",
            Data = user
        };
    }

    public async Task<ApiRequestResult> GetAllUsersAsync()
    {
        // 使用 ToListAsync 替代 ToList
        var users = await _userRepository.GetListAsync(u => true);
        return new ApiRequestResult
        {
            Success = true,
            Message = "获取用户列表成功",
            Data = users
        };
    }

    public async Task<ApiRequestResult> CreateUserAsync(UserData user)
    {
        // 使用 AddAsync 替代 Add
        await _userRepository.AddAsync(user);
        // 使用 SaveChangesAsync 替代 SaveChanges
        await _userRepository.SaveChangesAsync();
        return new ApiRequestResult
        {
            Success = true,
            Message = "创建用户成功",
            Data = user
        };
    }

    public async Task<ApiRequestResult> UpdateUserAsync(UserData user)
    {
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
        return new ApiRequestResult
        {
            Success = true,
            Message = "更新用户成功",
            Data = user
        };
    }

    public async Task<ApiRequestResult> DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.FindAsync(userId);
        if (user == null) 
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = "用户不存在",
                Data = null
            };
        }
        
        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();
        return new ApiRequestResult
        {
            Success = true,
            Message = "删除用户成功",
            Data = null
        };
    }

    public async Task<ApiRequestResult> GetUserDataAsync(Guid userId)
    {
        var user = await _userRepository.FindAsync(userId);
        return new ApiRequestResult
        {
            Success = user != null,
            Message = user != null ? "获取用户数据成功" : "用户不存在",
            Data = user
        };
    }
}
```

### Controller 层异步示例

```csharp
public class UserDataController : BaseApiController
{
    private readonly IUserDataService _userDataService;

    public UserDataController(IUserDataService userDataService)
    {
        _userDataService = userDataService;
    }

    [HttpGet("{id}")]
    [ActionName("GetUserByIdAsync")]
    public async Task<ApiRequestResult> GetUserByIdAsync(Guid id)
    {
        var result = await _userDataService.GetUserByIdAsync(id);
        return result; // Assuming service returns ApiRequestResult
    }

    [HttpGet]
    [ActionName("GetAllUsersAsync")]
    public async Task<ApiRequestResult> GetAllUsersAsync()
    {
        var result = await _userDataService.GetAllUsersAsync();
        return result; // Assuming service returns ApiRequestResult
    }

    [HttpPost]
    [ActionName("CreateUserAsync")]
    public async Task<ApiRequestResult> CreateUserAsync([FromBody] UserData user)
    {
        var result = await _userDataService.CreateUserAsync(user);
        return result; // Assuming service returns ApiRequestResult
    }

    [HttpPut]
    [ActionName("UpdateUserAsync")]
    public async Task<ApiRequestResult> UpdateUserAsync([FromBody] UserData user)
    {
        var result = await _userDataService.UpdateUserAsync(user);
        return result; // Assuming service returns ApiRequestResult
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var deleted = await _userDataService.DeleteUserAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
```

### 注意事项

1. **避免阻塞调用**: 不要在异步方法中使用 `.Result`、`.Wait()` 或 `.GetAwaiter().GetResult()`，这会导致死锁和性能问题
2. **保持异步链**: 从 Controller 到 Service 到 Repository，整个调用链都应该是异步的
3. **使用 CancellationToken**: 对于长时间运行的操作，应接受 `CancellationToken` 参数
4. **异常处理**: 异步方法的异常处理与同步方法相同，可以使用 try-catch 块

### Controller 方法注解规范

在本项目中，Controller 方法的路由注解必须遵循以下规范：

#### 1. 注解命名规范
- **必须使用 `[ActionName("方法名")]` 注解**，确保路由路径与方法名一致
- **HTTP 方法注解**（如 `[HttpPost]`、`[HttpGet]` 等）不需要指定路径
- **路由模板**在控制器级别使用 `[Route("api/[controller]/[action]")]`，其中 `[action]` 会自动使用方法名
- 控制器的注解无需   [ActionName("方法名")] 

#### 2. 注解格式
```csharp
[ApiController]
[Route("api/[controller]/[action]")]  // 控制器级别路由模板
public class YourController : BaseApiController
{
    [HttpPost]
    [ActionName("方法名")]  // 方法名必须与注解中的名称一致
    public async Task<IActionResult> 方法名([FromBody] RequestModel model)
    {
        // 实现逻辑
    }
}
```

#### 3. 正确示例
```csharp
// 方法名: LoginAsync
[HttpPost]
[ActionName("LoginAsync")]
public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
{
    // 实现逻辑
}

// 方法名: GetUserByIdAsync
[HttpGet]
[ActionName("GetUserByIdAsync")]
public async Task<IActionResult> GetUserByIdAsync(Guid id)
{
    // 实现逻辑
}

// 方法名: CreateUserAsync
[HttpPost]
[ActionName("CreateUserAsync")]
public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request)
{
    // 实现逻辑
}
```

#### 4. 错误示例
```csharp
// ❌ 错误：在 HttpPost 中指定路径
[HttpPost("LoginAsync")]
public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
{
    // 这会导致路由重复配置，且与 [ActionName] 不一致
}

// ❌ 错误：缺少 [ActionName] 注解
[HttpPost]
public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
{
    // 这会导致路由路径使用方法名 LoginAsync，但与 api/index.ts 配置不一致
}

// ❌ 错误：[ActionName] 与方法名不一致
[HttpPost]
[ActionName("Login")]
public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
{
    // 这会导致路由路径为 Login，但方法名为 LoginAsync
}
```

#### 5. 为什么需要 [ActionName]？
- **API 路径一致性**: 确保前端 `api/index.ts` 中定义的 API 路径与后端 `[ActionName]` 注解一致
- **路由清晰**: `[Route("api/[controller]/[action]")]` 中的 `[action]` 会自动使用 `[ActionName]` 的值
- **代码可读性**: 开发者可以直观地从 `[ActionName]` 知道对应的 API 路径
- **减少错误**: 避免因路由不一致导致的 404 或 405 错误
- **灵活性**: 可以独立控制路由路径和方法名，便于重构

#### 6. 完整示例
```csharp
using Microsoft.AspNetCore.Mvc;
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;

namespace DDDProject.API.Controllers;

/// <summary>
/// 登录控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class LoginController : BaseApiController
{
    private readonly ILoginService _loginService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="loginService">登录服务</param>
    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录结果</returns>
    [HttpPost()]
    [ActionName("LoginAsync")]
    [ApiSearch(Name = "用户登录", Description = "用户登录验证", Category = "认证")]
    public async Task<ApiRequestResult> LoginAsync([FromBody] LoginRequest request)
    {
        return await _loginService.LoginAsync(request);
    }
}

```

**前端 API 配置对应**:
```typescript
// api/index.ts
const api = {
  Login: {
    Login: 'api/Login/LoginAsync',  // 必须与 [ActionName("LoginAsync")] 一致
  },
};
```

**路由路径说明**:
- 控制器路由: `[Route("api/[controller]/[action]")]`
- 控制器名: `LoginController` → `[controller]` = `Login`
- 方法名: `LoginAsync` → `[action]` = `[ActionName]` 的值 = `LoginAsync`
- 最终路由: `api/Login/LoginAsync`



#### 控制器实现最佳实践 - 以LoginController为例

在本项目中，所有Controller应该参考LoginController的编写方式进行实现。特别是以下几点需要注意：

LoginController作为标准的范例，展示了正确的编写规范：

```csharp
using Microsoft.AspNetCore.Mvc;
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;

namespace DDDProject.API.Controllers;

/// <summary>
/// 登录控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class LoginController : BaseApiController
{
    private readonly ILoginService _loginService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="loginService">登录服务</param>
    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录结果</returns>
    [HttpPost()]
    [ActionName("LoginAsync")]
    [ApiSearch(Name = "用户登录", Description = "用户登录验证", Category = "认证")]
    public async Task<ApiRequestResult> LoginAsync([FromBody] LoginRequest request)
    {
        return await _loginService.LoginAsync(request);
    }
}

```

#### Service 层返回值约定

所有Service方法的返回类型应该是 `Task<ApiRequestResult>` 或相关泛型变体。这是为了让控制器能够方便地返回统一格式的结果：

```csharp
// 在服务中
public async Task<ApiRequestResult> LoginAsync(LoginRequest request)
{
    // 业务逻辑
    return new ApiRequestResult
    {
        Success = true,
        Message = "登录成功",
        Data = userInfo
    };
}
```

#### 编写要点

1. **继承BaseApiController**: 所有控制器必须继承 `BaseApiController`
2. **注入服务**: 通过构造函数注入所需的Service
3. **使用[ActionName]**: 按照注解规范使用ActionName
4. **异步方法**: 控制器方法使用 `async/await` 并返回 `Task<IActionResult>`
5. **引用ApiSearch**: 添加适当的`[ApiSearch]`标签用于接口发现
6. **统一返回**: 调用服务层方法后，直接返回 `Ok(result)` 给前端
7. **Service返回值**: 所有服务方法必须返回 `Task<ApiRequestResult>` 使得Controller可以直接转发结果

### Repository 层异步实现说明

#### 双泛型仓储实现（Repository<TEntity, TId>）
**文件位置**: `DDDProject.Infrastructure.Repositories.Repository.cs`
- 实现 `IRepository<TEntity, TId>` 接口（带实体和ID类型）
- 针对各种主键类型的通用实体仓储实现

#### 单泛型仓储实现（Repository<TEntity>）
**文件位置**: `DDDProject.Infrastructure.Repositories.RepositorySimple.cs`
- 实现 `IRepository<TEntity>` 接口（仅带实体类型，默认Guid主键）
- 专门针对使用Guid作为主键的实体的仓储实现
- 服务于Menu、User等需要IRepository<Menu>和IRepository<User>的场景

#### RepositorySimple示例实现

```csharp
public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity<Guid>
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
```

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
[ActionName("GetAsync")]
[ApiSearch(Name = "获取示例列表", Description = "返回示例数据列表", Category = "示例")]
public async Task<ApiRequestResult> GetAsync()
{
    return await yourService.GetAsync(); // Service method should return ApiRequestResult
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
	public async Task<ApiRequestResult> GetAsync()
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
    [ActionName("GetUserDataAsync")]
    [ApiSearch(Name = "获取用户数据", Category = "用户")]
    public async Task<ApiRequestResult> GetUserDataAsync(Guid userId)
    {
        return await _userDataService.GetUserDataAsync(userId);
    }
}
```

## 注意事项

- 遵循 DDD 设计原则
- 领域层不应依赖其他层
- 应用层协调领域层完成业务操作
- 基础设施层实现抽象接口
- API 层仅处理 HTTP 相关注
- 数据库连接字符串已配置，如需修改请更新 `DDDProject/API/appsettings.json`
- 前端 API 基础地址配置在 `DDDVue/src/utils/http.ts`

## 端口配置说明（禁止随意更改）

本项目有严格的端口配置规范，**禁止随意更改**，以确保前后端正常通信：

### 前端项目（DDDVue）
- **默认端口**: `5173`
- **访问地址**: http://localhost:5173/
- **配置文件**: `DDDVue/vite.config.ts`
- **修改方式**: 如需修改端口，请编辑 `DDDVue/vite.config.ts` 中的 `server.port` 配置

### 后端项目（DDDProject）
- **默认端口**: `5272`
- **访问地址**: http://localhost:5272/
- **Swagger 地址**: http://localhost:5272/swagger
- **配置文件**: `DDDProject/API/appsettings.json` 和 `DDDProject/API/Properties/launchSettings.json`
- **修改方式**: 如需修改端口，请编辑 `DDDProject/API/Properties/launchSettings.json` 中的 `applicationUrl` 配置

### 端口冲突处理
如果遇到端口被占用的情况：
1. **前端端口冲突**: 修改 `DDDVue/vite.config.ts` 中的端口配置
2. **后端端口冲突**: 修改 `DDDProject/API/Properties/launchSettings.json` 中的 `applicationUrl`
3. **修改后务必同步更新相关配置文件中的端口号**

### 端口配置文件位置
```
DDDProject/
└── DDDProject.API/
    └── Properties/
        └── launchSettings.json          # 后端端口配置

DDDVue/
├── vite.config.ts                      # 前端端口配置
└── src/
    └── utils/
        └── http.ts                     # API 基础地址配置
```

## Git 工作流规范

### 分支命名规范
- `master`: 主分支，稳定版本
- `feature/xxx`: 功能分支
- `bugfix/xxx`: 修复分支
- `hotfix/xxx`: 紧急修复分支
- `develop`: 开发分支（如需要）

### 提交信息规范
采用 conventional commits 格式：
```
<type>(<scope>): <subject>

<body>

<footer>
```

**type 类型**：
- `feat`: 新功能
- `fix`: 修复 bug
- `docs`: 文档变更
- `style`: 代码格式变更（不影响代码运行）
- `refactor`: 重构
- `perf`: 性能优化
- `test`: 测试相关
- `chore`: 构建过程或辅助工具变动

**示例**：
```
feat(user): 添加用户管理功能
fix(api): 修复 API 分页参数错误
docs(readme): 更新项目说明文档
```

### 工作流程
1. 从 `master` 拉取最新代码
2. 创建功能分支 `feature/xxx`
3. 在功能分支上开发并提交
4. 保持分支与 `master` 同步（定期 `git pull origin master`）
5. 开发完成后合并到 `master`
6. 推送到远程仓库

## 已完成的功能

### 后端（DDDProject）
✅ DDD 四层架构搭建完成
✅ 数据库连接配置完成
✅ Entity Framework Core 集成完成
✅ 数据库迁移支持完成
✅ API Search 功能实现
✅ 示例代码已提供
✅ Startup.cs 配置分离完成（提高可维护性）
✅ 仓储接口依赖注入修复完成（解决Repository<T>接口注册问题）

### 前端（DDDVue）
✅ Vue 3 + TypeScript + Vite 项目搭建完成
✅ Vue Router 路由配置完成
✅ Element Plus UI 框架集成
✅ Axios HTTP 请求封装
✅ API 接口统一管理
✅ UTF-8 编码规范设置

## 密码加密传输说明

### 概述
本项目采用 AES-256-CBC 算法对用户密码进行加密传输，确保密码在传输过程中不以明文形式暴露。

### 加密原理
- **加密算法**: AES-256-CBC
- **密钥**: `DDDProject2024SecretKey!` (前后端一致)
- **初始化向量 (IV)**: `DDDProject2024IV!` (前后端一致)
- **填充模式**: PKCS7

### 加密流程
1. **前端加密**: 用户输入密码后，使用 AES 算法加密，然后发送给后端
2. **后端解密**: 后端接收到加密密码后，使用相同的密钥和 IV 解密
3. **密码验证**: 解密后对明文密码进行 SHA256 哈希，与数据库中的哈希值比对

### 前端实现

#### 加密工具
**文件位置**: `DDDVue/src/utils/crypto.ts`

```typescript
import CryptoJS from 'crypto-js'

const AES_SECRET_KEY = 'DDDProject2024SecretKey!'
const AES_IV = 'DDDProject2024IV!'

// AES 加密
export function aesEncrypt(text: string): string {
  const encrypted = CryptoJS.AES.encrypt(text, CryptoJS.enc.Utf8.parse(AES_SECRET_KEY), {
    iv: CryptoJS.enc.Utf8.parse(AES_IV),
    mode: CryptoJS.mode.CBC,
    padding: CryptoJS.pad.Pkcs7
  })
  return encrypted.toString()
}

// AES 解密
export function aesDecrypt(encryptedText: string): string {
  const decrypted = CryptoJS.AES.decrypt(encryptedText, CryptoJS.enc.Utf8.parse(AES_SECRET_KEY), {
    iv: CryptoJS.enc.Utf8.parse(AES_IV),
    mode: CryptoJS.mode.CBC,
    padding: CryptoJS.pad.Pkcs7
  })
  return CryptoJS.enc.Utf8.stringify(decrypted)
}
```

#### 登录页面使用
**文件位置**: `DDDVue/src/views/Login.vue`

```typescript
import { aesEncrypt } from '../utils/crypto'

// 加密密码后发送
const response = await http.post<LoginResponse>(api.Login.Login, {
  userName: form.value.userName,
  password: aesEncrypt(form.value.password)  // 密码加密传输
})
```

#### 安装依赖
```bash
# 安装 crypto-js
npm install crypto-js

# 安装 TypeScript 类型定义
npm install --save-dev @types/crypto-js
```

### 后端实现

#### 密码帮助类
**文件位置**: `DDDProject.Domain/Helpers/PasswordHelper.cs`

```csharp
using System.Security.Cryptography;
using System.Text;

namespace DDDProject.Domain.Helpers;

public static class PasswordHelper
{
    // AES 加密密钥（必须与前端一致）
    private const string AES_SECRET_KEY = "DDDProject2024SecretKey!";
    private const string AES_IV = "DDDProject2024IV!";

    /// <summary>
    /// 计算密码哈希（SHA256）
    /// </summary>
    public static string ComputeHash(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    /// <summary>
    /// 解密 AES 加密的密码
    /// </summary>
    public static string DecryptPassword(string encryptedPassword)
    {
        if (string.IsNullOrEmpty(encryptedPassword))
            return encryptedPassword;

        try
        {
            var key = Encoding.UTF8.GetBytes(AES_SECRET_KEY);
            var iv = Encoding.UTF8.GetBytes(AES_IV);
            var encryptedBytes = Convert.FromBase64String(encryptedPassword);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch
        {
            // 解密失败，返回原值（兼容非加密传输）
            return encryptedPassword;
        }
    }
}
```

#### 登录服务使用
**文件位置**: `DDDProject.Application/Services/LoginService.cs`

```csharp
public async Task<ApiRequestResult> LoginAsync(LoginRequest request)
{
    // 解密密码
    var decryptedPassword = PasswordHelper.DecryptPassword(request.Password);
    
    // 计算密码哈希
    var passwordHash = PasswordHelper.ComputeHash(decryptedPassword);

    // 查询用户并验证
    var users = await _userRepository.GetListAsync(u => u.UserName == request.UserName && u.PasswordHash == passwordHash);
    // ... 其他逻辑
}
```

### 安全性说明

1. **传输安全**: 密码在前端加密后以密文形式传输，即使被截获也无法直接获取明文密码
2. **密钥管理**: 当前密钥硬编码在代码中，**生产环境应从配置文件或环境变量读取**
3. **双重保护**: 即使加密被破解，密码仍经过 SHA256 哈希存储，进一步保护用户密码
4. **兼容性**: 后端解密失败时会兼容处理，确保平滑过渡

### 生产环境建议

1. **密钥配置化**: 将密钥移到 `appsettings.json` 或环境变量中
   ```json
   {
     "Encryption": {
       "Key": "YourSecretKeyHere",
       "IV": "YourIVHere"
     }
   }
   ```

2. **使用 HTTPS**: 配置 SSL/TLS 证书，确保传输层安全

3. **定期更换密钥**: 定期更新加密密钥和 IV

4. **密码策略**: 
   - 要求用户使用强密码
   - 实施密码过期策略
   - 记录登录日志

5. **额外安全措施**:
   - 实施登录失败锁定策略
   - 记录 IP 地址和设备信息
   - 实施双重身份验证（2FA）

### 注意事项

1. **密钥一致性**: 前后端密钥和 IV 必须完全一致，否则无法解密
2. **不要提交密钥**: 如果将密钥移到配置文件，确保 `.gitignore` 中排除包含密钥的配置文件
3. **错误处理**: 解密失败时会返回原值，确保有适当的错误处理机制
4. **性能考虑**: AES 加密解密性能良好，对用户体验无明显影响

## Startup.cs 配置分离说明
### 概述
为提高项目可维护性，已将原本集中在`Program.cs`文件中的大量配置逻辑分离至独立的`Startup.cs`类中。这样使主程序文件更简洁，配置逻辑更集中易管理。

### 配置分离内容
#### 1. Startup.cs 结构
**文件位置**: `DDDProject.API/Startup.cs`
- **构造函数**: 接收 IConfiguration 配置对象
- **ConfigureServices(IServiceCollection services)**: 注册各种服务
- **Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Program> logger)**: 配置HTTP请求管道

#### 2. 原 Program.cs 配置逻辑迁移
- 服务注册逻辑（AddControllers, AddDbContext, JWT配置等）
- 中间件配置（UseRouting, UseAuthentication, UseAuthorization等）
- CORS配置
- Swagger/Swashbuckle配置
- 数据库迁移和种子数据初始化

#### 3. 重构后 Program.cs 文件
现在 Program.cs 只保留最小必需代码：
```csharp
var builder = WebApplication.CreateBuilder(args);
var startup = new Startup(builder.Configuration);

// 配置服务
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// 配置应用管道
startup.Configure(app, app.Environment, app.Services.GetRequiredService<ILogger<Program>>());

app.Run();
```

### 仓储接口注册修复
为解决依赖注入错误，对仓储接口注册进行了完善：

#### 1. Repository 实现补充
- **文件位置**: `DDDProject.Infrastructure.Repositories.RepositorySimple.cs`
- 添加了针对使用 Guid 作为主键的实体的 `IRepository<T>` 接口实现
- 原有的 `IRepository<T,TId>` 接口实现在 `Repository.cs` 中

#### 2. 服务注册完善
**文件位置**: `DDDProject.Infrastructure.ServiceCollectionExtensions.cs`
```csharp
public static IServiceCollection AddRepositories(this IServiceCollection services)
{
    services.AddScoped(typeof(IRepository<,>), typeof(Infrastructure.Repositories.Repository<,>));
    services.AddScoped(typeof(IRepository<>), typeof(Infrastructure.Repositories.Repository<>));
    
    // 注册时间服务
    services.AddScoped<ITimeService, ChinaStandardTimeService>();
    
    return services;
}
```

### 优势
1. **更好的组织性**: 将服务配置和中间件配置逻辑分别封装到Startup类的不同方法中
2. **可维护性**: 使得配置代码更易于定位和修改
3. **遵循传统模式**: 使项目结构更接近传统的.NET Core启动配置模式
4. **解决依赖注入问题**: 确保应用服务（如 MenuService、RoleService）能正确解析仓储依赖

## JWT认证配置说明

### 概述
本项目使用JWT（JSON Web Token）作为身份验证机制，确保API的安全访问。JWT令牌通过HTTP Authorization头部携带，使用Bearer方案进行传输。

### 配置参数

#### 1. JWT设置
**文件位置**: `DDDProject/DDLProject.API/appsettings.json`

```json
{
  "JwtSettings": {
    "Issuer": "DDDProject",
    "Audience": "DDDProject", 
    "Key": "YourSuperSecretAndLongEnoughKeyForSecureToken12345678901234567890123",
    "ExpireMinutes": 720
  }
}
```

**配置说明**:
- `Issuer`: JWT签发者，标识令牌来源
- `Audience`: JWT接收方，标识令牌目标
- `Key`: JWT签名密钥，必须足够长且保密
- `ExpireMinutes`: 令牌有效时间（分钟）

#### 2. 启动配置
**文件位置**: `DDDProject/DDLProject.API/Program.cs`

- 自动注册JWT认证中间件
- 配置TokenValidationParameters参数
- 设置HttpContextAccessor服务以获取当前用户信息

### 使用方式

#### 1. 认证流程
1. 用户通过登录接口获取JWT令牌
2. 客户端在后续请求的Authorization头部携带Bearer令牌
3. 服务端验证令牌的有效性和完整性
4. 成功验证后提供受保护的资源

#### 2. 受保护的控制器
所有需要身份验证的控制器需使用`[Authorize]`特性：

```csharp
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize] // 需要身份验证
public class MenuController : BaseApiController
{
    // 需要身份验证的方法
}
```

### 当前用户信息获取

#### 1. CurrentUser服务
**文件位置**: `DDDProject.API/Extensions/CurrentUser.cs`

实现了获取当前登录用户信息的全局类：

- `UserId`: 获取当前用户的ID
- `UserName`: 获取当前用户的用户名  
- `RealName`: 获取当前用户的真实姓名
- `IsAuthenticated`: 判断用户是否已认证

#### 2. 依赖注入配置
在`Program.cs`中已注册CurrentUser服务：

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>();
```

#### 3. 在控制器中使用
```csharp
public class MenuController : BaseApiController
{
    private readonly CurrentUser _currentUser;

    public MenuController(IMenuService menuService, CurrentUser currentUser)
    {
        _menuService = menuService;
        _currentUser = currentUser;
    }
    
    [HttpGet]
    public async Task<ApiRequestResult> GetUserSpecificDataAsync()
    {
        var userId = _currentUser.UserId;
        var userName = _currentUser.UserName;
        // 使用当前用户信息进行业务处理
    }
}
```

### 安全性说明

1. **令牌存储**: JWT令牌存储在客户端，无需服务器维护会话状态
2. **密钥安全**: 签名密钥必须保密，生产环境中应从安全配置源获取
3. **过期时间**: 设置合理的过期时间平衡安全性和用户体验
4. **HTTPS传输**: 生产环境中必须通过HTTPS传输以防止中间人攻击
5. **令牌验证**: 每次访问受保护资源时都验证令牌的完整性和有效性

### 生产环境建议

1. **密钥轮换**: 定期更换JWT签名密钥
2. **过期策略**: 设置较短的访问令牌过期时间和较长的刷新令牌有效期
3. **审计日志**: 记录JWT验证失败的尝试
4. **CORS安全**: 限制允许的源和方法
5. **黑名单管理**: 对注销用户的令牌进行黑名单管理

### 令牌组成结构
- **Header**: 包含算法信息和令牌类型
- **Payload**: 包含声明信息（Subject、Name、GivenName等）
- **Signature**: 使用密钥对header和payload进行签名

## 时区设置说明

### 中国标准时间（UTC+8）

本项目所有时间相关操作都使用**中国标准时间**（CST，UTC+8）以确保时间显示的一致性。

### 实现方式

1. **后端配置**：
   - 实体创建时间使用 `DateTime.Now` 替代 `DateTime.UtcNow`
   - 数据库存储使用服务器本地时间（配置为CST）
   - 时间服务统一处理中国标准时间

2. **数据库配置**：
   - 通过 `GETDATE()` 函数使用服务器本地时间
   - 不使用 UTC 时间函数

3. **部署要求**：
   - 生产服务器应设置时区为 **(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi**
   - Docker 容器应设置 `Asia/Shanghai` 时区

### 时间服务（ITimeService）

项目中引入了时间服务来统一处理时区：

```csharp
public interface ITimeService
{
    DateTime GetCurrentTime();
    DateTimeOffset GetCurrentOffsetTime();
}

public class ChinaStandardTimeService : ITimeService
{
    private static readonly TimeZoneInfo ChinaTimeZone = 
        TimeZoneInfo.FindSystemTimeZoneById("China Standard Time") ?? 
        TimeZoneInfo.Local;
    
    public DateTime GetCurrentTime()
    {
        return TimeZoneInfo.ConvertTime(DateTime.UtcNow, ChinaTimeZone).DateTime;
    }
    
    public DateTimeOffset GetCurrentOffsetTime()
    {
        return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, ChinaTimeZone);
    }
}
```

### 部署配置

#### Windows 服务器
```cmd
# 设置系统时区为北京时间
tzutil /s "China Standard Time"
```

#### Linux 服务器（Docker环境）
```bash
# 设置时区为上海
sudo timedatectl set-timezone Asia/Shanghai
```

#### Dockerfile 时区配置
```dockerfile
# Ubuntu/Debian
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN dpkg-reconfigure -f noninteractive tzdata

# 或者设置环境变量
ENV TZ=Asia/Shanghai
RUN echo $TZ > /etc/timezone
```

### 注意事项

1. **数据库服务器时间**：确保数据库服务器时间设置为CST
2. **应用程序服务器时间**：确保应用服务器时间设置为CST 
3. **客户端时间**：前端使用的时间戳基于服务端时间，保证一致性
4. **日志记录**：所有日志时间均记录为CST时间
5. **用户界面**：所有显示时间都来自服务端，确保统一性
