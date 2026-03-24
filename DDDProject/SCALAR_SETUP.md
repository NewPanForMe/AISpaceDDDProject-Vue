# Scalar API 文档配置说明

## 后端配置 (DDDProject.API)

### 已安装的包
- `Scalar.AspNetCore` (v2.13.13)

### 配置文件
- **Startup.cs**: 添加了 `using Scalar.AspNetCore;` 命名空间，并在 `UseEndpoints` 中配置了 `MapScalarApiReference`
- **appsettings.json**: 添加了 Scalar 配置节
- **appsettings.Development.json**: 添加了 Scalar 配置节

### 访问地址
- **后端 Scalar 文档**: `http://localhost:5272/api-docs`

## 前端配置 (DDDVue)

### 已安装的包
- `@scalar/api-reference` (v1.49.3)

### 配置文件
- **vite.config.ts**: 添加了代理配置，将 `/api-docs` 代理到后端
- **src/router/index.ts**: 添加了 `/api-docs` 路由
- **src/views/ApiDocs.vue**: 使用 Scalar API Reference 组件
- **src/views/layout/Layout.vue**: 添加了 "API 文档" 菜单项

### 访问地址
- **前端 API 文档页面**: `http://localhost:5173/api-docs`

## 启动说明

1. **启动后端**:
   ```bash
   cd D:\GitHub\AISpaceDDDProject-Vue\DDDProject
   dotnet run --project DDDProject.API\DDDProject.API.csproj
   ```

2. **启动前端**:
   ```bash
   cd D:\GitHub\AISpaceDDDProject-Vue\DDDVue
   npm run dev
   ```

3. **访问 API 文档**:
   - 打开浏览器访问 `http://localhost:5173/api-docs`
   - 或直接访问后端地址 `http://localhost:5272/api-docs`

## 配置说明

### 后端 Scalar 配置 (appsettings.json)
```json
{
  "Scalar": {
    "Title": "AISpace DDD Project API",
    "Description": "DDD Project API Documentation",
    "Path": "/api-docs"
  }
}
```

### 前端代理配置 (vite.config.ts)
```typescript
server: {
  port: 5173,
  proxy: {
    '/api-docs': {
      target: 'http://localhost:5272',
      changeOrigin: true
    }
  }
}
```
