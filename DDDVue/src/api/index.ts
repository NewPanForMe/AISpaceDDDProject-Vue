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

// ==================== 用户模块 ====================
export interface UserDto {
  id: string;
  userName: string;
  email: string;
  phoneNumber?: string;
  realName?: string;
  avatar?: string;
  status: number;
  lastLoginTime?: string;
  lastLoginIp?: string;
  remark?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUserRequest {
  userName: string;
  email: string;
  password: string;
  realName?: string;
  phoneNumber?: string;
  avatar?: string;
  remark?: string;
}

export interface UpdateUserRequest {
  id: string;
  email?: string;
  phoneNumber?: string;
  realName?: string;
  avatar?: string;
  remark?: string;
  status?: number;
}

export interface ResetPasswordRequest {
  id: string;
  newPassword: string;
}

// ==================== 角色模块 ====================
export interface RoleDto {
  id: string;
  name: string;
  code: string;
  description?: string;
  status: number;
  sortOrder: number;
  remark?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateRoleRequest {
  name: string;
  code: string;
  description?: string;
  sortOrder: number;
  remark?: string;
}

export interface UpdateRoleRequest {
  id: string;
  name?: string;
  code?: string;
  description?: string;
  sortOrder?: number;
  remark?: string;
  status?: number;
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
    GetRoutesAsync: string;
    CreateMenuAsync: string;
    UpdateMenuAsync: string;
    DeleteMenuAsync: string;
    EnableMenuAsync: string;
    DisableMenuAsync: string;
  };
  User: {
    GetUsersAsync: string;
    GetUserByIdAsync: string;
    CreateUserAsync: string;
    UpdateUserAsync: string;
    DeleteUserAsync: string;
    EnableUserAsync: string;
    DisableUserAsync: string;
    ResetPasswordAsync: string;
  };
  Role: {
    GetRolesAsync: string;
    GetRoleByIdAsync: string;
    CreateRoleAsync: string;
    UpdateRoleAsync: string;
    DeleteRoleAsync: string;
    EnableRoleAsync: string;
    DisableRoleAsync: string;
    GetUserRoleIdsAsync: string;
    AssignUserRolesAsync: string;
    GetEnabledRolesAsync: string;
    GetRoleUserIdsAsync: string;
    AssignRoleUsersAsync: string;
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
    GetRoutesAsync: 'api/Menu/GetRoutesAsync',
    CreateMenuAsync: 'api/Menu/CreateMenuAsync',
    UpdateMenuAsync: 'api/Menu/UpdateMenuAsync',
    DeleteMenuAsync: 'api/Menu/DeleteMenuAsync',
    EnableMenuAsync: 'api/Menu/EnableMenuAsync',
    DisableMenuAsync: 'api/Menu/DisableMenuAsync',
  },
  User: {
    GetUsersAsync: 'api/User/GetUsersAsync',
    GetUserByIdAsync: 'api/User/GetUserByIdAsync',
    CreateUserAsync: 'api/User/CreateUserAsync',
    UpdateUserAsync: 'api/User/UpdateUserAsync',
    DeleteUserAsync: 'api/User/DeleteUserAsync',
    EnableUserAsync: 'api/User/EnableUserAsync',
    DisableUserAsync: 'api/User/DisableUserAsync',
    ResetPasswordAsync: 'api/User/ResetPasswordAsync',
  },
  Role: {
    GetRolesAsync: 'api/Role/GetRolesAsync',
    GetRoleByIdAsync: 'api/Role/GetRoleByIdAsync',
    CreateRoleAsync: 'api/Role/CreateRoleAsync',
    UpdateRoleAsync: 'api/Role/UpdateRoleAsync',
    DeleteRoleAsync: 'api/Role/DeleteRoleAsync',
    EnableRoleAsync: 'api/Role/EnableRoleAsync',
    DisableRoleAsync: 'api/Role/DisableRoleAsync',
    GetUserRoleIdsAsync: 'api/Role/GetUserRoleIdsAsync',
    AssignUserRolesAsync: 'api/Role/AssignUserRolesAsync',
    GetEnabledRolesAsync: 'api/Role/GetEnabledRolesAsync',
    GetRoleUserIdsAsync: 'api/Role/GetRoleUserIdsAsync',
    AssignRoleUsersAsync: 'api/Role/AssignRoleUsersAsync',
  },
};

export default api;
