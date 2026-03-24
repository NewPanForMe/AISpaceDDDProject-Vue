// API 接口统一管理文件
// 接口文档来源: http://localhost:5272/api/ApiSearch/SearchStr

// ==================== 登录模块 ====================
export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: string;
  userName: string;
  realName: string;
}

// ==================== 菜单模块 ====================
export interface MenuDto {
  id?: string;
  name: string;
  path: string;
  component: string;
  icon?: string;
  parentId?: string;
  sortOrder: number;
  status: number;
  children?: MenuDto[];
}

export interface CreateMenuRequestDto {
  name: string;
  path: string;
  component: string;
  icon?: string;
  parentId?: string;
  sortOrder: number;
}

export interface UpdateMenuRequestDto {
  id: string;
  name?: string;
  path?: string;
  component?: string;
  icon?: string;
  parentId?: string;
  sortOrder?: number;
}

export interface ChangeMenuStatusRequestDto {
  id: string;
}

// 分页响应接口
export interface PagedResult<T> {
  list: T[];
  total: number;
  pageNum: number;
  pageSize: number;
  totalPage: number;
}

// ==================== API 配置接口定义 ====================
export interface ApiConfig {
  ApiSearch: Record<string, any>;
  BaseApi: Record<string, any>;
  Login: {
    LoginAsync: string;
  };
  Menu: {
    GetMenusAsync: string;
    GetMenuByIdAsync: string;
    GetSidebarMenusAsync: string;
    GetTreeMenusAsync: string;
    GetPagedTreeMenusAsync: string;
    GetUserMenuTreeAsync: string;
    CreateMenuAsync: string;
    UpdateMenuAsync: string;
    DeleteMenuAsync: string;
    EnableMenuAsync: string;
    DisableMenuAsync: string;
  };
}

// ==================== API 配置对象 ====================
const api: ApiConfig = {
  ApiSearch: {},
  BaseApi: {},
  Login: {
    LoginAsync: 'api/Login/LoginAsync',
  },
  Menu: {
    GetMenusAsync: 'api/Menu/GetMenusAsync',
    GetMenuByIdAsync: 'api/Menu/GetMenuByIdAsync',
    GetSidebarMenusAsync: 'api/Menu/GetSidebarMenusAsync',
    GetTreeMenusAsync: 'api/Menu/GetTreeMenusAsync',
    GetPagedTreeMenusAsync: 'api/Menu/GetPagedTreeMenusAsync',
    GetUserMenuTreeAsync: 'api/Menu/GetUserMenuTreeAsync',
    CreateMenuAsync: 'api/Menu/CreateMenuAsync',
    UpdateMenuAsync: 'api/Menu/UpdateMenuAsync',
    DeleteMenuAsync: 'api/Menu/DeleteMenuAsync',
    EnableMenuAsync: 'api/Menu/EnableMenuAsync',
    DisableMenuAsync: 'api/Menu/DisableMenuAsync',
  },
};

export default api;
