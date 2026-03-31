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

export interface UpdateProfileRequest {
  email?: string;
  phoneNumber?: string;
  realName?: string;
  avatar?: string;
}

export interface ChangePasswordRequest {
  oldPassword: string;
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

// ==================== 系统设置模块 ====================
export interface SettingDto {
  id: string;
  key: string;
  value: string;
  description?: string;
  group: string;
  createdAt: string;
  updatedAt?: string;
}

export interface UpdateSettingRequest {
  key: string;
  value: string;
}

export interface BatchUpdateSettingsRequest {
  settings: UpdateSettingRequest[];
}

// ==================== 权限模块 ====================
export interface PermissionDto {
  id: string;
  code: string;
  name: string;
  description?: string;
  module: string;
  menuId?: string;
  sortOrder: number;
  status: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreatePermissionRequest {
  code: string;
  name: string;
  description?: string;
  module: string;
  menuId?: string;
  sortOrder: number;
}

export interface UpdatePermissionRequest {
  id: string;
  name?: string;
  description?: string;
  sortOrder?: number;
}

export interface AssignRolePermissionsRequest {
  roleId: string;
  permissionIds: string[];
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
    UpdateProfileAsync: string;
    ChangePasswordAsync: string;
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
    GetUserRolesAsync: string;
    AssignUserRolesAsync: string;
    GetEnabledRolesAsync: string;
    GetRoleUserIdsAsync: string;
    AssignRoleUsersAsync: string;
  };
  MenuRole: {
    GetRoleMenuIdsAsync: string;
    GetMenuRoleIdsAsync: string;
    AssignRoleMenusAsync: string;
    AssignMenuRolesAsync: string;
    GetRoleMenusAsync: string;
    GetUserMenusByRolesAsync: string;
    ClearRoleMenusAsync: string;
    ClearMenuRolesAsync: string;
  };
  Setting: {
    GetAllSettingsAsync: string;
    GetSettingsByGroupAsync: string;
    GetSettingByKeyAsync: string;
    UpdateSettingAsync: string;
    BatchUpdateSettingsAsync: string;
  };
  Permission: {
    GetPermissionsAsync: string;
    GetAllEnabledPermissionsAsync: string;
    GetPermissionsByModuleAsync: string;
    GetPermissionByIdAsync: string;
    CreatePermissionAsync: string;
    UpdatePermissionAsync: string;
    DeletePermissionAsync: string;
    EnablePermissionAsync: string;
    DisablePermissionAsync: string;
    GetRolePermissionIdsAsync: string;
    AssignRolePermissionsAsync: string;
    GetUserPermissionsAsync: string;
    HasPermissionAsync: string;
  };
  Button: {
    GetButtonsAsync: string;
    GetButtonByIdAsync: string;
    GetButtonsByMenuIdAsync: string;
    CreateButtonAsync: string;
    UpdateButtonAsync: string;
    DeleteButtonAsync: string;
    EnableButtonAsync: string;
    DisableButtonAsync: string;
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
    UpdateProfileAsync: 'api/User/UpdateProfileAsync',
    ChangePasswordAsync: 'api/User/ChangePasswordAsync',
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
    GetUserRolesAsync: 'api/Role/GetUserRolesAsync',
    AssignUserRolesAsync: 'api/Role/AssignUserRolesAsync',
    GetEnabledRolesAsync: 'api/Role/GetEnabledRolesAsync',
    GetRoleUserIdsAsync: 'api/Role/GetRoleUserIdsAsync',
    AssignRoleUsersAsync: 'api/Role/AssignRoleUsersAsync',
  },
  MenuRole: {
    GetRoleMenuIdsAsync: 'api/MenuRole/GetRoleMenuIdsAsync',
    GetMenuRoleIdsAsync: 'api/MenuRole/GetMenuRoleIdsAsync',
    AssignRoleMenusAsync: 'api/MenuRole/AssignRoleMenusAsync',
    AssignMenuRolesAsync: 'api/MenuRole/AssignMenuRolesAsync',
    GetRoleMenusAsync: 'api/MenuRole/GetRoleMenusAsync',
    GetUserMenusByRolesAsync: 'api/MenuRole/GetUserMenusByRolesAsync',
    ClearRoleMenusAsync: 'api/MenuRole/ClearRoleMenusAsync',
    ClearMenuRolesAsync: 'api/MenuRole/ClearMenuRolesAsync',
  },
  Setting: {
    GetAllSettingsAsync: 'api/Setting/GetAllSettingsAsync',
    GetSettingsByGroupAsync: 'api/Setting/GetSettingsByGroupAsync',
    GetSettingByKeyAsync: 'api/Setting/GetSettingByKeyAsync',
    UpdateSettingAsync: 'api/Setting/UpdateSettingAsync',
    BatchUpdateSettingsAsync: 'api/Setting/BatchUpdateSettingsAsync',
  },
  Permission: {
    GetPermissionsAsync: 'api/Permission/GetPermissionsAsync',
    GetAllEnabledPermissionsAsync: 'api/Permission/GetAllEnabledPermissionsAsync',
    GetPermissionsByModuleAsync: 'api/Permission/GetPermissionsByModuleAsync',
    GetPermissionByIdAsync: 'api/Permission/GetPermissionByIdAsync',
    CreatePermissionAsync: 'api/Permission/CreatePermissionAsync',
    UpdatePermissionAsync: 'api/Permission/UpdatePermissionAsync',
    DeletePermissionAsync: 'api/Permission/DeletePermissionAsync',
    EnablePermissionAsync: 'api/Permission/EnablePermissionAsync',
    DisablePermissionAsync: 'api/Permission/DisablePermissionAsync',
    GetRolePermissionIdsAsync: 'api/Permission/GetRolePermissionIdsAsync',
    AssignRolePermissionsAsync: 'api/Permission/AssignRolePermissionsAsync',
    GetUserPermissionsAsync: 'api/Permission/GetUserPermissionsAsync',
    HasPermissionAsync: 'api/Permission/HasPermissionAsync',
  },
  Button: {
    GetButtonsAsync: 'api/Button/GetButtonsAsync',
    GetButtonByIdAsync: 'api/Button/GetButtonByIdAsync',
    GetButtonsByMenuIdAsync: 'api/Button/GetButtonsByMenuIdAsync',
    CreateButtonAsync: 'api/Button/CreateButtonAsync',
    UpdateButtonAsync: 'api/Button/UpdateButtonAsync',
    DeleteButtonAsync: 'api/Button/DeleteButtonAsync',
    EnableButtonAsync: 'api/Button/EnableButtonAsync',
    DisableButtonAsync: 'api/Button/DisableButtonAsync',
  },
};

export default api;
