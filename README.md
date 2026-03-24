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

## 开发说明

本章节详细说明项目的开发规范和最佳实践，包括异步编程、各层规范、API 功能、树形结构处理、端口配置、Git 工作流、JWT 认证和权限认证等。

### 异步编程规范

#### 数据库交互必须使用异步方法

在本项目中，所有与数据库交互的操作**必须使用异步方法**，以避免阻塞线程和提高性能。

#### EF Core 异步方法对照表

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

#### 注意事项

1. **避免阻塞调用**: 不要在异步方法中使用 `.Result`、`.Wait()` 或 `.GetAwaiter().GetResult()`，这会导致死锁和性能问题
2. **保持异步链**: 从 Controller 到 Service 到 Repository，整个调用链都应该是异步的
3. **使用 CancellationToken**: 对于长时间运行的操作，应接受 `CancellationToken` 参数
4. **异常处理**: 异步方法的异常处理与同步方法相同，可以使用 try-catch 块

### Service 层规范

在本项目中，新建 Service 时应遵循以下规范：

#### 1. 基本规范

1. **新建接口**: 在 `DDDProject.Application\Interfaces` 目录下创建接口文件
2. **新建实现类**: 在 `DDDProject.Application\Services` 目录下创建实现类
3. **继承关系**:
   - Service 实现类继承自对应的接口
   - 接口继承自 `IApplicationService`
   - `IApplicationService` 是所有应用服务的基接口
4. **异步方法命名**: 所有异步方法名必须添加 `Async` 后缀
5. **数据库操作**: 所有数据库操作必须使用异步方法
6. **返回值处理**: Service 方法返回 `ApiRequestResult` 时，应直接使用 `new ApiRequestResult` 创建实例

#### 1.1 枚举类型使用规范

**使用枚举类型替代字符串常量**：
- 枚举类型提供编译时类型检查，避免拼写错误
- 枚举值具有明确的语义，提高代码可读性
- 支持 Intellisense 自动补全，提升开发效率

**示例**：
```csharp
// ❌ 错误：使用字符串常量
[ApiSearch(Name = "获取菜单列表", Category = "菜单管理")]
public async Task<ApiRequestResult> GetMenusAsync()

// ✅ 正确：使用枚举类型
[ApiSearch(Name = "获取菜单列表", Category = ApiSearchCategory.Menu)]
public async Task<ApiRequestResult> GetMenusAsync()
```

**枚举定义位置**：枚举类型应定义在 `DDDProject.Application\Common` 目录下

#### 2. 分页请求规范

**新建 Service 时，获取列表数据必须使用分页请求方法，分页请求参数统一使用 `PagedRequest` 类**：
- 分页参数包含 `PageNumber`（页码）和 `PageSize`（每页大小）
- 返回值使用 `PagedResult<T>` 泛型类，包含 `List`（数据列表）、`Total`（总记录数）、`PageNumber`、`PageSize`、`TotalPages`（总页数）

**示例**：
```csharp
public async Task<ApiRequestResult> GetMenusAsync(PagedRequest request)
{
    var skipCount = (request.PageNumber - 1) * request.PageSize;
    var total = await _repository.CountAsync(m => true);
    var menus = await _repository.GetListAsync(m => true, q => q.OrderBy(m => m.SortOrder), skipCount, request.PageSize);

    var pagedResult = new PagedResult<MenuDto>
    {
        List = menuDtos.ToList(),
        Total = total,
        PageNumber = request.PageNumber,
        PageSize = request.PageSize
    };

    return new ApiRequestResult { Success = true, Message = "操作成功", Data = pagedResult };
}
```

#### 3. Service 层异步示例

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

### Controller 层规范

#### 1. 注解规范

在本项目中，Controller 方法的路由注解必须遵循以下规范：

**必须使用 `[ActionName("方法名")]` 注解**，确保路由路径与方法名一致：
- HTTP 方法注解（如 `[HttpPost]`、`[HttpGet]` 等）不需要指定路径
- 路由模板在控制器级别使用 `[Route("api/[controller]/[action]")]`，其中 `[action]` 会自动使用方法名
- 控制器的注解无需 `[ActionName("方法名")]`

**注解格式**：
```csharp
[ApiController]
[Route("api/[controller]/[action]")]  // 控制器级别路由模板
public class YourController : BaseApiController
{
    [HttpPost]
    [ActionName("方法名")]  // 方法名必须与注解中的名称一致
    public async Task<ApiRequestResult> 方法名([FromBody] RequestModel model)
    {
        // 实现逻辑
    }
}
```

**正确示例**：
```csharp
// 方法名: LoginAsync
[HttpPost]
[ActionName("LoginAsync")]
public async Task<ApiRequestResult> LoginAsync([FromBody] LoginRequest request)
{
    // 实现逻辑
}

// 方法名: GetUserByIdAsync
[HttpGet]
[ActionName("GetUserByIdAsync")]
public async Task<ApiRequestResult> GetUserByIdAsync(Guid id)
{
    // 实现逻辑
}

// 方法名: CreateUserAsync
[HttpPost]
[ActionName("CreateUserAsync")]
public async Task<ApiRequestResult> CreateUserAsync([FromBody] CreateUserRequest request)
{
    // 实现逻辑
}
```

**错误示例**：
```csharp
// ❌ 错误：在 HttpPost 中指定路径
[HttpPost("LoginAsync")]
public async Task<ApiRequestResult> LoginAsync([FromBody] LoginRequest request)
{
    // 这会导致路由重复配置，且与 [ActionName] 不一致
}

// ❌ 错误：缺少 [ActionName] 注解
[HttpPost]
public async Task<ApiRequestResult> LoginAsync([FromBody] LoginRequest request)
{
    // 这会导致路由路径使用方法名 LoginAsync，但与 api/index.ts 配置不一致
}

// ❌ 错误：[ActionName] 与方法名不一致
[HttpPost]
[ActionName("Login")]
public async Task<ApiRequestResult> LoginAsync([FromBody] LoginRequest request)
{
    // 这会导致路由路径为 Login，但方法名为 LoginAsync
}
```

#### 2. Controller 方法注解规范

**为什么需要 `[ActionName]`？**
- **API 路径一致性**: 确保前端 `api/index.ts` 中定义的 API 路径与后端 `[ActionName]` 注解一致
- **路由清晰**: `[Route("api/[controller]/[action]")]` 中的 `[action]` 会自动使用 `[ActionName]` 的值
- **代码可读性**: 开发者可以直观地从 `[ActionName]` 知道对应的 API 路径
- **减少错误**: 避免因路由不一致导致的 404 或 405 错误
- **灵活性**: 可以独立控制路由路径和方法名，便于重构

#### 3. 完整示例

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
    LoginAsync: 'api/Login/LoginAsync',  // 必须与 [ActionName("LoginAsync")] 一致
  },
};
```

**路由路径说明**:
- 控制器路由: `[Route("api/[controller]/[action]")]`
- 控制器名: `LoginController` → `[controller]` = `Login`
- 方法名: `LoginAsync` → `[action]` = `[ActionName]` 的值 = `LoginAsync`
- 最终路由: `api/Login/LoginAsync`

#### 4. Controller 层异步示例

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
       
        return await _userDataService.GetUserByIdAsync(id);; // Assuming service returns ApiRequestResult
    }

    [HttpGet]
    [ActionName("GetAllUsersAsync")]
    public async Task<ApiRequestResult> GetAllUsersAsync()
    {
     
        return await _userDataService.GetAllUsersAsync();; // Assuming service returns ApiRequestResult
    }

    [HttpPost]
    [ActionName("CreateUserAsync")]
    public async Task<ApiRequestResult> CreateUserAsync([FromBody] UserData user)
    {
      
        return await _userDataService.CreateUserAsync(user);; // Assuming service returns ApiRequestResult
    }

    [HttpPut]
    [ActionName("UpdateUserAsync")]
    public async Task<ApiRequestResult> UpdateUserAsync([FromBody] UserData user)
    {
    
        return await _userDataService.UpdateUserAsync(user);; // Assuming service returns ApiRequestResult
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

#### 5. Service 层返回值约定

所有 Service 方法的返回类型应该是 `Task<ApiRequestResult>` 或相关泛型变体。这是为了让控制器能够方便地返回统一格式的结果：

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

#### 6. 编写要点

1. **继承 BaseApiController**: 所有控制器必须继承 `BaseApiController`
2. **注入服务**: 通过构造函数注入所需的 Service
3. **使用 [ActionName]**: 按照注解规范使用 ActionName
4. **异步方法**: 控制器方法使用 `async/await` 并返回 `Task<IActionResult>`
5. **引用 ApiSearch**: 添加适当的 `[ApiSearch]` 标签用于接口发现
6. **统一返回**: 调用服务层方法后，直接返回 `Ok(result)` 给前端
7. **Service 返回值**: 所有服务方法必须返回 `Task<ApiRequestResult>` 使得 Controller 可以直接转发结果

### Repository 层规范

#### 双泛型仓储实现（Repository<TEntity, TId>）

**文件位置**: `DDDProject.Infrastructure\Repositories\Repository.cs`
- 实现 `IRepository<TEntity, TId>` 接口（带实体和 ID 类型）
- 针对各种主键类型的通用实体仓储实现

#### 单泛型仓储实现（Repository<TEntity>）

**文件位置**: `DDDProject.Infrastructure\Repositories\RepositorySimple.cs`
- 实现 `IRepository<TEntity>` 接口（仅带实体类型，默认 Guid 主键）
- 专门针对使用 Guid 作为主键的实体的仓储实现
- 服务于 Menu、User 等需要 `IRepository<Menu>` 和 `IRepository<User>` 的场景
- 新建类型都使用单泛型仓储实现

#### RepositorySimple 示例实现

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

### 树形结构数据处理规范

#### 1. 树形结构特征

在本项目中，遇到以下字段时，表明该实体为树形结构：
- `ParentId` (Guid? 或 string)
- `ParentCode` (string)
- `ParentId` (int?)
- 其他类似 `Parent*` 的字段

树形结构数据具有以下特征：
- 根节点的 `ParentId` 为 `null` 或空字符串
- 子节点的 `ParentId` 指向父节点的 `Id`
- 数据之间存在层级关系

#### 2. Service 层处理规范

当处理树形结构数据时，需要递归构建树形结构：

**示例：菜单服务中的树形结构构建**
```csharp
public async Task<ApiRequestResult> GetTreeMenusAsync()
{
    var menus = await _menuRepository.GetListAsync(m => true);
    var menuList = menus.ToList();

    // 获取根节点
    var rootMenus = menuList.Where(m => m.ParentId == null || m.ParentId == Guid.Empty)
                           .OrderBy(m => m.SortOrder)
                           .ToList();

    // 递归构建树形结构
    var treeMenus = BuildTreeMenu(rootMenus, menuList);

    return new ApiRequestResult
    {
        Success = true,
        Message = "操作成功",
        Data = treeMenus
    };
}

// 递归构建树形菜单结构
private List<MenuDto> BuildTreeMenu(List<Menu> rootMenus, List<Menu> allMenus)
{
    var result = new List<MenuDto>();

    foreach (var menu in rootMenus)
    {
        var menuDto = new MenuDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Path = menu.Path,
            Component = menu.Component,
            Icon = menu.Icon,
            ParentId = menu.ParentId,
            SortOrder = menu.SortOrder,
            Status = menu.Status,
            Children = new List<MenuDto>()
        };

        // 递归获取子菜单
        var children = allMenus.Where(m => m.ParentId == menu.Id).OrderBy(m => m.SortOrder).ToList();
        if (children.Any())
        {
            menuDto.Children = BuildTreeMenu(children, allMenus);
        }

        result.Add(menuDto);
    }

    return result;
}
```

#### 3. 前端表格层级显示

当数据包含 `children` 字段时，前端表格需要使用层级显示：

**Element Plus Table 层级显示配置**：
```vue
<template>
  <el-table
    :data="menuList"
    row-key="id"
    :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
  >
    <el-table-column prop="name" label="菜单名称" />
    <el-table-column prop="path" label="路由路径" />
    <el-table-column prop="component" label="组件路径" />
    <el-table-column prop="icon" label="图标" />
    <el-table-column prop="sortOrder" label="排序" />
    <el-table-column prop="status" label="状态" />
    <el-table-column label="操作" width="200" fixed="right">
      <template #default="{ row }">
        <el-button size="small" @click="editMenu(row)">编辑</el-button>
        <el-button size="small" type="danger" @click="deleteMenu(row.id)">删除</el-button>
        <el-button size="small" type="primary" @click="addChildMenu(row)">添加子菜单</el-button>
      </template>
    </el-table-column>
  </el-table>
</template>

<script setup lang="ts">
// 菜单数据类型（包含 children 字段）
interface Menu {
  id: string | number
  name: string
  path: string
  component: string
  icon?: string
  parentId?: string | number
  sortOrder: number
  status: number
  children?: Menu[]  // 递归子菜单
}
</script>
```

**关键配置说明**：
- `row-key="id"`: 指定行的唯一标识
- `:tree-props="{ children: 'children', hasChildren: 'hasChildren' }"`: 指定树形结构的字段
  - `children`: 子节点数组字段名
  - `hasChildren`: 是否有子节点字段（可选）

#### 4. 分页与树形结构

当数据量较大时，需要分页获取树形数据：

**后端分页获取树形数据**：
```csharp
public async Task<ApiRequestResult> GetMenusByPagingAsync(PagedRequest request)
{
    var allMenus = new List<Menu>();
    var currentPage = request.PageNumber;

    // 循环分页获取所有数据
    while (true)
    {
        var menus = await _menuRepository.GetListAsync(
            m => true,
            q => q.OrderBy(x => x.SortOrder),
            (currentPage - 1) * request.PageSize,
            request.PageSize
        );

        if (!menus.Any()) break;

        allMenus.AddRange(menus);

        if (menus.Count() < request.PageSize) break;
        currentPage++;
    }

    // 构建树形结构
    var menuDtos = BuildMenuDtoTree(allMenus);

    return new ApiRequestResult
    {
        Success = true,
        Message = "操作成功",
        Data = menuDtos
    };
}
```

**前端调用**：
```typescript
// 获取树形菜单数据（分页方式）
const response = await http.get(api.Menu.GetMenusByPagingAsync, {
  params: { pageNum: 1, pageSize: 1000 }
})
menuList.value = response.data || []
```

#### 5. 注意事项

1. **递归深度**: 树形结构的递归深度不宜过深，建议不超过 5 层
2. **性能优化**: 数据量较大时，使用分页获取所有数据后构建树形结构
3. **字段一致性**: 前后端的 `children` 字段名必须保持一致
4. **空值处理**: 根节点的 `ParentId` 应为 `null` 或空字符串
5. **排序**: 树形结构数据应按 `SortOrder` 字段排序

### API Search 功能说明

#### 功能概述

API Search 功能用于自动扫描和检索项目中所有标记了 `[ApiSearch]` 注解的 API 方法，并生成 JSON 格式的 API 列表。

#### 组件说明

##### 1. ApiSearchAttribute (注解)

**位置**: `DDDProject.Application\Common\ApiSearchAttribute.cs`

用于标记需要搜索的 API 方法。

**属性**:
- `Name`: API 名称
- `Description`: API 描述
- `Category`: API 分类（使用 `ApiSearchCategory` 枚举）

**ApiSearchCategory 枚举值**:
- `Login`: 登录认证相关操作
- `Menu`: 菜单管理相关操作
- `User`: 用户管理相关操作
- `Role`: 角色管理相关操作
- `Dictionary`: 数据字典相关操作
- `Log`: 日志管理相关操作
- `Setting`: 系统设置相关操作
- `File`: 文件上传相关操作
- `Other`: 其他操作

**使用示例**：
```csharp
[HttpGet]
[ActionName("GetAsync")]
[ApiSearch(Name = "获取示例列表", Description = "返回示例数据列表", Category = ApiSearchCategory.Example)]
public async Task<ApiRequestResult> GetAsync()
{
    return await yourService.GetAsync(); // Service method should return ApiRequestResult
}
```

##### 2. ApiSearchController (控制器)

**位置**: `DDDProject.API\Controllers\ApiSearchController.cs`

提供 API 搜索功能的控制器。

**可用接口**:

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/ApiSearch/Search` | GET | 获取所有标记了 ApiSearch 注解的 API |
| `/api/ApiSearch/SearchByCategory?category=xxx` | GET | 根据分类获取 API 列表 |
| `/api/ApiSearch/SearchByKeyWord?keyword=xxx` | GET | 根据关键词搜索 API |

##### 3. ApiSearchService (服务)

**位置**: `DDDProject.Application\Services\ApiSearchService.cs`

用于扫描控制器并生成 API 列表的服务。

**主要方法**:
- `GetApiSearchList()`: 获取所有 API
- `GetApiSearchListByCategory(string category)`: 按分类筛选（支持枚举值或字符串）
- `GetApiSearchListByKeyWord(string keyword)`: 按关键词搜索

#### 使用方法

##### 1. 在控制器方法上添加注解

```csharp
using DDDProject.Application.Common;

[ApiController]
[Route("api/[controller]/[action]")]
public class YourController : BaseApiController
{
    [HttpGet]
    [ApiSearch(Name = "获取数据", Description = "获取用户数据列表", Category = ApiSearchCategory.User)]
	public async Task<ApiRequestResult> GetAsync()
    {
        // 业务逻辑
    }
}
```

##### 2. 调用搜索接口

```bash
# 获取所有 API
curl http://localhost:5272/api/ApiSearch/Search

# 按分类搜索（使用枚举值名称）
curl http://localhost:5272/api/ApiSearch/SearchByCategory?category=User

# 按关键词搜索
curl http://localhost:5272/api/ApiSearch/SearchByKeyWord?keyword=用户
```

##### 3. 前端调用示例

```javascript
import api from '@/api'

// 获取所有 API
api.ApiSearch.Search()

// 按分类搜索
api.ApiSearch.SearchByCategory({ category: '用户' })

// 按关键词搜索
api.ApiSearch.SearchByKeyWord({ keyword: '用户' })
```

### 端口配置说明（禁止随意更改）

本项目有严格的端口配置规范，**禁止随意更改**，以确保前后端正常通信：

#### 前端项目（DDDVue）

- **默认端口**: `5173`
- **访问地址**: http://localhost:5173/
- **配置文件**: `DDDVue/vite.config.ts`
- **修改方式**: 如需修改端口，请编辑 `DDDVue/vite.config.ts` 中的 `server.port` 配置

#### 后端项目（DDDProject）

- **默认端口**: `5272`
- **访问地址**: http://localhost:5272/
- **Swagger 地址**: http://localhost:5272/swagger
- **配置文件**: `DDDProject/API/appsettings.json` 和 `DDDProject/API/Properties/launchSettings.json`
- **修改方式**: 如需修改端口，请编辑 `DDDProject/API/Properties/launchSettings.json` 中的 `applicationUrl` 配置

#### 端口冲突处理

如果遇到端口被占用的情况：
1. **前端端口冲突**: 修改 `DDDVue/vite.config.ts` 中的端口配置
2. **后端端口冲突**: 修改 `DDDProject/API/Properties/launchSettings.json` 中的 `applicationUrl`
3. **修改后务必同步更新相关配置文件中的端口号**

#### 端口配置文件位置

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

### Git 工作流规范

#### 分支命名规范

- `master`: 主分支，稳定版本
- `feature/xxx`: 功能分支
- `bugfix/xxx`: 修复分支
- `hotfix/xxx`: 紧急修复分支
- `develop`: 开发分支（如需要）

#### 提交信息规范

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

#### 工作流程

1. 从 `master` 拉取最新代码
2. 创建功能分支 `feature/xxx`
3. 在功能分支上开发并提交
4. 保持分支与 `master` 同步（定期 `git pull origin master`）
5. 开发完成后合并到 `master`
6. 推送到远程仓库

### JWT 认证配置说明

#### 概述

本项目使用 JWT（JSON Web Token）作为身份验证机制，确保 API 的安全访问。JWT 令牌通过 HTTP Authorization 头部携带，使用 Bearer 方案进行传输。

#### 配置参数

##### 1. JWT 设置

**文件位置**: `DDDProject/DDLProject.API/appsettings.json`

```json
{
  "JwtSettings": {
    "Issuer": "DDDProject",
    "Audience": "DDDProject",
    "Key": "1fe277c55303f1c97e0d5861959039077",
    "ExpireMinutes": 720
  }
}
```

**配置说明**:
- `Issuer`: JWT 签发者，标识令牌来源
- `Audience`: JWT 接收方，标识令牌目标
- `Key`: JWT 签名密钥，必须足够长且保密
- `ExpireMinutes`: 令牌有效时间（分钟）

##### 2. 启动配置

**文件位置**: `DDDProject/DDLProject.API/Program.cs`

- 自动注册 JWT 认证中间件
- 配置 TokenValidationParameters 参数
- 设置 HttpContextAccessor 服务以获取当前用户信息

#### 使用方式

##### 1. 认证流程

1. 用户通过登录接口获取 JWT 令牌
2. 客户端在后续请求的 Authorization 头部携带 Bearer 令牌
3. 服务端验证令牌的有效性和完整性
4. 成功验证后提供受保护的资源

##### 2. 受保护的控制器

所有需要身份验证的控制器需使用 `[Authorize]` 特性：

```csharp
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize] // 需要身份验证
public class MenuController : BaseApiController
{
    // 需要身份验证的方法
}
```

#### 当前用户信息获取

##### 1. CurrentUser 服务

**文件位置**: `DDDProject.API/Extensions/CurrentUser.cs`

实现了获取当前登录用户信息的全局类：

- `UserId`: 获取当前用户的 ID
- `UserName`: 获取当前用户的用户名
- `RealName`: 获取当前用户的真实姓名
- `IsAuthenticated`: 判断用户是否已认证

##### 2. 依赖注入配置

在 `Program.cs` 中已注册 CurrentUser 服务：

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>();
```

##### 3. 在控制器中使用

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

#### 安全性说明

1. **令牌存储**: JWT 令牌存储在客户端，无需服务器维护会话状态
2. **密钥安全**: 签名密钥必须保密，生产环境中应从安全配置源获取
3. **过期时间**: 设置合理的过期时间平衡安全性和用户体验
4. **HTTPS 传输**: 生产环境中必须通过 HTTPS 传输以防止中间人攻击
5. **令牌验证**: 每次访问受保护资源时都验证令牌的完整性和有效性

#### 生产环境建议

1. **密钥轮换**: 定期更换 JWT 签名密钥
2. **过期策略**: 设置较短的访问令牌过期时间和较长的刷新令牌有效期
3. **审计日志**: 记录 JWT 验证失败的尝试
4. **CORS 安全**: 限制允许的源和方法
5. **黑名单管理**: 对注销用户的令牌进行黑名单管理

#### 令牌组成结构

- **Header**: 包含算法信息和令牌类型
- **Payload**: 包含声明信息（Subject、Name、GivenName等）
- **Signature**: 使用密钥对 header 和 payload 进行签名

### 权限认证说明

#### 概述

本项目使用 JWT 认证机制，结合自定义权限检查中间件实现灵活的权限控制。**只有标记了 `[Authorize]` 注解的 API 才需要登录验证**，未标记的 API 可以匿名访问。

#### 权限检查中间件

##### 工作原理

1. **中间件位置**：在 `UseRouting`、`UseAuthentication`、`UseAuthorization` 之后执行
2. **检查逻辑**：
   - 检查当前请求的控制器或方法是否有 `[Authorize]` 注解
   - 如果没有 `[Authorize]` 注解，跳过权限检查，允许匿名访问
   - 如果有 `[Authorize]` 注解，验证用户是否已通过 JWT 认证
   - 未认证的请求返回 401 错误

##### 使用方式

**无需登录的 API**（默认行为）：
```csharp
[ApiController]
[Route("api/[controller]/[action]")]
public class PublicController : BaseApiController
{
    [HttpGet]
    [ActionName("GetPublicData")]
    public async Task<ApiRequestResult> GetPublicData()
    {
        // 无需登录即可访问
    }
}
```

**需要登录的 API**：
```csharp
// 方式1：在控制器级别添加 [Authorize]（所有方法都需要登录）
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class ProtectedController : BaseApiController
{
    [HttpGet]
    [ActionName("GetUserData")]
    public async Task<ApiRequestResult> GetUserData()
    {
        // 需要登录
    }
}

// 方式2：在方法级别添加 [Authorize]（仅该方法需要登录）
[ApiController]
[Route("api/[controller]/[action]")]
public class MixedController : BaseApiController
{
    [HttpGet]
    [ActionName("GetPublicData")]
    public async Task<ApiRequestResult> GetPublicData()
    {
        // 无需登录
    }

    [HttpGet]
    [ActionName("GetUserData")]
    [Authorize]
    public async Task<ApiRequestResult> GetUserData()
    {
        // 需要登录
    }
}
```

#### CurrentUser 服务

##### 功能说明

`CurrentUser` 服务用于在控制器中获取当前登录用户的信息。该服务从 JWT Token 的 Claim 中提取用户数据。

##### Claim 字段映射

`CurrentUser` 从 JWT Token 中读取以下 Claim（与 `LoginService.CreateJwtTokenAsync` 生成的 Claim 一致）：

| CurrentUser 属性 | Claim 类型 | 说明 |
|-----------------|-----------|------|
| `UserId` | `sub` / `JwtRegisteredClaimNames.Sub` | 用户 ID (Guid) |
| `UserName` | `ClaimTypes.Name` / `JwtRegisteredClaimNames.Name` / `JwtRegisteredClaimNames.UniqueName` | 用户名 |
| `RealName` | `ClaimTypes.GivenName` / `"givenname"` | 真实姓名 |

##### 使用示例

```csharp
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class UserController : BaseApiController
{
    private readonly ICurrentUser _currentUser;

    public UserController(CurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpGet]
    [ActionName("GetUserInfo")]
    public async Task<ApiRequestResult> GetUserInfo()
    {
        var userId = _currentUser.UserId;      // 从 Token 的 sub claim 获取
        var userName = _currentUser.UserName;  // 从 Token 的 name claim 获取
        var realName = _currentUser.RealName;  // 从 Token 的 givenname claim 获取

        return new ApiRequestResult
        {
            Success = true,
            Message = "获取用户信息成功",
            Data = new { UserId = userId, UserName = userName, RealName = realName }
        };
    }
}
```

##### 依赖注入配置

`CurrentUser` 已在 `Startup.cs` 中注册：
```csharp
services.AddHttpContextAccessor();
services.AddScoped<CurrentUser>();
```

#### JWT Token 生成

##### Claim 列表

登录成功后，`LoginService` 会生成包含以下 Claim 的 JWT Token：

```csharp
var claims = new List<Claim>
{
    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),           // 用户 ID
    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),       // 用户名（唯一）
    new Claim(JwtRegisteredClaimNames.Name, user.UserName),             // 用户名
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),           // 用户 ID（声明类型）
    new Claim(ClaimTypes.Name, user.UserName)                           // 用户名（声明类型）
};

// 如果有真实姓名
if (!string.IsNullOrEmpty(user.RealName))
{
    claims.Add(new Claim(ClaimTypes.GivenName, user.RealName));         // 真实姓名
}
```

##### Token 使用流程

1. **登录获取 Token**：调用 `LoginAsync` 接口，返回包含 Token 的响应
2. **携带 Token 访问**：在 HTTP 请求头中添加 `Authorization: Bearer <token>`
3. **服务端验证**：JWT 认证中间件验证 Token 有效性
4. **获取用户信息**：通过 `CurrentUser` 服务获取当前用户信息

#### 注意事项

1. **中间件执行顺序**：权限检查中间件必须在 `UseRouting` 之后，以便能够获取 Endpoint 元数据
2. **Claim 类型一致性**：`CurrentUser` 读取的 Claim 类型必须与 `LoginService` 生成的 Claim 一致
3. **[Authorize] 注解继承**：在控制器级别添加 `[Authorize]` 后，所有方法都需要登录；可以在方法级别使用 `[AllowAnonymous]` 允许匿名访问
4. **Token 过期处理**：Token 过期后访问受保护 API 会返回 401，前端应引导用户重新登录
5. **日志记录**：中间件会记录权限检查相关的日志，包括控制器名、方法名和检查结果

#### 调试建议

启用调试日志可以查看权限检查中间件的详细信息：

```json
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "DDDProject.API.Middlewares.PermissionCheckMiddleware": "Debug"
    }
  }
}
```

日志输出示例：
```
Debug: 控制器: DDDProject.API.Controllers.PublicController, 方法: GetPublicData, 有 Authorize 注解: False
Debug: 无 Authorize 注解，跳过权限检查

Debug: 控制器: DDDProject.API.Controllers.ProtectedController, 方法: GetUserData, 有 Authorize 注解: True
Debug: 权限检查通过: 控制器: DDDProject.API.Controllers.ProtectedController, 方法: GetUserData

Warning: 未授权访问: 控制器 DDDProject.API.Controllers.ProtectedController, 方法 GetUserData
```

### 密码加密传输说明

#### 概述

本项目采用 AES-256-CBC 算法对用户密码进行加密传输，确保密码在传输过程中不以明文形式暴露。

#### 加密原理

- **加密算法**: AES-256-CBC
- **密钥**: `DDDProject2024SecretKey!` (前后端一致)
- **初始化向量 (IV)**: `DDDProject2024IV!` (前后端一致)
- **填充模式**: PKCS7

#### 加密流程

1. **前端加密**: 用户输入密码后，使用 AES 算法加密，然后发送给后端
2. **后端解密**: 后端接收到加密密码后，使用相同的密钥和 IV 解密
3. **密码验证**: 解密后对明文密码进行 SHA256 哈希，与数据库中的哈希值比对

#### 前端实现

##### 加密工具

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

##### 登录页面使用

**文件位置**: `DDDVue/src/views/Login.vue`

```typescript
import { aesEncrypt } from '../utils/crypto'

// 加密密码后发送
const response = await http.post<LoginResponse>(api.Login.Login, {
  userName: form.value.userName,
  password: aesEncrypt(form.value.password)  // 密码加密传输
})
```

##### 安装依赖

```bash
# 安装 crypto-js
npm install crypto-js

# 安装 TypeScript 类型定义
npm install --save-dev @types/crypto-js
```

#### 后端实现

##### 密码帮助类

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

##### 登录服务使用

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

#### 安全性说明

1. **传输安全**: 密码在前端加密后以密文形式传输，即使被截获也无法直接获取明文密码
2. **密钥管理**: 当前密钥硬编码在代码中，**生产环境应从配置文件或环境变量读取**
3. **双重保护**: 即使加密被破解，密码仍经过 SHA256 哈希存储，进一步保护用户密码
4. **兼容性**: 后端解密失败时会兼容处理，确保平滑过渡

#### 生产环境建议

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

#### 注意事项

1. **密钥一致性**: 前后端密钥和 IV 必须完全一致，否则无法解密
2. **不要提交密钥**: 如果将密钥移到配置文件，确保 `.gitignore` 中排除包含密钥的配置文件
3. **错误处理**: 解密失败时会返回原值，确保有适当的错误处理机制
4. **性能考虑**: AES 加密解密性能良好，对用户体验无明显影响

### 时区设置说明

#### 中国标准时间（UTC+8）

本项目所有时间相关操作都使用**中国标准时间**（CST，UTC+8）以确保时间显示的一致性。

#### 实现方式

1. **后端配置**：
   - 实体创建时间使用 `DateTime.Now` 替代 `DateTime.UtcNow`
   - 数据库存储使用服务器本地时间（配置为 CST）
   - 时间服务统一处理中国标准时间

2. **数据库配置**：
   - 通过 `GETDATE()` 函数使用服务器本地时间
   - 不使用 UTC 时间函数

3. **部署要求**：
   - 生产服务器应设置时区为 **(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi**
   - Docker 容器应设置 `Asia/Shanghai` 时区

#### 时间服务（ITimeService）

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

#### 部署配置

##### Windows 服务器

```cmd
# 设置系统时区为北京时间
tzutil /s "China Standard Time"
```

##### Linux 服务器（Docker 环境）

```bash
# 设置时区为上海
sudo timedatectl set-timezone Asia/Shanghai
```

##### Dockerfile 时区配置

```dockerfile
# Ubuntu/Debian
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN dpkg-reconfigure -f noninteractive tzdata

# 或者设置环境变量
ENV TZ=Asia/Shanghai
RUN echo $TZ > /etc/timezone
```

#### 注意事项

1. **数据库服务器时间**：确保数据库服务器时间设置为 CST
2. **应用程序服务器时间**：确保应用服务器时间设置为 CST
3. **客户端时间**：前端使用的时间戳基于服务端时间，保证一致性
4. **日志记录**：所有日志时间均记录为 CST 时间
5. **用户界面**：所有显示时间都来自服务端，确保统一性

### 完整示例：添加新的 API 功能

#### 1. 创建接口和 Service

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
        await _userRepository.AddAsync(user);
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

#### 2. 在控制器中使用

```csharp
using Microsoft.AspNetCore.Mvc;
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;

namespace DDDProject.API.Controllers;

/// <summary>
/// 用户数据控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class UserDataController : BaseApiController
{
    private readonly IUserDataService _userDataService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userDataService">用户数据服务</param>
    public UserDataController(IUserDataService userDataService)
    {
        _userDataService = userDataService;
    }

    /// <summary>
    /// 获取用户ById
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户数据</returns>
    [HttpGet("{id}")]
    [ActionName("GetUserByIdAsync")]
    [ApiSearch(Name = "获取用户ById", Category = "用户")]
    public async Task<ApiRequestResult> GetUserByIdAsync(Guid id)
    {
        return await _userDataService.GetUserByIdAsync(id);
    }

    /// <summary>
    /// 获取所有用户
    /// </summary>
    /// <returns>用户列表</returns>
    [HttpGet]
    [ActionName("GetAllUsersAsync")]
    [ApiSearch(Name = "获取所有用户", Category = "用户")]
    public async Task<ApiRequestResult> GetAllUsersAsync()
    {
        return await _userDataService.GetAllUsersAsync();
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="user">用户数据</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    [ActionName("CreateUserAsync")]
    [ApiSearch(Name = "创建用户", Category = "用户")]
    public async Task<ApiRequestResult> CreateUserAsync([FromBody] UserData user)
    {
        return await _userDataService.CreateUserAsync(user);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="user">用户数据</param>
    /// <returns>更新结果</returns>
    [HttpPut]
    [ActionName("UpdateUserAsync")]
    [ApiSearch(Name = "更新用户", Category = "用户")]
    public async Task<ApiRequestResult> UpdateUserAsync([FromBody] UserData user)
    {
        return await _userDataService.UpdateUserAsync(user);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    [ActionName("DeleteUserAsync")]
    [ApiSearch(Name = "删除用户", Category = "用户")]
    public async Task<IActionResult> DeleteUserAsync(Guid id)
    {
        var result = await _userDataService.DeleteUserAsync(id);
        if (!result.Success)
            return BadRequest(result);
        return NoContent();
    }

    /// <summary>
    /// 获取用户数据
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户数据</returns>
    [HttpGet]
    [ActionName("GetUserDataAsync")]
    [ApiSearch(Name = "获取用户数据", Category = "用户")]
    public async Task<ApiRequestResult> GetUserDataAsync(Guid userId)
    {
        return await _userDataService.GetUserDataAsync(userId);
    }
}
```

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

### Vue 前端路由规范

#### 概述

本项目采用动态路由配置机制，路由数据从后端 API 动态获取，确保前后端路由一致性。路由配置遵循扁平化结构，所有路由均为单层路由，不存在嵌套路由。

#### 路由配置文件

**主要文件**：
- `DDDVue/src/router/index.ts`: 路由主配置文件
- `DDDVue/src/utils/routeGenerator.ts`: 路由存储工具函数
- `DDDVue/src/views/layout/Layout.vue`: 布局组件（包含侧边栏菜单）
- `DDDVue/src/api/menu.ts`: 菜单 API 接口

#### 路由数据来源

路由数据从后端 API `http://localhost:5272/api/Menu/GetSidebarMenusAsync` 动态获取：

```typescript
// router/index.ts
const createDynamicRouter = async () => {
  // 从 localStorage 获取已缓存的路由
  let cachedRoutes = getRoutesFromStorage()
  
  // 尝试从 API 获取路由配置
  try {
    const response = await menuApi.getSidebarMenuTree()
    if (response.data && Array.isArray(response.data)) {
      routesFromApi = convertMenuToRoutes(response.data)
      // 只有当 API 返回有效路由时才保存
      if (routesFromApi.length > 0) {
        saveRoutesToStorage(routesFromApi)
        cachedRoutes = routesFromApi
      }
    }
  } catch (error) {
    console.error('从 API 获取路由失败，使用缓存路由:', error)
  }
  
  // 创建路由实例
  const router = createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/', name: 'login', component: Login },
      {
        path: '/',
        name: 'layout',
        component: Layout,
        children: [
          ...cachedRoutes,  // 动态路由
          { path: 'profile', name: 'profile', component: Profile },
          { path: 'clear-cache', name: 'clear-cache', component: ClearCache },
          { path: ':pathMatch(.*)*', name: 'not-found', component: NotFound }
        ]
      }
    ]
  })
  
  return router
}
```

#### 路由结构规范

**1. 扁平化单层路由**

所有路由均为单层结构，不存在嵌套路由：

```typescript
// ✅ 正确：单层路由
{
  path: 'dashboard',
  name: 'dashboard',
  component: () => import('../views/home/Dashboard/Dashboard.vue')
}

// ❌ 错误：嵌套路由（禁止使用）
{
  path: 'settings',
  component: Settings,
  children: [
    {
      path: 'menus',
      component: SettingsMenus
    }
  ]
}
```

**2. 路由路径与页面一一对应**

每个路由路径必须对应一个实际的 Vue 页面组件：

```typescript
// 路由路径: /dashboard
// 对应页面: DDDVue/src/views/home/Dashboard/Dashboard.vue

// 路由路径: /users
// 对应页面: DDDVue/src/views/home/Users/Users.vue

// 路由路径: /menu
// 对应页面: DDDViews/home/menu/Menu.vue
```

**3. 路由配置接口**

```typescript
interface RouteConfig {
  path: string          // 路由路径（必须唯一）
  name: string          // 路由名称
  component: string     // 组件路径（相对于 views/home/）
  icon?: string         // 菜单图标名称
  parentId?: string | number  // 父菜单 ID（用于菜单树）
  sortOrder?: number    // 排序序号
  status?: number       // 状态（0: 禁用, 1: 启用）
}
```

#### 路由存储机制

**localStorage 键名统一**：

- **菜单和路由数据**: `sidebarMenu` - 存储侧边栏菜单和路由配置

```typescript
// utils/routeGenerator.ts
export const getRoutesFromStorage = (): MenuItem[] => {
  try {
    const storedMenu = localStorage.getItem('sidebarMenu')
    if (storedMenu) {
      return JSON.parse(storedMenu)
    }
  } catch (e) {
    console.error('解析菜单数据失败', e)
  }
  return []
}

export const saveRoutesToStorage = (menus: MenuItem[]): void => {
  try {
    localStorage.setItem('sidebarMenu', JSON.stringify(menus))
  } catch (e) {
    console.error('保存菜单数据失败', e)
  }
}
```

#### 路径映射规则

**组件路径自动映射**：

后端返回的 `component` 字段值会自动拼接为完整路径：

```typescript
// 后端返回
{
  path: 'dashboard',
  component: 'Dashboard/Dashboard'  // 相对于 views/home/
}

// 实际加载路径
import('../views/home/Dashboard/Dashboard.vue')
```

**页面文件命名规范**：

```
views/home/
├── Dashboard/
│   └── Dashboard.vue          # 路由路径: /dashboard
├── Users/
│   └── Users.vue              # 路由路径: /users
├── Products/
│   └── Products.vue           # 路由路径: /products
├── Settings/
│   ├── Settings.vue           # 路由路径: /settings
│   └── Permissions.vue        # 路由路径: /settings-permissions
├── menu/
│   └── Menu.vue               # 路由路径: /menu
├── Profile/
│   └── Profile.vue            # 路由路径: /profile
└── ClearCache/
    └── ClearCache.vue         # 路由路径: /clear-cache
```

#### 路由更新机制

**更新路由方法**：

```typescript
// router/index.ts
export const updateRoutes = async () => {
  try {
    // 从 API 获取最新路由配置
    const response = await menuApi.getSidebarMenuTree()
    if (response.data && Array.isArray(response.data)) {
      const newRoutes = convertMenuToRoutes(response.data)
      saveRoutesToStorage(newRoutes)
      return newRoutes
    }
    return getRoutesFromStorage()
  } catch (error) {
    console.error('更新路由失败:', error)
    return getRoutesFromStorage()
  }
}
```

**更新路由注意事项**：

1. **页面组件独立性**：
   - 所有菜单和路由数据统一使用 `sidebarMenu` 键存储
   - 修改 menu 页不会影响其他页面
   - 公共组件除外

2. **刷新页面应用新路由**：
   ```typescript
   // menu 页更新菜单后刷新路由
   const refreshSidebarMenu = async () => {
     try {
       localStorage.removeItem('sidebarMenu')
       window.location.reload()  // 重新加载页面以应用新路由
     } catch (error) {
       console.error('刷新侧边栏菜单失败:', error)
     }
   }
   ```

#### 菜单与路由关系

**菜单数据结构**（用于侧边栏显示）：

```typescript
interface MenuItem {
  id?: string | number
  path: string        // 路由路径
  name: string        // 菜单名称
  icon?: string       // 图标
  parentId?: string | number
  children?: MenuItem[]  // 子菜单（仅用于显示，不创建嵌套路由）
}
```

**路由数据结构**（用于路由匹配）：

```typescript
interface RouteConfig {
  path: string        // 路由路径（扁平化）
  name: string
  component: string   // 组件路径
  icon?: string
  parentId?: string | number
  sortOrder?: number
  status?: number
}
```

**关键区别**：

| 特性 | 菜单/路由数据 |
|-----|--------------|
| 存储键 | `sidebarMenu` |
| 结构 | 树形（可嵌套） |
| 用途 | 侧边栏菜单显示和路由匹配 |
| 嵌套 | 支持 `children` |

#### 固定路由

以下路由为固定路由，不从 API 动态获取：

```typescript
{
  path: 'profile',
  name: 'profile',
  component: () => import('../views/home/Profile/Profile.vue')
},
{
  path: 'clear-cache',
  name: 'clear-cache',
  component: ClearCache
},
{
  path: ':pathMatch(.*)*',
  name: 'not-found',
  component: NotFound
}
```

#### 导航守卫

```typescript
// router/index.ts
router.beforeEach(async (to, from) => {
  const token = localStorage.getItem('token')

  // 访问登录页
  if (to.path === '/') {
    if (token) {
      return '/dashboard'  // 已登录跳转到仪表盘
    } else {
      return true  // 未登录允许访问
    }
  }
  // 访问受保护页面
  else {
    if (!token) {
      return '/'  // 未登录跳转到登录页
    } else {
      return true  // 已登录允许访问
    }
  }
})
```

#### 开发规范

**1. 添加新页面步骤**：

1. 在 `DDDVue/src/views/home/` 下创建页面组件
2. 在后端数据库的 `Menu` 表中添加菜单记录：
   - `Path`: 路由路径（如 `dashboard`）
   - `Component`: 组件路径（如 `Dashboard/Dashboard`）
   - `Name`: 菜单名称
   - `Icon`: 图标名称（可选）
   - `ParentId`: 父菜单 ID（可选）
   - `SortOrder`: 排序序号
   - `Status`: 状态（1: 启用, 0: 禁用）
3. 清除浏览器 localStorage 或刷新页面

**2. 路径命名规范**：

- 使用短横线命名法：`settings-menus`（不要用 `/settings/menus`）
- 保持路径简洁：`users`（不要用 `user-management`）
- 与页面路径一致：`dashboard` → `Dashboard/Dashboard.vue`

**3. 组件独立性原则**：

- 所有菜单和路由数据统一使用 `sidebarMenu` 键存储
- 修改 menu 页不会影响其他页面
- 公共组件除外（如 `routeGenerator.ts`）

**4. 错误处理**：

```typescript
// 路由加载失败时的处理
const convertMenuToRoutes = (menus: RouteConfig[]): RouteConfig[] => {
  return menus.map(menu => {
    try {
      return {
        path: menu.path,
        name: menu.name,
        component: () => import(`../views/home/${menu.component}.vue`),
        icon: menu.icon,
        parentId: menu.parentId,
        sortOrder: menu.sortOrder,
        status: menu.status
      }
    } catch (error) {
      console.error(`加载组件失败: ${menu.component}`, error)
      return null
    }
  }).filter(route => route !== null)
}
```

#### 常见问题

**Q1: 路由不生效怎么办？**

A: 检查以下几点：
1. 后端 API `GetSidebarMenusAsync` 是否返回正确数据
2. `component` 字段路径是否正确
3. 页面组件是否存在
4. localStorage 中的路由数据是否正确

**Q2: 如何调试路由？**

A: 在浏览器控制台查看：
```javascript
// 查看菜单和路由数据
localStorage.getItem('sidebarMenu')
// 清除缓存
localStorage.removeItem('sidebarMenu')
```

**Q4: 如何添加子菜单？**

A: 子菜单在后端创建时设置 `ParentId` 指向父菜单 ID，前端会自动构建树形菜单显示，但路由仍然是扁平化的。

#### 总结

本项目的路由规范核心要点：

1. ✅ **动态路由**: 从 API 动态获取路由配置
2. ✅ **扁平化**: 所有路由均为单层，无嵌套路由
3. ✅ **一一对应**: 路由路径与页面组件一一对应
4. ✅ **存储统一**: 菜单和路由数据使用同一个 localStorage 键 `sidebarMenu`
5. ✅ **独立性**: 页面组件独立，互不影响
6. ✅ **固定路由**: profile、clear-cache、404 等为固定路由

### 后端 API 开发规范

#### 概述

本项目采用 DDD（Domain-Driven Design）分层架构设计，包含以下四层：

- **Domain 层**: 领域层，包含核心业务逻辑和实体
- **Application 层**: 应用层，协调领域对象，处理用例
- **Infrastructure 层**: 基础设施层，实现抽象接口，提供技术细节
- **API 层**: API 层，处理 HTTP 请求，接收参数，返回响应

#### 添加新 API 功能步骤

**1. 创建实体（Domain 层）**

在 `DDDProject.Domain/Entities/` 目录下创建实体类：

```csharp
using DDDProject.Domain.Entities;

namespace DDDProject.Domain.Entities;

/// <summary>
/// 实体描述
/// </summary>
public class EntityName : AggregateRoot
{
    /// <summary>
    /// 属性1
    /// </summary>
    public string Property1 { get; private set; } = string.Empty;

    /// <summary>
    /// 属性2
    /// </summary>
    public int Property2 { get; private set; }

    /// <summary>
    /// 构造函数（私有，用于ORM）
    /// </summary>
    protected EntityName() { }

    /// <summary>
    /// 创建实体
    /// </summary>
    public static EntityName Create(string property1, int property2)
    {
        var entity = new EntityName
        {
            Id = Guid.NewGuid(),
            Property1 = property1,
            Property2 = property2,
            CreatedAt = DateTime.Now  // 使用本地时间（中国标准时间）
        };

        return entity;
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    public void Update(string? property1 = null, int? property2 = null)
    {
        if (!string.IsNullOrEmpty(property1))
            Property1 = property1;

        if (property2.HasValue)
            Property2 = property2;

        UpdatedAt = DateTime.Now; // 使用本地时间（中国标准时间）
    }
}
```

**2. 创建仓储接口（Domain 层）**

在 `DDDProject.Domain/Repositories/` 目录下创建接口：

```csharp
using DDDProject.Domain.Entities;

namespace DDDProject.Domain.Repositories;

/// <summary>
/// 仓储接口
/// </summary>
public interface IEntityNameRepository : IRepository<EntityName>
{
    // 自定义方法
}
```

**3. 创建仓储实现（Infrastructure 层）**

在 `DDDProject.Infrastructure/Repositories/` 目录下创建实现类：

```csharp
using DDDProject.Domain.Repositories;
using DDDProject.Domain.Entities;
using DDDProject.Infrastructure.Contexts;

namespace DDDProject.Infrastructure.Repositories;

/// <summary>
/// 仓储实现
/// </summary>
public class EntityNameRepository : Repository<EntityName>, IEntityNameRepository
{
    public EntityNameRepository(ApplicationDbContext context) : base(context)
    {
    }
}
```

**4. 创建 DTO（Application 层）**

在 `DDDProject.Application/DTOs/` 目录下创建数据传输对象：

```csharp
namespace DDDProject.Application.DTOs;

/// <summary>
/// DTO 描述
/// </summary>
public class EntityNameDto
{
    public Guid Id { get; set; }
    public string Property1 { get; set; } = string.Empty;
    public int Property2 { get; set; }
}
```

**5. 创建应用服务接口（Application 层）**

在 `DDDProject.Application/Interfaces/` 目录下创建接口：

```csharp
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 应用服务接口
/// </summary>
public interface IEntityNameService : IApplicationService
{
    Task<ApiRequestResult> GetEntityNamesAsync(PagedRequest request);
    Task<ApiRequestResult> GetEntityNameByIdAsync(Guid id);
    Task<ApiRequestResult> CreateEntityNameAsync(EntityNameDto dto);
    Task<ApiRequestResult> UpdateEntityNameAsync(EntityNameDto dto);
    Task<ApiRequestResult> DeleteEntityNameAsync(Guid id);
}
```

**6. 创建应用服务实现（Application 层）**

在 `DDDProject.Application/Services/` 目录下创建实现类：

```csharp
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Repositories;

namespace DDDProject.Application.Services;

/// <summary>
/// 应用服务实现
/// </summary>
public class EntityNameService : IEntityNameService
{
    private readonly IRepository<EntityName> _repository;

    public EntityNameService(IRepository<EntityName> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 获取实体列表（分页）
    /// </summary>
    public async Task<ApiRequestResult> GetEntityNamesAsync(PagedRequest request)
    {
        try
        {
            var skipCount = (request.PageNumber - 1) * request.PageSize;
            var total = await _repository.CountAsync(m => true);
            var entities = await _repository.GetListAsync(
                m => true,
                q => q.OrderBy(m => m.Id),
                skipCount,
                request.PageSize
            );

            var dtos = entities.Select(e => new EntityNameDto
            {
                Id = e.Id,
                Property1 = e.Property1,
                Property2 = e.Property2
            }).ToList();

            var pagedResult = new PagedResult<EntityNameDto>
            {
                List = dtos,
                Total = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return new ApiRequestResult
            {
                Success = true,
                Message = "操作成功",
                Data = pagedResult
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取列表失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 获取实体详情
    /// </summary>
    public async Task<ApiRequestResult> GetEntityNameByIdAsync(Guid id)
    {
        try
        {
            var entity = await _repository.FindAsync(id);

            if (entity == null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "实体不存在",
                    Data = null
                };
            }

            var dto = new EntityNameDto
            {
                Id = entity.Id,
                Property1 = entity.Property1,
                Property2 = entity.Property2
            };

            return new ApiRequestResult
            {
                Success = true,
                Message = "操作成功",
                Data = dto
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"获取详情失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 创建实体
    /// </summary>
    public async Task<ApiRequestResult> CreateEntityNameAsync(EntityNameDto dto)
    {
        try
        {
            var entity = EntityName.Create(dto.Property1, dto.Property2);

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "创建成功",
                Data = entity.Id
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"创建失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    public async Task<ApiRequestResult> UpdateEntityNameAsync(EntityNameDto dto)
    {
        try
        {
            var existing = await _repository.FindAsync(dto.Id);
            if (existing == null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "实体不存在",
                    Data = null
                };
            }

            existing.Update(dto.Property1, dto.Property2);
            _repository.Update(existing);
            await _repository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "更新成功",
                Data = existing.Id
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"更新失败: {ex.Message}",
                Data = null
            };
        }
    }

    /// <summary>
    /// 删除实体
    /// </summary>
    public async Task<ApiRequestResult> DeleteEntityNameAsync(Guid id)
    {
        try
        {
            var entity = await _repository.FindAsync(id);
            if (entity == null)
            {
                return new ApiRequestResult
                {
                    Success = false,
                    Message = "实体不存在",
                    Data = null
                };
            }

            _repository.Remove(entity);
            await _repository.SaveChangesAsync();

            return new ApiRequestResult
            {
                Success = true,
                Message = "删除成功",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new ApiRequestResult
            {
                Success = false,
                Message = $"删除失败: {ex.Message}",
                Data = null
            };
        }
    }
}
```

**7. 创建控制器（API 层）**

在 `DDDProject.API/Controllers/` 目录下创建控制器：

```csharp
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DDDProject.API.Controllers;

/// <summary>
/// 控制器描述
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize] // 需要身份验证
public class EntityNameController : BaseApiController
{
    private readonly IEntityNameService _service;

    public EntityNameController(IEntityNameService service)
    {
        _service = service;
    }

    /// <summary>
    /// 获取实体列表（分页）
    /// </summary>
    [HttpGet]
    [ActionName("GetEntityNamesAsync")]
    [ApiSearch(Name = "获取实体列表", Description = "返回实体列表（支持分页）", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> GetEntityNamesAsync([FromQuery] int pageNum = 1, [FromQuery] int pageSize = 10)
    {
        var request = new PagedRequest
        {
            PageNumber = pageNum,
            PageSize = pageSize
        };
        return await _service.GetEntityNamesAsync(request);
    }

    /// <summary>
    /// 获取实体详情
    /// </summary>
    [HttpGet]
    [ActionName("GetEntityNameByIdAsync")]
    [ApiSearch(Name = "获取实体详情", Description = "根据ID获取实体详细信息", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> GetEntityNameByIdAsync([FromQuery] Guid id)
    {
        return await _service.GetEntityNameByIdAsync(id);
    }

    /// <summary>
    /// 创建实体
    /// </summary>
    [HttpPost]
    [ActionName("CreateEntityNameAsync")]
    [ApiSearch(Name = "创建实体", Description = "创建新的实体", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> CreateEntityNameAsync([FromBody] EntityNameDto dto)
    {
        return await _service.CreateEntityNameAsync(dto);
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    [HttpPut]
    [ActionName("UpdateEntityNameAsync")]
    [ApiSearch(Name = "更新实体", Description = "更新现有实体", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> UpdateEntityNameAsync([FromBody] EntityNameDto dto)
    {
        return await _service.UpdateEntityNameAsync(dto);
    }

    /// <summary>
    /// 删除实体
    /// </summary>
    [HttpDelete]
    [ActionName("DeleteEntityNameAsync")]
    [ApiSearch(Name = "删除实体", Description = "根据ID删除实体", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> DeleteEntityNameAsync([FromQuery] Guid id)
    {
        return await _service.DeleteEntityNameAsync(id);
    }
}
```

**8. 添加数据库迁移**

```bash
# 进入项目目录
cd DDDProject

# 添加迁移
dotnet ef migrations add CreateEntityName --project DDDProject.Infrastructure --startup-project DDDProject.API

# 更新数据库
dotnet ef database update --project DDDProject.Infrastructure --startup-project DDDProject.API
```

#### 路由配置 API 开发示例

**1. 创建路由配置模型（Domain 层）**

在 `DDDProject.Domain/Models/` 目录下创建 `RouteConfig.cs`：

```csharp
namespace DDDProject.Domain.Models;

/// <summary>
/// 路由配置模型
/// </summary>
public class RouteConfig
{
    /// <summary>
    /// 路由路径
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 路由名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 组件路径
    /// </summary>
    public string Component { get; set; } = string.Empty;

    /// <summary>
    /// 图标名称（可选）
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 父级菜单ID（可选）
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 排序号（可选）
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 菜单状态：0-禁用，1-启用（可选）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 子菜单列表（可选）
    /// </summary>
    public List<RouteConfig>? Children { get; set; }
}
```

**2. 在菜单服务接口中添加方法（Application 层）**

在 `DDDProject.Application/Interfaces/IMenuService.cs` 中添加：

```csharp
/// <summary>
/// 获取路由配置（用于前端动态路由）
/// </summary>
Task<ApiRequestResult> GetRoutesAsync();
```

**3. 在菜单服务实现中添加方法（Application 层）**

在 `DDDProject.Application/Services/MenuService.cs` 中添加：

```csharp
/// <summary>
/// 获取路由配置（用于前端动态路由）
/// </summary>
public async Task<ApiRequestResult> GetRoutesAsync()
{
    try
    {
        // 获取所有菜单
        var menus = await _menuRepository.GetListAsync(m => true);
        
        // 构建路由配置列表
        var routeConfigs = BuildRouteConfigs(menus.ToList());

        return new ApiRequestResult
        {
            Success = true,
            Message = "操作成功",
            Data = routeConfigs
        };
    }
    catch (Exception ex)
    {
        return new ApiRequestResult
        {
            Success = false,
            Message = $"获取路由配置失败: {ex.Message}",
            Data = null
        };
    }
}

/// <summary>
/// 构建路由配置列表
/// </summary>
private List<RouteConfig> BuildRouteConfigs(List<Menu> allMenus)
{
    // 获取根节点
    var rootMenus = allMenus.Where(m => m.ParentId == null || m.ParentId == Guid.Empty)
                           .OrderBy(m => m.SortOrder)
                           .ToList();

    var result = new List<RouteConfig>();

    foreach (var menu in rootMenus)
    {
        var routeConfig = BuildRouteConfig(menu, allMenus);
        result.Add(routeConfig);
    }

    return result;
}

/// <summary>
/// 递归构建路由配置
/// </summary>
private RouteConfig BuildRouteConfig(Menu menu, List<Menu> allMenus)
{
    var children = allMenus.Where(m => m.ParentId == menu.Id).OrderBy(m => m.SortOrder).ToList();

    return new RouteConfig
    {
        Path = menu.Path,
        Name = menu.Name,
        Component = menu.Component,
        Icon = menu.Icon,
        ParentId = menu.ParentId,
        SortOrder = menu.SortOrder,
        Status = menu.Status,
        Children = children.Any() ? children.Select(m => BuildRouteConfig(m, allMenus)).ToList() : null
    };
}
```

**4. 在菜单控制器中添加 API 端点（API 层）**

在 `DDDProject.API/Controllers/MenuController.cs` 中添加：

```csharp
/// <summary>
/// 获取路由配置（用于前端动态路由）
/// </summary>
[HttpGet]
[ActionName("GetRoutesAsync")]
[ApiSearch(Name = "获取路由配置", Description = "返回路由配置列表，用于前端动态路由", Category = ApiSearchCategory.Menu)]
public async Task<ApiRequestResult> GetRoutesAsync()
{
    return await _menuService.GetRoutesAsync();
}
```

**5. 前端调用示例**

```typescript
// api/menu.ts
export const getRoutes = () => {
  return http.get<RouteConfig[]>(api.Menu.GetRoutesAsync)
}

// router/index.ts
import * as menuApi from '@/api/menu'

const createDynamicRouter = async () => {
  try {
    const response = await menuApi.getRoutes()
    if (response.data && Array.isArray(response.data)) {
      // 使用路由配置
      const routes = response.data
    }
  } catch (error) {
    console.error('获取路由配置失败:', error)
  }
}
```

#### 注意事项

- ✅ 所有数据库操作必须使用异步方法（`ToListAsync()`、`FirstOrDefaultAsync()` 等）
- ✅ 控制器方法必须使用 `[ActionName]` 注解
- ✅ 实体属性使用私有 setter，通过方法修改
- ✅ 使用 `DateTime.Now` 代替 `DateTime.UtcNow`（中国标准时间）
- ✅ 路由配置 API 返回树形结构数据
- ✅ 添加新功能后需要添加数据库迁移并更新数据库
- ⚠️ 修改模型后需要添加迁移并更新数据库

## 注意事项

- 遵循 DDD 设计原则
- 领域层不应依赖其他层
- 应用层协调领域层完成业务操作
- 基础设施层实现抽象接口
- API 层仅处理 HTTP 相关注
- 数据库连接字符串已配置，如需修改请更新 `DDDProject/API/appsettings.json`
- 前端 API 基础地址配置在 `DDDVue/src/utils/http.ts`

## 前端 localStorage 分类管理

### 概述

本项目已实现统一的 localStorage 分类管理工具，将缓存数据按功能分类，便于管理和清除。

### 分类说明

#### 1. Login（登录缓存）
存储与用户登录相关的信息：
- `token`: JWT 认证令牌
- `userInfo`: 用户基本信息（userId, userName, realName）

#### 2. Menu（菜单缓存）
存储与菜单相关的信息：
- `sidebarMenu`: 侧边栏菜单数据（从 API 获取的菜单树）

#### 3. List（列表缓存）
预留分类，用于存储列表数据缓存（当前未使用）

#### 4. All（全部缓存）
包含以上所有分类的缓存数据

### 使用方法

#### 导入工具

```typescript
import { getItem, setItem, removeItem, clearByCategory, StorageKeys, getStorageStats } from '@/utils/storage'
```

#### 基本操作

```typescript
// 获取数据
const token = getItem<string>(StorageKeys.Token)
const userInfo = getItem<UserInfo>(StorageKeys.UserInfo)

// 设置数据
setItem(StorageKeys.Token, token)
setItem(StorageKeys.UserInfo, userInfo)

// 移除数据
removeItem(StorageKeys.Token)
```

#### 分类清除

```typescript
// 清除登录缓存
clearByCategory('Login')

// 清除菜单缓存
clearByCategory('Menu')

// 清除列表缓存
clearByCategory('List')

// 清除全部缓存
clearByCategory('All')
```

#### 获取缓存统计

```typescript
// 获取各分类的缓存项数
const stats = getStorageStats()
console.log(stats['Login'])  // 登录缓存项数
console.log(stats['Menu'])   // 菜单缓存项数
```

### 更新的文件

#### 新建文件
- `src/utils/storage.ts`: localStorage 分类管理工具

#### 修改文件
- `src/views/home/ClearCache/ClearCache.vue`: 更新清除缓存页面，支持分类清除
- `src/views/layout/Layout.vue`: 使用新的 storage 工具
- `src/views/Login.vue`: 使用新的 storage 工具
- `src/views/home/Profile/Profile.vue`: 使用新的 storage 工具
- `src/views/home/menu/Menu.vue`: 使用新的 storage 工具
- `src/router/index.ts`: 使用新的 storage 工具
- `src/router/detail.ts`: 使用新的 storage 工具
- `src/utils/http.ts`: 使用新的 storage 工具
- `src/utils/routeGenerator.ts`: 使用新的 storage 工具

### 优势

1. **统一管理**: 所有 localStorage 操作通过统一的工具函数进行
2. **分类清晰**: 按功能分类存储，便于管理和清除
3. **类型安全**: 使用 TypeScript 类型定义，提供更好的开发体验
4. **易于维护**: 修改存储键名时只需在 `StorageKeys` 中修改一次
5. **错误处理**: 工具函数内部已包含错误处理，无需在业务代码中重复处理

### 注意事项

1. 所有数据存储都使用 `JSON.stringify` 和 `JSON.parse` 进行序列化
2. 获取数据时建议使用泛型指定类型：`getItem<T>(key)`
3. 清除缓存后可能需要刷新页面或重新加载数据
4. 在清除登录缓存后，通常需要跳转到登录页
