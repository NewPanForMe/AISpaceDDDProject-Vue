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
- **Repositories**: 仓储的具体实现（Repository）
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

### Service 层异步示例

```csharp
// IUserDataService.cs (Interfaces 目录)
public interface IUserDataService : IApplicationService
{
    Task<UserData> GetUserByIdAsync(Guid userId);
    Task<List<UserData>> GetAllUsersAsync();
    Task<UserData> CreateUserAsync(UserData user);
    Task<UserData> UpdateUserAsync(UserData user);
    Task<bool> DeleteUserAsync(Guid userId);
}

// UserDataService.cs (Services 目录)
public class UserDataService : IUserDataService
{
    private readonly IRepository<UserData> _userRepository;

    public UserDataService(IRepository<UserData> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserData> GetUserByIdAsync(Guid userId)
    {
        // 使用 FindAsync 替代 Find
        return await _userRepository.FindAsync(userId);
    }

    public async Task<List<UserData>> GetAllUsersAsync()
    {
        // 使用 ToListAsync 替代 ToList
        return await _userRepository.GetListAsync(u => true);
    }

    public async Task<UserData> CreateUserAsync(UserData user)
    {
        // 使用 AddAsync 替代 Add
        await _userRepository.AddAsync(user);
        // 使用 SaveChangesAsync 替代 SaveChanges
        await _userRepository.SaveChangesAsync();
        return user;
    }

    public async Task<UserData> UpdateUserAsync(UserData user)
    {
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.FindAsync(userId);
        if (user == null) return false;
        
        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();
        return true;
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
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userDataService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userDataService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserData user)
    {
        var createdUser = await _userDataService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserData user)
    {
        var updatedUser = await _userDataService.UpdateUserAsync(user);
        return Ok(updatedUser);
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
[ApiController]
[Route("api/[controller]/[action]")]  // 路由模板：api/Login/LoginAsync
public class LoginController : BaseApiController
{
    private readonly ILoginService _loginService;

    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    // ✅ 正确：使用 [ActionName] 指定路由路径
    [HttpPost]
    [ActionName("LoginAsync")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var result = await _loginService.LoginAsync(request);
        return Ok(result);
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

### Repository 层异步实现示例

```csharp
public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity<Guid>
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken = default)
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

## 修复记录

### 2026-03-20 - 修复后端解密失败问题

**问题描述**: 后端无法正确解密前端加密的密码，导致登录失败。

**问题原因**: 
- 后端 `PasswordHelper.DecryptPassword` 方法在处理非 OpenSSL 格式的加密数据时，密钥和 IV 的长度处理不正确
- CryptoJS 的 `AES.encrypt` 直接使用 `CryptoJS.enc.Utf8.parse()` 转换的字节数组
- .NET 的 `Aes` 类需要精确匹配密钥和 IV 的长度，而原代码使用了错误的填充方式

**修复内容**:
- 修改了 `DDDProject.Domain/Helpers/PasswordHelper.cs` 中的 `DecryptPassword` 方法
- **密钥**: 直接使用 UTF-8 字节（24 字节），不进行填充
- **IV**: 截断到 16 字节（从 17 字节的字符串取前 16 字节）

**验证结果**:
- 加密字符串 `Xd+XjJjHDusCva7dLEDJWQ==` 成功解密为 `12345`
- SHA256 哈希匹配正确

**修改文件**:
- `DDDProject.Domain/Helpers/PasswordHelper.cs`

**测试方法**:
```bash
# 重新构建后端
cd DDDProject
dotnet build DDDProject.slnx

# 重新启动后端服务
dotnet run --project DDDProject.API
```

**相关文件**:
- 前端加密: `DDDVue/src/utils/crypto.ts`
- 登录服务: `DDDProject.Application/Services/LoginService.cs`
- 密码帮助类: `DDDProject.Domain/Helpers/PasswordHelper.cs`
