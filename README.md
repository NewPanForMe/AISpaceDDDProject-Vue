# DDDNewProject - DDD 分层架构全栈项目

本项目采用 DDD（Domain-Driven Design）分层架构设计，包含后端 API（DDDProject）和前端 Vue（DDDVue）两个子项目。

## 目录

- [快速开始](#快速开始)
- [项目结构](#项目结构)
- [技术栈](#技术栈)
- [开发规范](#开发规范)
  - [通用编码规范](#51-通用编码规范)
  - [后端开发规范](#52-后端开发规范)
  - [前端开发规范](#53-前端开发规范)
  - [安全规范](#54-安全规范)
  - [配置规范](#55-配置规范)
  - [Git 规范](#56-git-规范)
- [完整示例](#完整示例添加新的-api-功能)
- [实际开发清单](#实际开发清单新增一个模块时按什么顺序改)
- [注意事项](#注意事项)
- [文档更新日志](#文档更新日志)

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

#### 5.2.6 树形结构数据处理规范

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

#### 5.2.7 API Search 功能

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

#### 5.3.3 字典数据使用规范

**核心原则：页面上的下拉选项数据必须通过字典接口获取，禁止硬编码**

**设计思路：**

1. 所有可复用的下拉选项（如状态、类型、模块等）都应记录在字典表中
2. 页面通过字典类型（type）从接口获取对应的选项列表
3. 字典数据支持缓存，减少重复请求

**字典数据结构：**

```typescript
interface DictItem {
  label: string        // 显示文本（对应字典的 name）
  value: string | number  // 实际值（对应字典的 value）
  code?: string        // 字典编码
  sortOrder?: number   // 排序
}
```

**预定义字典类型（`DICT_TYPES`）：**

| 字典类型 | 说明 | 示例值 |
|----------|------|--------|
| `status` | 通用状态 | 启用(1)、禁用(0) |
| `gender` | 性别 | 男(1)、女(2)、其他(0) |
| `user_type` | 用户类型 | 管理员(admin)、普通用户(user)、访客(guest) |
| `menu_type` | 菜单类型 | 目录(directory)、菜单(menu)、按钮(button) |
| `permission_type` | 权限类型 | API权限(api)、菜单权限(menu)、按钮权限(button) |
| `log_operation_type` | 操作日志-操作类型 | Create、Update、Delete、Export、Import 等 |
| `log_module` | 操作日志-操作模块 | User、Role、Menu、Permission、Setting 等 |
| `log_status` | 操作日志-状态 | Success、Failure |

**使用方法：**

```typescript
import { useDictionary, DICT_TYPES } from '@/utils/dictionary'

// 获取字典数据
const { dictData: statusOptions, loadDict: loadStatusDict, getLabelByValue: getStatusLabel } = useDictionary(DICT_TYPES.STATUS)

// 在 onMounted 中加载
onMounted(() => {
  loadStatusDict()
})
```

**模板中使用：**

```vue
<template>
  <!-- 下拉框使用字典数据 -->
  <el-select v-model="filterParams.status" placeholder="状态" clearable>
    <el-option
      v-for="item in statusOptions"
      :key="item.value"
      :label="item.label"
      :value="item.value"
    />
  </el-select>

  <!-- 表格中显示字典标签 -->
  <el-table-column prop="status" label="状态">
    <template #default="{ row }">
      {{ getStatusLabel(row.status) }}
    </template>
  </el-table-column>
</template>
```

**新增字典类型步骤：**

1. **后端**：在 `DictionarySeeder.cs` 中添加种子数据
2. **前端**：在 `dictionary.ts` 的 `DICT_TYPES` 中添加类型常量
3. **页面**：使用 `useDictionary` 获取并使用字典数据

**后端种子数据示例：**

```csharp
// DictionarySeeder.cs
Dictionary.Create("log_operation_type_create", "创建", "Create", "log_operation_type", 1, "创建操作"),
Dictionary.Create("log_operation_type_update", "更新", "Update", "log_operation_type", 2, "更新操作"),
// ...
```

**前端类型常量示例：**

```typescript
// dictionary.ts
export const DICT_TYPES = {
  // ... 其他类型
  LOG_OPERATION_TYPE: 'log_operation_type',  // 操作类型
  LOG_MODULE: 'log_module',                  // 操作模块
  LOG_STATUS: 'log_status'                   // 日志状态
}
```

**API 接口：**

| 接口 | 说明 |
|------|------|
| `GET /api/Dictionary/GetDictionariesByTypeAsync?type=xxx` | 根据类型获取字典列表 |

**注意事项：**

- 字典数据会被缓存 30 分钟，存储在 `localStorage` 中，键名格式：`dict_类型`
- 新增字典类型后，需要清除缓存或等待缓存过期才能看到新数据
- 字典的 `value` 值应与后端实体/枚举值保持一致
- 禁用状态的字典项不会返回给前端

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

## 实际开发清单：新增一个模块时按什么顺序改

本章节用于指导“从 0 到 1 新增一个完整模块”的实际开发流程。推荐按“先后端、后数据库、再前端、最后联调”的顺序推进，避免前后端同时改动时反复返工。

### 场景说明

这里的“新增一个模块”指的是类似“用户管理”“角色管理”“日志管理”这类带有列表、增删改查、权限控制、菜单、按钮和前端页面的完整功能模块。

### 一、开发顺序总览

1. 先确认模块范围：模块名称、路由路径、权限编码、是否需要字典、是否需要树形结构。
2. 在后端 Domain 层新增实体，明确字段、默认值、业务方法。
3. 在 Infrastructure 层补充 `DbSet`、实体配置、仓储依赖和迁移准备。
4. 在 Application 层新增 DTO、Service 接口、Service 实现，完成业务逻辑。
5. 在 API 层新增 Controller 和接口注解，补齐 `[ActionName]`、`[ApiSearch]`、`[Permission]`。
6. 如果模块需要菜单、按钮、权限或字典，补充对应 Seeder 数据。
7. 执行数据库迁移和更新，确认数据表、种子数据、接口返回正常。
8. 在前端新增 API 定义、页面组件、按钮控制、字典加载和缓存逻辑。
9. 在数据库中补充菜单记录，或者确认种子数据已生成菜单与按钮。
10. 联调并验证列表、详情、创建、编辑、删除、启停、权限控制、缓存刷新是否正常。

### 二、后端实际修改清单

#### 1. Domain 层

**通常需要新增或修改的文件：**

- `DDDProject.Domain/Entities/模块实体.cs`
- 如有领域规则，可补充相关枚举、常量、值对象

**要做的事：**

- 定义实体字段，所有属性按规范设置默认值
- 如果是聚合根，继承项目既有的基类
- 补充 `Create`、`Update`、`Enable`、`Disable` 等领域方法
- 时间统一使用 `DateTime.Now`
- 如果是树形结构，明确 `ParentId`、排序字段等

#### 2. Infrastructure 层

**通常需要新增或修改的文件：**

- `DDDProject.Infrastructure/Contexts/...DbContext.cs`
- `DDDProject.Infrastructure/EntityConfigurations/模块实体配置.cs` 或项目现有映射位置
- `DDDProject.Infrastructure/Contexts/Migrations/*`
- 如项目中有 Repository 注册或特殊仓储实现，也需同步补充

**要做的事：**

- 在 `DbContext` 中添加对应 `DbSet`
- 配置表名、字段长度、索引、必填项、关系映射
- 确认是否需要唯一索引、外键、级联行为
- 准备数据库迁移

#### 3. Application 层

**通常需要新增或修改的文件：**

- `DDDProject.Application/DTOs/模块Dto.cs`
- `DDDProject.Application/DTOs/Requests/模块请求Dto.cs` 或项目现有请求模型目录
- `DDDProject.Application/Interfaces/I模块Service.cs`
- `DDDProject.Application/Services/模块Service.cs`

**要做的事：**

- 定义列表、详情、新增、编辑所需 DTO
- Service 接口继承 `IApplicationService`
- 所有异步方法名必须以 `Async` 结尾
- 返回值统一使用 `ApiRequestResult`
- 分页统一使用 `PagedRequest` 和 `PagedResult<T>`
- 所有数据库操作使用异步方法
- 如果列表页需要前端缓存，返回数据结构要稳定，便于前端直接缓存

#### 4. API 层

**通常需要新增或修改的文件：**

- `DDDProject.API/Controllers/模块Controller.cs`

**要做的事：**

- 控制器继承 `BaseApiController`
- 路由统一使用 `[Route("api/[controller]/[action]")]`
- 每个方法都添加 `[ActionName("方法名")]`
- 需要接口检索的接口添加 `[ApiSearch(...)]`
- 数据变更接口必须添加 `[Permission("模块:操作")]`
- 需要登录的控制器或方法添加 `[Authorize]`

**推荐接口最小集合：**

- `Get模块列表Async`
- `Get模块详情Async`
- `Create模块Async`
- `Update模块Async`
- `Delete模块Async`
- `Enable模块Async` / `Disable模块Async`（如有状态）
- `GetEnabled模块Async`（如下拉需要）

#### 5. 种子数据与权限补充

**通常需要新增或修改的文件：**

- `PermissionSeeder.cs`
- `MenuSeeder.cs` 或数据库菜单初始化位置
- `ButtonSeeder.cs`
- `DictionarySeeder.cs`（如果页面有新的字典类型）

**要做的事：**

- 新增模块对应的权限编码，如 `module:add`、`module:edit`、`module:delete`
- 新增菜单数据，确保前端动态路由能拿到页面入口
- 为菜单生成按钮数据，确保页面按钮显示正常
- 如果页面有状态、类型、分类等下拉项，补充字典种子数据

### 三、前端实际修改清单

#### 1. API 接口层

**通常需要新增或修改的文件：**

- `DDDVue/src/api/index.ts`
- `DDDVue/src/api/模块.ts` 或项目现有接口文件

**要做的事：**

- 定义与后端 `[ActionName]` 一致的接口地址
- 统一封装列表、详情、新增、编辑、删除、启停接口
- 保持命名与后端方法一致，降低联调成本

#### 2. 页面与组件层

**通常需要新增或修改的文件：**

- `DDDVue/src/views/home/模块名/模块页面.vue`
- 如有弹窗表单、详情抽屉，可拆分到 `components`

**要做的事：**

- 新建模块列表页
- 补充查询区、表格区、分页区、弹窗表单区
- 如为树形数据，使用 `el-table` 的树形配置
- 页面路径命名遵循短横线规则，并与菜单路径保持一致

#### 3. 缓存、字典、按钮权限

**通常需要新增或修改的文件：**

- `DDDVue/src/utils/storage.ts`
- `DDDVue/src/utils/dictionary.ts`
- `DDDVue/src/utils/buttons.ts`
- 当前模块页面文件

**要做的事：**

- 列表缓存键遵循 `list_模块名_页码_每页数量`
- 增删改、启停后必须调用 `loadData()` 刷新缓存
- 页面下拉项必须走字典接口，禁止硬编码
- 页面按钮显示通过 `useButtons('菜单路径')` 控制

#### 4. 路由与菜单联动

**说明：**

- 本项目前端路由由后端菜单动态生成，通常不需要手工在前端静态路由里新增业务路由
- 新页面能否显示，关键在于后端菜单数据和页面文件路径是否匹配

**要做的事：**

- 在 `src/views/home/` 下创建对应页面组件
- 确认菜单表中的路径与页面目录能一一对应
- 新增菜单后清理 `sidebarMenu` 缓存或刷新页面验证

### 四、推荐执行顺序（可直接照做）

#### 情况 A：新增普通列表模块

1. 先定义模块名称、表名、菜单路径、权限编码前缀。
2. 创建 Domain 实体。
3. 在 Infrastructure 中补 `DbSet` 和实体映射。
4. 创建 DTO、Service 接口和 Service 实现。
5. 创建 Controller，补齐 `[ActionName]`、`[ApiSearch]`、`[Permission]`。
6. 补 `PermissionSeeder`、`MenuSeeder`、`ButtonSeeder`，如有下拉再补 `DictionarySeeder`。
7. 执行迁移和数据库更新。
8. 用 Swagger 先验证后端接口。
9. 前端新增 `api` 定义和 `views/home` 页面。
10. 接入按钮控制、字典加载、列表缓存。
11. 联调新增、编辑、删除、分页、状态切换。
12. 清缓存并验证菜单、按钮、权限是否生效。

#### 情况 B：新增树形模块

在“普通列表模块”基础上，额外补充：

1. 实体增加 `ParentId`、排序字段等层级信息。
2. Service 中增加树形组装逻辑，如 `BuildTree`。
3. 列表接口按树结构返回数据。
4. 前端表格开启 `row-key` 和 `tree-props`。
5. 新增/编辑时处理父节点选择逻辑。

#### 情况 C：新增带下拉字典的模块

在“普通列表模块”基础上，额外补充：

1. 在 `DictionarySeeder.cs` 中新增字典类型和字典项。
2. 在前端 `dictionary.ts` 中新增 `DICT_TYPES` 常量。
3. 页面通过 `useDictionary` 加载选项。
4. 清理字典缓存，确认页面展示的是最新值。

### 五、最容易漏掉的检查项

- 实体属性是否都设置了默认值
- null 判断是否使用 `is null` / `is not null`
- 所有数据库操作是否都是异步
- Controller 方法是否都加了 `[ActionName]`
- 数据变更接口是否都加了 `[Permission]`
- 新权限是否写入 `PermissionSeeder.cs`
- 新菜单是否能被前端动态路由正确识别
- 新按钮是否已在 `ButtonSeeder.cs` 中生成
- 页面下拉项是否误写成了硬编码
- 增删改后是否调用了 `loadData()` 刷新缓存
- 是否清除了 `sidebarMenu`、列表缓存、字典缓存后再验证

### 六、建议的自测顺序

1. 后端编译通过。
2. 迁移执行成功，数据库表结构正确。
3. Swagger 中逐个验证查询、详情、新增、编辑、删除接口。
4. 验证未登录、已登录、无权限三种访问情况。
5. 前端页面能正常打开，菜单能正常显示。
6. 列表、分页、弹窗表单、删除确认、状态切换功能正常。
7. 按钮显示与当前菜单按钮配置一致。
8. 字典标签、下拉项、缓存刷新都符合预期。

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

---

## 文档更新日志

| 日期 | 更新内容 |
|------|:--------:|
| 2026-04-03 | 新增字典数据使用规范（5.3.4），页面下拉选项必须通过字典接口获取；新增日志相关字典类型；修复菜单编辑失败问题 |
| 2026-04-02 | 新增操作日志模块（OperationLog），自动记录控制器中的增删改导出等操作；新增 `OperationLogFilter` 过滤器 |
| 2026-04-01 | 新增自定义权限注解 `[Permission]`，支持方法级别的权限控制；为所有控制器的数据变更方法添加权限注解 |
| 2026-03-31 | 新增按钮管理模块（Button），支持页面按钮的统一管理；新增按钮权限设计规范；新增 `useButtons` 组合式函数 |
| 2026-03-26 | 新增权限管理模块（Permission），支持按钮级别权限控制；JWT 配置改为从数据库获取；前端列表页添加缓存功能；登录时检查用户角色状态 |
| 2026-03-25 | 新增 MenuRole 模块（菜单角色关联），完善 Role 模块功能；整理文档结构，将开发规范整合为统一章节 |
