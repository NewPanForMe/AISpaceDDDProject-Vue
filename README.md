# DDDNewProject - DDD 分层架构全栈项目

本项目采用 DDD（Domain-Driven Design）分层架构设计，包含后端 API（DDDProject）和前端 Vue（DDDVue）两个子项目。

## 文档更新日志

文档做更新时，应该在此留下记录。

<table>
  <thead>
    <tr>
      <th>日期</th>
      <th>更新内容</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td rowspan="7">2026-04-01</td>
      <td>新增自定义权限注解 <code>[Permission]</code>，支持方法级别的权限控制</td>
    </tr>
    <tr>
      <td>为所有控制器的数据变更方法添加 <code>[Permission]</code> 注解</td>
    </tr>
    <tr>
      <td>将权限注解规范记录到 README.md 开发守则</td>
    </tr>
    <tr>
      <td rowspan="6">2026-03-31</td>
      <td>新增按钮管理模块（Button），支持页面按钮的统一管理</td>
    </tr>
    <tr>
      <td>新增按钮权限设计规范，页面按钮显示由按钮数据决定</td>
    </tr>
    <tr>
      <td>修改分配权限页面，数据来源改为按钮信息，按菜单分组显示</td>
    </tr>
    <tr>
      <td>修改所有列表页按钮，使用 `hasBtn()` 替代 `hasPermission()` 进行权限控制</td>
    </tr>
    <tr>
      <td>新增 `useButtons` 组合式函数，简化按钮权限管理</td>
    </tr>
    <tr>
      <td>新增按钮种子数据生成器，根据菜单自动生成按钮</td>
    </tr>
    <tr>
      <td rowspan="10">2026-03-26</td>
      <td>新增权限管理模块（Permission），支持按钮级别的权限控制</td>
    </tr>
    <tr>
      <td>JWT 配置从配置文件改为从数据库 Settings 表获取，支持动态配置</td>
    </tr>
    <tr>
      <td>前端列表页添加缓存功能，统一缓存键格式为 `list_模块名_页码_每页数量`</td>
    </tr>
    <tr>
      <td>登录时检查用户角色状态，禁用角色禁止登录并给出提示</td>
    </tr>
    <tr>
      <td>用户列表添加重置密码按钮功能</td>
    </tr>
    <tr>
      <td>新增缓存更新规则文档，规范操作后更新缓存的流程</td>
    </tr>
    <tr>
      <td>修复登录时权限获取问题，从 PermissionDto[] 中正确提取权限编码</td>
    </tr>
    <tr>
      <td>权限管理页面添加按钮权限控制，模块筛选改为动态获取</td>
    </tr>
    <tr>
      <td>缓存管理页面表格添加操作列，支持单独删除各分类缓存</td>
    </tr>
    <tr>
      <td>完善个人中心页面：基本信息展示、编辑资料、修改密码功能</td>
    </tr>
    <tr>
      <td rowspan="3">2026-03-25</td>
      <td>新增 MenuRole 模块（菜单角色关联），完善 Role 模块功能，添加数据库迁移</td>
    </tr>
    <tr>
      <td>整理文档结构，将开发规范整合为统一章节，优化文档可读性</td>
    </tr>
    <tr>
      <td>新增 Git 拉推原则（Pull-Push Principle）说明</td>
    </tr>
  </tbody>
</table>

## 目录

- [快速开始](#快速开始)
- [项目结构](#项目结构)
- [技术栈](#技术栈)
- [开发规范](#开发规范)
  - [通用编码规范](#51-通用编码规范)
  - [后端开发规范](#52-后端开发规范)
  - [权限注解规范](#524-权限注解规范)
  - [前端开发规范](#53-前端开发规范)
  - [安全规范](#54-安全规范)
  - [配置规范](#55-配置规范)
  - [Git 规范](#56-git-规范)
- [完整示例](#完整示例添加新的-api-功能)
- [注意事项](#注意事项)
- [开发日志](#开发日志)

---

## 开发日志

### 2026-03-26

#### 新增功能

| 模块 | 功能 | 说明 |
|------|------|------|
| Permission | 权限管理模块 | 新增权限实体、角色权限关联，支持按钮级别权限控制 |
| Permission | 权限分配功能 | 角色管理页面新增分配权限按钮，支持按模块分组选择权限 |
| Permission | 权限控制功能 | 各页面按钮根据用户权限显示/隐藏 |
| Login | JWT 动态配置 | JWT 配置从配置文件改为从数据库 Settings 表获取 |
| Login | 登录角色状态检查 | 用户角色被禁用时禁止登录，并提示禁用的角色名称 |
| Users | 重置密码功能 | 用户列表操作栏添加重置密码按钮 |
| Storage | 列表缓存功能 | 用户、角色、菜单、权限列表支持缓存，优先从缓存获取数据 |
| Role | 删除前检查关联 | 删除角色前检查用户、菜单、权限关联，有关联时禁止删除 |
| ClearCache | 表格操作列 | 缓存统计表格添加删除操作列，支持单独删除各分类缓存 |
| Profile | 个人中心完善 | 完整的个人信息展示、编辑资料、修改密码功能 |

#### 新增文件

**Domain 层：**
- `Entities/Permission.cs` - 权限实体和角色权限关联实体

**Application 层：**
- `DTOs/PermissionDTO.cs` - 权限相关 DTO 和请求类
- `Interfaces/IPermissionService.cs` - 权限服务接口
- `Services/PermissionService.cs` - 权限服务实现

**Infrastructure 层：**
- `Configuration/PermissionConfiguration.cs` - 权限 EF Core 配置
- `Configuration/RolePermissionConfiguration.cs` - 角色权限关联 EF Core 配置

**前端：**
- `utils/permission.ts` - 权限工具函数

#### 数据库迁移

| 迁移名称 | 说明 |
|----------|------|
| `AddSettingsTable` | 创建系统设置表 |
| `AddPermissionTables` | 创建权限表和角色权限关联表 |

#### API 接口

**PermissionController：**
- `GET /api/Permission/GetPermissionsAsync` - 获取权限列表（分页）
- `GET /api/Permission/GetAllEnabledPermissionsAsync` - 获取所有启用的权限
- `GET /api/Permission/GetPermissionsByModuleAsync` - 按模块获取权限
- `GET /api/Permission/GetPermissionByIdAsync` - 获取权限详情
- `POST /api/Permission/CreatePermissionAsync` - 创建权限
- `PUT /api/Permission/UpdatePermissionAsync` - 更新权限
- `DELETE /api/Permission/DeletePermissionAsync` - 删除权限
- `POST /api/Permission/EnablePermissionAsync` - 启用权限
- `POST /api/Permission/DisablePermissionAsync` - 禁用权限
- `GET /api/Permission/GetRolePermissionIdsAsync` - 获取角色的权限ID列表
- `POST /api/Permission/AssignRolePermissionsAsync` - 为角色分配权限
- `GET /api/Permission/GetUserPermissionsAsync` - 获取用户权限列表
- `GET /api/Permission/HasPermissionAsync` - 检查用户是否有指定权限

**UserController 新增接口：**
- `PUT /api/User/UpdateProfileAsync` - 更新当前用户资料
- `POST /api/User/ChangePasswordAsync` - 修改当前用户密码

#### 前端优化

| 文件 | 优化内容 |
|------|---------|
| `Login.vue` | 登录成功后获取用户权限并存储到 localStorage |
| `storage.ts` | 新增 Permissions 键、权限工具函数、权限编码常量、UserDto 类型 |
| `ClearCache.vue` | 添加权限控制，按钮根据权限显示；表格添加操作列 |
| `Users.vue` | 添加列表缓存逻辑、权限控制 |
| `UserRole.vue` | 添加列表缓存逻辑、权限控制、分配权限功能、删除前检查关联 |
| `Menu.vue` | 添加权限控制 |
| `System.vue` | 添加权限控制 |
| `Permissions.vue` | 新增权限管理页面，支持 CRUD 和模块筛选 |
| `Profile.vue` | 完整重写，添加基本信息展示、编辑资料对话框、修改密码对话框 |
| `user.ts` | 新增 updateProfile、changePassword API 函数 |
| `api/index.ts` | 新增 UpdateProfileRequest、ChangePasswordRequest 类型定义 |

#### 后端优化

| 文件 | 优化内容 |
|------|---------|
| `LoginService.cs` | JWT 配置从数据库获取，登录时检查用户角色状态 |
| `ApplicationDbContext.cs` | 新增 Permissions 和 RolePermissions DbSet，新增权限种子数据 |
| `Startup.cs` | 启动时初始化权限种子数据 |
| `BaseApiController.cs` | 新增 GetCurrentUserId、GetCurrentUserName 方法 |
| `IUserDataService.cs` | 新增 UpdateProfileAsync、ChangePasswordAsync 接口 |
| `UserDataService.cs` | 实现更新资料和修改密码逻辑 |
| `UserDTO.cs` | 新增 UpdateProfileRequest、ChangePasswordRequest DTO |

#### 权限编码常量

| 模块 | 权限编码 | 说明 |
|------|----------|------|
| Menu | `menu:add`, `menu:edit`, `menu:delete`, `menu:add_child` | 菜单管理权限 |
| User | `user:add`, `user:edit`, `user:delete`, `user:reset_password`, `user:assign_role`, `user:enable`, `user:disable` | 用户管理权限 |
| Role | `role:add`, `role:edit`, `role:delete`, `role:assign_menu`, `role:assign_user`, `role:enable`, `role:disable` | 角色管理权限 |
| Setting | `setting:save_jwt`, `setting:save_system` | 系统设置权限 |
| Cache | `cache:clear_auth`, `cache:clear_user`, `cache:clear_menu`, `cache:clear_list`, `cache:clear_setting`, `cache:clear_all` | 缓存管理权限 |

#### 文档更新

- 新增权限管理模块文档
- 新增 JWT 动态配置说明
- 新增权限编码常量说明
- 更新文档更新日志

---

### 2026-03-25

#### 新增功能

| 模块 | 功能 | 说明 |
|------|------|------|
| MenuRole | 菜单角色关联模块 | 新增菜单与角色的多对多关联功能 |
| Role | 角色管理模块 | 完善角色CRUD、用户角色分配、角色菜单分配等功能 |
| UserRole | 用户角色关联模块 | 支持用户与角色的多对多关联 |

#### 新增文件

**Domain 层：**
- `Entities/MenuRole.cs` - 菜单角色关联实体

**Application 层：**
- `DTOs/MenuRoleDTO.cs` - 菜单角色 DTO 和请求类
- `Interfaces/IMenuRoleService.cs` - 菜单角色服务接口
- `Services/MenuRoleService.cs` - 菜单角色服务实现

**Infrastructure 层：**
- `Configuration/MenuRoleConfiguration.cs` - 菜单角色 EF Core 配置

**API 层：**
- `Controllers/MenuRoleController.cs` - 菜单角色控制器

#### 数据库迁移

| 迁移名称 | 说明 |
|----------|------|
| `AddRoleTable` | 创建角色表 |
| `AddUserRoleTable` | 创建用户角色关联表 |
| `AddMenuRoleTable` | 创建菜单角色关联表 |

#### API 接口

**MenuRoleController：**
- `GET /api/MenuRole/GetRoleMenuIdsAsync` - 获取角色的菜单ID列表
- `GET /api/MenuRole/GetMenuRoleIdsAsync` - 获取菜单的角色ID列表
- `POST /api/MenuRole/AssignRoleMenusAsync` - 为角色分配菜单
- `POST /api/MenuRole/AssignMenuRolesAsync` - 为菜单分配角色
- `GET /api/MenuRole/GetRoleMenusAsync` - 获取角色的菜单树
- `GET /api/MenuRole/GetUserMenusByRolesAsync` - 获取用户菜单
- `DELETE /api/MenuRole/ClearRoleMenusAsync` - 清除角色菜单权限
- `DELETE /api/MenuRole/ClearMenuRolesAsync` - 清除菜单角色关联

**RoleController：**
- `GET /api/Role/GetRolesAsync` - 获取角色列表（分页）
- `GET /api/Role/GetRoleByIdAsync` - 获取角色详情
- `POST /api/Role/CreateRoleAsync` - 创建角色
- `PUT /api/Role/UpdateRoleAsync` - 更新角色
- `DELETE /api/Role/DeleteRoleAsync` - 删除角色
- `POST /api/Role/EnableRoleAsync` - 启用角色
- `POST /api/Role/DisableRoleAsync` - 禁用角色
- `GET /api/Role/GetUserRoleIdsAsync` - 获取用户角色
- `POST /api/Role/AssignUserRolesAsync` - 配置用户角色
- `GET /api/Role/GetEnabledRolesAsync` - 获取启用角色
- `GET /api/Role/GetRoleUserIdsAsync` - 获取角色用户
- `POST /api/Role/AssignRoleUsersAsync` - 配置角色用户

#### 文档更新

- 整理 README.md，将开发规范部分整合为统一章节
- 优化文档结构，添加目录导航

---

## 快速开始

### 环境要求

- .NET 10.0 SDK
- Node.js 18+
- SQL Server

### 构建和运行

#### 后端项目（DDDProject）

```bash
# 进入项目目录
cd DDDProject

# 构建解决方案
dotnet build

# 运行 API 项目
dotnet run --project DDDProject.API

# 创建数据库迁移
dotnet ef migrations add MigrationName --project DDDProject.Infrastructure --startup-project DDDProject.API --output-dir Contexts/Migrations

# 更新数据库
dotnet ef database update --project DDDProject.Infrastructure --startup-project DDDProject.API
```

#### 前端项目（DDDVue）

```bash
# 进入项目目录
cd DDDVue

# 安装依赖
npm install

# 开发模式运行
npm run dev

# 构建生产版本
npm run build
```

### 访问地址

| 项目 | 地址 | 说明 |
|------|------|------|
| 前端 | http://localhost:5173/ | Vue 开发服务器 |
| 后端 API | http://localhost:5272/ | ASP.NET Core WebAPI |
| Swagger | http://localhost:5272/swagger | API 文档 |

---

## 项目结构

```
DDDNewProject/
├── DDDProject/               # DDD 后端项目（.NET Core WebAPI）
│   ├── DDDProject.Domain/          # 领域层（Domain Layer）
│   ├── DDDProject.Application/     # 应用层（Application Layer）
│   ├── DDDProject.Infrastructure/  # 基础设施层（Infrastructure Layer）
│   ├── DDDProject.API/             # API 层（Presentation Layer）
│   └── DDDProject.sln              # 解决方案文件
└── DDDVue/                   # Vue 前端项目
    ├── src/
    │   ├── api/              # API 接口统一管理
    │   ├── assets/           # 静态资源
    │   ├── components/       # 公共组件
    │   ├── router/           # 路由配置
    │   ├── utils/            # 工具函数
    │   ├── views/            # 页面组件
    │   ├── App.vue           # 根组件
    │   └── main.ts           # 入口文件
    └── package.json
```

---

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

---

## 开发规范

本章节详细说明项目的开发规范和最佳实践，请务必遵循。

### 5.1 通用编码规范

#### 5.1.1 模型属性默认值规范

所有实体类的属性**必须设置默认值**，以避免空引用异常和确保数据一致性。

**基本原则：**

| 类型 | 默认值 | 示例 |
|------|--------|------|
| 字符串 | `string.Empty` | `public string Name { get; set; } = string.Empty;` |
| 数值 | 业务默认值 | `public int Status { get; set; } = 1;` |
| 日期时间 | `DateTime.Now` | `public DateTime CreatedAt { get; set; } = DateTime.Now;` |
| 集合 | `new List<T>()` | `public List<Menu> Children { get; set; } = new();` |
| 可空类型 | 显式声明 | `public string? Phone { get; set; }` |

**正确示例：**

```csharp
public class User : AggregateRoot
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int Status { get; private set; } = 1;
    public DateTime? LastLoginTime { get; private set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
```

**错误示例：**

```csharp
// ❌ 错误：字符串属性没有默认值
public string UserName { get; set; }

// ❌ 错误：集合属性没有默认值
public ICollection<Menu> Children { get; set; }
```

#### 5.1.2 Null 检查规范

所有 null 检查**必须使用模式匹配语法**：

```csharp
// ✅ 正确
if (user is null) { /* ... */ }
if (existingUser is not null) { /* ... */ }

// ❌ 错误
if (user == null) { /* ... */ }
if (existingUser != null) { /* ... */ }
```

**为什么使用模式匹配？**
- 类型安全：编译器进行更严格的类型检查
- 可读性：`is not null` 更接近自然语言
- 避免运算符重载问题：`is null` 不受 `==` 重载影响

#### 5.1.3 异步编程规范

所有与数据库交互的操作**必须使用异步方法**：

| 同步方法 | 异步方法 | 说明 |
|---------|---------|------|
| `ToList()` | `ToListAsync()` | 获取列表 |
| `FirstOrDefault()` | `FirstOrDefaultAsync()` | 获取第一个元素 |
| `Find()` | `FindAsync()` | 根据主键查找 |
| `Add()` | `AddAsync()` | 添加实体 |
| `SaveChanges()` | `SaveChangesAsync()` | 保存更改 |

**注意事项：**
- 避免阻塞调用：不要使用 `.Result`、`.Wait()` 或 `.GetAwaiter().GetResult()`
- 保持异步链：从 Controller 到 Service 到 Repository，整个调用链都应该是异步的
- 使用 CancellationToken：对于长时间运行的操作，应接受 `CancellationToken` 参数

#### 5.1.4 时区设置说明

本项目所有时间相关操作都使用**中国标准时间**（CST，UTC+8）：

- 实体创建时间使用 `DateTime.Now` 替代 `DateTime.UtcNow`
- 数据库存储使用服务器本地时间
- 生产服务器应设置时区为 **(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi**

---

### 5.2 后端开发规范

#### 5.2.1 DDD 分层架构

| 层级 | 职责 | 目录 |
|------|------|------|
| Domain 层 | 核心业务逻辑和实体 | `DDDProject.Domain/` |
| Application 层 | 协调领域对象，处理用例 | `DDDProject.Application/` |
| Infrastructure 层 | 实现抽象接口，提供技术细节 | `DDDProject.Infrastructure/` |
| API 层 | 处理 HTTP 请求，返回响应 | `DDDProject.API/` |

#### 5.2.2 Service 层规范

**基本规范：**

1. **新建接口**: 在 `DDDProject.Application\Interfaces` 目录下创建
2. **新建实现类**: 在 `DDDProject.Application\Services` 目录下创建
3. **继承关系**: Service 实现类继承自对应接口，接口继承自 `IApplicationService`
4. **异步方法命名**: 所有异步方法名必须添加 `Async` 后缀
5. **数据库操作**: 所有数据库操作必须使用异步方法
6. **返回值**: Service 方法返回 `ApiRequestResult`

**枚举类型使用规范：**

```csharp
// ❌ 错误：使用字符串常量
[ApiSearch(Name = "获取菜单列表", Category = "菜单管理")]

// ✅ 正确：使用枚举类型
[ApiSearch(Name = "获取菜单列表", Category = ApiSearchCategory.Menu)]
```

**分页请求规范：**

分页请求参数统一使用 `PagedRequest` 类，返回值使用 `PagedResult<T>`：

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

#### 5.2.3 Controller 层规范

**注解规范：**

必须使用 `[ActionName("方法名")]` 注解，确保路由路径与方法名一致：

```csharp
[ApiController]
[Route("api/[controller]/[action]")]
public class YourController : BaseApiController
{
    [HttpPost]
    [ActionName("LoginAsync")]
    public async Task<ApiRequestResult> LoginAsync([FromBody] LoginRequest request)
    {
        // 实现逻辑
    }
}
```

**为什么需要 `[ActionName]`？**
- 确保 API 路径与前端 `api/index.ts` 配置一致
- 路由模板 `[Route("api/[controller]/[action]")]` 中的 `[action]` 会自动使用 `[ActionName]` 的值
- 减少因路由不一致导致的 404 或 405 错误

**编写要点：**

1. 继承 `BaseApiController`
2. 通过构造函数注入所需的 Service
3. 使用 `[ActionName]` 注解
4. 使用 `async/await` 并返回 `Task<IActionResult>`
5. 添加 `[ApiSearch]` 标签用于接口发现

#### 5.2.4 权限注解规范

**所有涉及数据变更的 API 方法必须使用 `[Permission]` 注解进行权限控制。**

**基本用法：**

```csharp
// 单个权限验证
[Permission("user:add")]
public async Task<ApiRequestResult> CreateUserAsync([FromBody] CreateUserRequest request)

// 多个权限验证（满足任一）
[Permission(new[] { "user:edit", "user:admin" }, PermissionLogic.Any)]
public async Task<ApiRequestResult> UpdateUserAsync([FromBody] UpdateUserRequest request)

// 多个权限验证（满足所有）
[Permission(new[] { "user:delete", "user:admin" }, PermissionLogic.All)]
public async Task<ApiRequestResult> DeleteUserAsync([FromQuery] Guid id)
```

**权限编码命名规范：**

权限编码格式为 `模块:操作`，必须全小写，使用冒号分隔：

| 模块 | 操作类型 | 示例 |
|------|----------|------|
| 用户管理 | add, edit, delete, enable, disable, reset_password | `user:add`, `user:edit` |
| 角色管理 | add, edit, delete, enable, disable, assign_user, assign_menu | `role:add`, `role:assign_user` |
| 菜单管理 | add, edit, delete, enable, disable | `menu:add`, `menu:edit` |
| 按钮管理 | add, edit, delete, enable, disable | `button:add`, `button:edit` |

**必须添加权限注解的方法：**

- 创建（Create）
- 更新（Update）
- 删除（Delete）
- 启用/禁用（Enable/Disable）
- 分配/配置（Assign）

**不需要添加权限注解的方法：**

- 查询列表（GetList）
- 查询详情（GetById）
- 获取下拉选项（GetEnabledItems）

**示例：**

```csharp
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class UserController : BaseApiController
{
    // ✅ 查询方法不需要权限注解
    [HttpGet]
    [ActionName("GetUsersAsync")]
    public async Task<ApiRequestResult> GetUsersAsync([FromQuery] PagedRequest request)
    {
        return await _userService.GetUsersAsync(request);
    }

    // ✅ 数据变更方法必须添加权限注解
    [HttpPost]
    [ActionName("CreateUserAsync")]
    [Permission("user:add")]
    public async Task<ApiRequestResult> CreateUserAsync([FromBody] CreateUserRequest request)
    {
        return await _userService.CreateUserAsync(request);
    }

    // ✅ 更新操作
    [HttpPut]
    [ActionName("UpdateUserAsync")]
    [Permission("user:edit")]
    public async Task<ApiRequestResult> UpdateUserAsync([FromBody] UpdateUserRequest request)
    {
        return await _userService.UpdateUserAsync(request);
    }

    // ✅ 删除操作
    [HttpDelete]
    [ActionName("DeleteUserAsync")]
    [Permission("user:delete")]
    public async Task<IActionResult> DeleteUserAsync([FromQuery] Guid id)
    {
        return await _userService.DeleteUserAsync(id);
    }
}
```

**注意事项：**

1. `[Permission]` 特性继承自 `[Authorize]`，所以不需要同时使用两者
2. 权限验证失败会返回 403 Forbidden 状态码
3. 如果用户未登录，会先触发身份验证失败（401 Unauthorized）
4. 权限编码应与数据库中 Permissions 表的 Code 字段保持一致
5. 新增权限需要在 `PermissionSeeder.cs` 中添加种子数据

#### 5.2.5 Repository 层规范

**双泛型仓储（Repository<TEntity, TId>）**：针对各种主键类型的通用实体仓储实现

**单泛型仓储（Repository<TEntity>）**：专门针对 Guid 主键的实体，新建类型都使用单泛型仓储

```csharp
public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity<Guid>
{
    public async Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    // ... 其他方法
}
```

#### 5.2.5 树形结构数据处理规范

**识别树形结构：** 当实体包含 `ParentId`、`ParentCode` 等字段时，表明为树形结构。

**Service 层处理：**

```csharp
public async Task<ApiRequestResult> GetTreeMenusAsync()
{
    var menus = await _menuRepository.GetListAsync(m => true);
    var menuList = menus.ToList();

    // 获取根节点
    var rootMenus = menuList.Where(m => m.ParentId is null || m.ParentId == Guid.Empty)
                           .OrderBy(m => m.SortOrder)
                           .ToList();

    // 递归构建树形结构
    var treeMenus = BuildTreeMenu(rootMenus, menuList);

    return new ApiRequestResult { Success = true, Message = "操作成功", Data = treeMenus };
}
```

**前端表格层级显示：**

```vue
<el-table
  :data="menuList"
  row-key="id"
  :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
>
  <!-- 列定义 -->
</el-table>
```

#### 5.2.6 API Search 功能

用于自动扫描和检索项目中所有标记了 `[ApiSearch]` 注解的 API 方法。

**ApiSearchCategory 枚举值：**

| 枚举值 | 说明 |
|--------|------|
| `Login` | 登录认证相关操作 |
| `Menu` | 菜单管理相关操作 |
| `User` | 用户管理相关操作 |
| `Role` | 角色管理相关操作 |
| `Dictionary` | 数据字典相关操作 |
| `Log` | 日志管理相关操作 |
| `Setting` | 系统设置相关操作 |
| `File` | 文件上传相关操作 |
| `Other` | 其他操作 |

**使用示例：**

```csharp
[HttpGet]
[ActionName("GetAsync")]
[ApiSearch(Name = "获取示例列表", Description = "返回示例数据列表", Category = ApiSearchCategory.Other)]
public async Task<ApiRequestResult> GetAsync()
{
    return await yourService.GetAsync();
}
```

**可用接口：**

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/ApiSearch/Search` | GET | 获取所有标记了 ApiSearch 注解的 API |
| `/api/ApiSearch/SearchByCategory?category=xxx` | GET | 根据分类获取 API 列表 |
| `/api/ApiSearch/SearchByKeyWord?keyword=xxx` | GET | 根据关键词搜索 API |

---

### 5.3 前端开发规范

#### 5.3.1 Vue 路由规范

**核心原则：**

1. **动态路由**: 从 API 动态获取路由配置
2. **扁平化**: 所有路由均为单层，无嵌套路由
3. **一一对应**: 路由路径与页面组件一一对应
4. **存储统一**: 菜单和路由数据使用同一个 localStorage 键 `sidebarMenu`

**路由数据来源：**

路由数据从后端 API `http://localhost:5272/api/Menu/GetSidebarMenusAsync` 动态获取。

**路径命名规范：**

- 使用短横线命名法：`settings-menus`
- 子菜单路径命名：`[父级菜单路径]-[子菜单名称]`
  - 示例：父菜单 `settings`，子菜单 `menu` → 子菜单路径为 `settings-menu`

**添加新页面步骤：**

1. 在 `DDDVue/src/views/home/` 下创建页面组件
2. 在后端数据库的 `Menu` 表中添加菜单记录
3. 清除浏览器 localStorage 或刷新页面

**页面文件命名规范：**

```
views/home/
├── Dashboard/
│   └── Dashboard.vue          # 路由路径: /dashboard
├── Users/
│   └── Users.vue              # 路由路径: /users
├── menu/
│   └── Menu.vue               # 路由路径: /menu
```

#### 5.3.2 localStorage 分类管理

**分类说明：**

| 分类 | 存储内容 | 键名 |
|------|----------|------|
| Login | 登录相关信息 | `token`, `userInfo` |
| Menu | 菜单相关数据 | `sidebarMenu` |
| List | 列表数据缓存 | `list_模块名_页码_每页数量` |

**列表缓存键格式：**

列表项数据应存入缓存中，缓存键统一格式为：`list_模块名_页码_每页数量`

| 列表 | 缓存键格式 | 示例 |
|------|-----------|------|
| 菜单列表 | `list_menu_{pageNum}_{pageSize}` | `list_menu_1_10` |
| 用户列表 | `list_user_{pageNum}_{pageSize}` | `list_user_1_10` |
| 角色列表 | `list_role_{pageNum}_{pageSize}` | `list_role_1_10` |

**使用方法：**

```typescript
import { getItem, setItem, removeItem, clearByCategory, StorageKeys } from '@/utils/storage'

// 获取数据
const token = getItem<string>(StorageKeys.Token)

// 设置数据
setItem(StorageKeys.Token, token)

// 列表缓存（优先从缓存获取）
const cacheKey = `${StorageKeys.List}_user_${pageNum}_${pageSize}`
const cachedData = getItem<{ list: UserDto[], total: number }>(cacheKey)
if (cachedData) {
  // 使用缓存数据
  return cachedData
}
// 缓存不存在，从 API 获取后存入缓存
setItem(cacheKey, { list, total })

// 分类清除
clearByCategory('Login')  // 清除登录缓存
clearByCategory('Menu')   // 清除菜单缓存
clearByCategory('List')   // 清除列表缓存（所有以 'list' 开头的键）
clearByCategory('All')    // 清除全部缓存
```

**缓存更新规则：**

在执行编辑、删除、添加、状态变更等操作后，**必须重新获取最新数据并更新缓存**：

```typescript
// 示例：提交表单后更新缓存
const submitForm = async () => {
  try {
    await formRef.value.validate()

    if (isEdit) {
      await updateApi(formData)
      showSuccessNotification({ title: '成功', message: '编辑成功' })
    } else {
      await createApi(formData)
      showSuccessNotification({ title: '成功', message: '添加成功' })
    }

    dialogVisible.value = false
    await loadData()  // 重新获取数据，更新缓存
  } catch (error) {
    console.log('操作失败:', error)
  }
}

// 示例：删除后更新缓存
const deleteItem = async (id: string) => {
  try {
    await ElMessageBox.confirm('确认删除吗？', '提示', { type: 'warning' })
    await deleteApi(id)
    showSuccessNotification({ title: '成功', message: '删除成功' })
    await loadData()  // 重新获取数据，更新缓存
  } catch (error) {
    console.log('取消删除或删除失败')
  }
}

// 示例：状态变更后更新缓存
const toggleStatus = async (row: Item) => {
  try {
    await ElMessageBox.confirm('确认操作吗？', '提示', { type: 'warning' })
    row.status === 1 ? await disableApi(row.id) : await enableApi(row.id)
    showSuccessNotification({ title: '成功', message: '操作成功' })
    await loadData()  // 重新获取数据，更新缓存
  } catch (error) {
    console.log('取消操作或操作失败')
  }
}
```

**注意事项：**

- `loadData()` 方法内部已实现缓存逻辑，调用后会自动更新缓存
- 所有数据变更操作（增删改、状态变更）后都必须调用 `loadData()` 刷新缓存
- 确保用户看到的数据始终是最新的

#### 5.3.4 按钮权限设计规范

**核心原则：页面按钮的显示/隐藏由按钮数据决定，而非权限编码**

**设计思路：**

1. 按钮数据存储在数据库 `Buttons` 表中，与菜单关联
2. 页面加载时，根据当前菜单路径获取该菜单下的所有按钮
3. 通过按钮编码判断按钮是否显示

**按钮数据结构：**

```typescript
interface ButtonDto {
  id: string
  name: string          // 按钮名称，如"添加用户"
  code: string          // 按钮编码，如"user:add"
  menuId: string        // 所属菜单ID
  menuName?: string     // 所属菜单名称
  permissionCode?: string // 关联的权限编码
  icon?: string         // 按钮图标
  sortOrder: number     // 排序号
  status: number        // 状态：1-启用，0-禁用
}
```

**按钮管理工具（`src/utils/buttons.ts`）：**

```typescript
import { useButtons } from '@/utils/buttons'

// 在组件中使用
const { hasBtn, hasAnyBtn } = useButtons('users')  // 传入当前菜单路径

// 单个按钮权限检查
<el-button v-if="hasBtn('user:add')">添加用户</el-button>

// 多个按钮权限检查（任意一个）
<el-button v-if="hasAnyBtn(['user:enable', 'user:disable'])">启用/禁用</el-button>
```

**API 接口：**

| 接口 | 说明 |
|------|------|
| `GET /api/Button/GetButtonsAsync` | 获取按钮列表（分页，支持按菜单筛选） |
| `GET /api/Button/GetButtonsByMenuIdAsync` | 根据菜单ID获取按钮列表 |
| `POST /api/Button/CreateButtonAsync` | 创建按钮 |
| `PUT /api/Button/UpdateButtonAsync` | 更新按钮 |
| `DELETE /api/Button/DeleteButtonAsync` | 删除按钮 |
| `POST /api/Button/EnableButtonAsync` | 启用按钮 |
| `POST /api/Button/DisableButtonAsync` | 禁用按钮 |

**按钮编码命名规范：**

按钮编码格式为 `模块:操作`，例如：

| 模块 | 按钮编码 | 说明 |
|------|----------|------|
| 用户管理 | `user:add`, `user:edit`, `user:delete`, `user:reset_password`, `user:assign_role`, `user:enable`, `user:disable` | 用户相关操作 |
| 角色管理 | `role:add`, `role:edit`, `role:delete`, `role:assign_menu`, `role:assign_user`, `role:assign_permission`, `role:enable`, `role:disable` | 角色相关操作 |
| 菜单管理 | `menu:add`, `menu:edit`, `menu:delete`, `menu:add_child` | 菜单相关操作 |
| 权限管理 | `permission:add`, `permission:edit`, `permission:delete`, `permission:enable`, `permission:disable` | 权限相关操作 |
| 按钮管理 | `button:add`, `button:edit`, `button:delete`, `button:enable`, `button:disable` | 按钮相关操作 |
| 系统设置 | `setting:save_jwt`, `setting:save_system` | 系统配置操作 |
| 缓存管理 | `cache:clear_auth`, `cache:clear_user`, `cache:clear_menu`, `cache:clear_list`, `cache:clear_setting`, `cache:clear_all` | 缓存清理操作 |

**完整使用示例：**

```vue
<template>
  <div class="user-container">
    <!-- 顶部操作按钮 -->
    <el-button v-if="hasBtn('user:add')" type="primary" @click="addUser">添加用户</el-button>

    <!-- 表格操作列 -->
    <el-table-column label="操作" width="200">
      <template #default="{ row }">
        <el-button v-if="hasBtn('user:edit')" size="small" @click="editUser(row)">编辑</el-button>
        <el-button v-if="hasBtn('user:delete')" size="small" type="danger" @click="deleteUser(row)">删除</el-button>
        <el-button v-if="hasAnyBtn(['user:enable', 'user:disable'])" size="small" @click="toggleStatus(row)">
          {{ row.status === 1 ? '禁用' : '启用' }}
        </el-button>
      </template>
    </el-table-column>
  </div>
</template>

<script setup lang="ts">
import { useButtons } from '@/utils/buttons'

const { hasBtn, hasAnyBtn } = useButtons('users')
</script>
```

**按钮种子数据生成：**

按钮种子数据根据菜单自动生成，在 `ButtonSeeder.cs` 中配置：

```csharp
// 根据菜单路径生成对应的按钮
switch (menu.Path)
{
    case "users":
        buttons.Add(CreateButton("添加用户", "user:add", menu.Id, "Plus", 1));
        buttons.Add(CreateButton("编辑用户", "user:edit", menu.Id, "Edit", 2));
        buttons.Add(CreateButton("删除用户", "user:delete", menu.Id, "Delete", 3));
        break;
}
```

**注意事项：**

- 按钮数据会被缓存到 localStorage，键名格式：`button_menuPath`
- 新增菜单时，需要在 `ButtonSeeder.cs` 中添加对应的按钮生成逻辑
- 按钮的 `permissionCode` 应与权限表中的权限编码一致，用于权限分配

---

### 5.4 安全规范

#### 5.4.1 JWT 认证配置

**配置参数（appsettings.json）：**

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

**使用方式：**

需要身份验证的控制器使用 `[Authorize]` 特性：

```csharp
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class MenuController : BaseApiController
{
    // 需要身份验证的方法
}
```

**CurrentUser 服务：**

用于获取当前登录用户信息：

```csharp
public class MenuController : BaseApiController
{
    private readonly CurrentUser _currentUser;

    public MenuController(CurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task<ApiRequestResult> GetUserSpecificDataAsync()
    {
        var userId = _currentUser.UserId;
        var userName = _currentUser.UserName;
        // 使用当前用户信息进行业务处理
    }
}
```

#### 5.4.2 权限认证说明

**核心原则：只有标记了 `[Authorize]` 注解的 API 才需要登录验证**

```csharp
// 无需登录的 API
[HttpGet]
public async Task<ApiRequestResult> GetPublicData()
{
    // 无需登录即可访问
}

// 需要登录的 API
[HttpGet]
[Authorize]
public async Task<ApiRequestResult> GetUserData()
{
    // 需要登录
}
```

#### 5.4.3 密码加密传输

采用 AES-256-CBC 算法对用户密码进行加密传输：

- **加密算法**: AES-256-CBC
- **密钥**: `DDDProject2024SecretKey!`（前后端一致）
- **初始化向量 (IV)**: `DDDProject2024IV!`（前后端一致）
- **填充模式**: PKCS7

**前端加密：**

```typescript
import { aesEncrypt } from '../utils/crypto'

const response = await http.post(api.Login.Login, {
  userName: form.value.userName,
  password: aesEncrypt(form.value.password)  // 密码加密传输
})
```

**后端解密：**

```csharp
var decryptedPassword = PasswordHelper.DecryptPassword(request.Password);
var passwordHash = PasswordHelper.ComputeHash(decryptedPassword);
```

---

### 5.5 配置规范

#### 5.5.1 端口配置（禁止随意更改）

| 项目 | 默认端口 | 配置文件 |
|------|----------|----------|
| 前端 | 5173 | `DDDVue/vite.config.ts` |
| 后端 | 5272 | `DDDProject.API/Properties/launchSettings.json` |

#### 5.5.2 数据库配置

**连接字符串：**

```
Data Source=.;Initial Catalog=AiSpace;Persist Security Info=True;User ID=sa;Password=123456;Encrypt=False
```

**数据库名称**: AiSpace

---

### 5.6 Git 规范

#### 5.6.1 分支命名规范

| 分支 | 说明 |
|------|------|
| `master` | 主分支，稳定版本 |
| `feature/xxx` | 功能分支 |
| `bugfix/xxx` | 修复分支 |
| `hotfix/xxx` | 紧急修复分支 |

#### 5.6.2 提交信息规范

采用 conventional commits 格式：

```
<type>(<scope>): <subject>
```

**type 类型：**

| 类型 | 说明 |
|------|------|
| `feat` | 新功能 |
| `fix` | 修复 bug |
| `docs` | 文档变更 |
| `style` | 代码格式变更 |
| `refactor` | 重构 |
| `perf` | 性能优化 |
| `test` | 测试相关 |
| `chore` | 构建过程或辅助工具变动 |

**示例：**

```
feat(user): 添加用户管理功能
fix(api): 修复 API 分页参数错误
docs(readme): 更新项目说明文档
```

#### 5.6.3 拉推原则（Pull-Push Principle）

**提交代码前必须先拉取最新代码**，遵循"先 pull 再 push"原则：

```bash
# 1. 先拉取远程最新代码
git pull origin master

# 2. 解决可能的冲突后，再提交
git add .
git commit -m "提交信息"

# 3. 最后推送到远程仓库
git push origin master
```

**为什么需要先 pull？**
- 避免推送时出现冲突
- 确保本地代码与远程同步
- 减少不必要的合并操作

#### 5.6.4 常用 Git 命令

```bash
# 查看仓库状态
git status

# 添加所有修改到暂存区
git add .

# 提交修改
git commit -m "提交信息"

# 推送到远程仓库（确保先 pull）
git push origin master

# 从远程仓库拉取最新代码
git pull origin master

# 创建并切换到新分支
git checkout -b feature/branch-name

# 查看提交历史
git log --oneline
```

---

## 完整示例：添加新的 API 功能

### 1. 创建实体（Domain 层）

```csharp
// DDDProject.Domain/Entities/EntityName.cs
public class EntityName : AggregateRoot
{
    public string Property1 { get; private set; } = string.Empty;
    public int Property2 { get; private set; }

    protected EntityName() { }

    public static EntityName Create(string property1, int property2)
    {
        return new EntityName
        {
            Id = Guid.NewGuid(),
            Property1 = property1,
            Property2 = property2,
            CreatedAt = DateTime.Now
        };
    }

    public void Update(string? property1 = null, int? property2 = null)
    {
        if (!string.IsNullOrEmpty(property1))
            Property1 = property1;
        if (property2.HasValue)
            Property2 = property2.Value;
        UpdatedAt = DateTime.Now;
    }
}
```

### 2. 创建 DTO（Application 层）

```csharp
// DDDProject.Application/DTOs/EntityNameDto.cs
public class EntityNameDto
{
    public Guid Id { get; set; }
    public string Property1 { get; set; } = string.Empty;
    public int Property2 { get; set; }
}
```

### 3. 创建服务接口（Application 层）

```csharp
// DDDProject.Application/Interfaces/IEntityNameService.cs
public interface IEntityNameService : IApplicationService
{
    Task<ApiRequestResult> GetEntityNamesAsync(PagedRequest request);
    Task<ApiRequestResult> GetEntityNameByIdAsync(Guid id);
    Task<ApiRequestResult> CreateEntityNameAsync(EntityNameDto dto);
    Task<ApiRequestResult> UpdateEntityNameAsync(EntityNameDto dto);
    Task<ApiRequestResult> DeleteEntityNameAsync(Guid id);
}
```

### 4. 创建服务实现（Application 层）

```csharp
// DDDProject.Application/Services/EntityNameService.cs
public class EntityNameService : IEntityNameService
{
    private readonly IRepository<EntityName> _repository;

    public EntityNameService(IRepository<EntityName> repository)
    {
        _repository = repository;
    }

    public async Task<ApiRequestResult> GetEntityNamesAsync(PagedRequest request)
    {
        var skipCount = (request.PageNumber - 1) * request.PageSize;
        var total = await _repository.CountAsync(m => true);
        var entities = await _repository.GetListAsync(
            m => true,
            q => q.OrderBy(m => m.Id),
            skipCount,
            request.PageSize
        );

        var pagedResult = new PagedResult<EntityNameDto>
        {
            List = entities.Select(e => new EntityNameDto { Id = e.Id, Property1 = e.Property1, Property2 = e.Property2 }).ToList(),
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = pagedResult };
    }

    // ... 其他方法实现
}
```

### 5. 创建控制器（API 层）

```csharp
// DDDProject.API/Controllers/EntityNameController.cs
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class EntityNameController : BaseApiController
{
    private readonly IEntityNameService _service;

    public EntityNameController(IEntityNameService service)
    {
        _service = service;
    }

    [HttpGet]
    [ActionName("GetEntityNamesAsync")]
    [ApiSearch(Name = "获取实体列表", Category = ApiSearchCategory.Other)]
    public async Task<ApiRequestResult> GetEntityNamesAsync([FromQuery] int pageNum = 1, [FromQuery] int pageSize = 10)
    {
        return await _service.GetEntityNamesAsync(new PagedRequest { PageNumber = pageNum, PageSize = pageSize });
    }

    // ... 其他方法
}
```

### 6. 添加数据库迁移

```bash
dotnet ef migrations add CreateEntityName --project DDDProject.Infrastructure --startup-project DDDProject.API
dotnet ef database update --project DDDProject.Infrastructure --startup-project DDDProject.API
```

---

## 注意事项

- 遵循 DDD 设计原则
- 领域层不应依赖其他层
- 应用层协调领域层完成业务操作
- 基础设施层实现抽象接口
- API 层仅处理 HTTP 相关注
- 所有数据库操作必须使用异步方法
- 控制器方法必须使用 `[ActionName]` 注解
- 使用 `DateTime.Now` 代替 `DateTime.UtcNow`（中国标准时间）