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

// ==================== 操作日志模块 ====================
export interface OperationLogDto {
  id: string;
  userId: string;
  userName: string;
  realName?: string;
  operationType: string;
  module: string;
  description?: string;
  requestMethod: string;
  requestPath: string;
  requestParams?: string;
  responseResult?: string;
  ipAddress?: string;
  status: string;
  errorMessage?: string;
  duration: number;
  browser?: string;
  osInfo?: string;
  createdAt: string;
}

export interface OperationLogQueryRequest {
  pageNum: number;
  pageSize: number;
  operationType?: string;
  module?: string;
  status?: string;
  userName?: string;
  startTime?: string;
  endTime?: string;
}

export interface ClearLogsRequest {
  startTime?: string;
  endTime?: string;
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

// ==================== 字典模块 ====================
export interface DictionaryDto {
  id: string;
  code: string;
  name: string;
  value: string;
  type: string;
  status: number;
  sortOrder: number;
  description?: string;
  remark?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface DictionaryQueryRequest {
  pageNum: number;
  pageSize: number;
  code?: string;
  name?: string;
  type?: string;
  status?: number;
}

export interface CreateDictionaryRequest {
  code: string;
  name: string;
  value: string;
  type: string;
  sortOrder?: number;
  description?: string;
  remark?: string;
}

export interface UpdateDictionaryRequest {
  id: string;
  name?: string;
  value?: string;
  type?: string;
  sortOrder?: number;
  description?: string;
  remark?: string;
  status?: number;
}

// ==================== 站内信模块 ====================
export interface MessageDto {
  id: string;
  senderId?: string;
  senderName?: string;
  receiverId: string;
  receiverName: string;
  title: string;
  content: string;
  messageType: string;
  priority: string;
  isRead: boolean;
  readTime?: string;
  isPushed: boolean;
  createdAt: string;
}

// 用户消息视图 DTO（用于用户查看自己的消息）
export interface UserMessageDto {
  recipientId: string;      // 接收者记录ID
  messageId: string;        // 消息ID
  senderId?: string;
  senderName?: string;
  title: string;
  content: string;
  messageType: string;
  priority: string;
  isRead: boolean;
  readTime?: string;
  isPushed: boolean;
  createdAt: string;
  hasBeenReadByOthers?: boolean;  // 是否有其他用户已读（用于判断是否可删除）
  isRevoked?: boolean;            // 是否已撤回
  revokedTime?: string;           // 撤回时间
}

export interface MessageQueryRequest {
  pageNum: number;
  pageSize: number;
  onlyUnread?: boolean;
  messageType?: string;
  priority?: string;
  keyword?: string;
  senderName?: string;
}

export interface CreateMessageRequest {
  receiverId: string;
  receiverName: string;
  title: string;
  content: string;
  messageType?: string;
  priority?: string;
}

export interface BatchSendMessageRequest {
  receiverIds: string[];
  title: string;
  content: string;
  priority?: string;
}

export interface MessageStatisticsDto {
  totalCount: number;
  unreadCount: number;
  systemMessageCount: number;
  userMessageCount: number;
}

export interface UpdateMessageRequest {
  title: string;
  content: string;
  priority?: string;
}

export interface PushMessageRequest {
  title: string;
  content: string;
  priority?: string;
}

export interface PushMessageToRoleRequest {
  roleIds: string[];
  title: string;
  content: string;
  priority?: string;
}

export interface PushExistingMessageRequest {
  pushType: 'all' | 'role' | 'user';
  roleIds?: string[];
  userIds?: string[];
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
    GetPublicSettingsAsync: string;
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
  OperationLog: {
    GetOperationLogsAsync: string;
    GetOperationLogByIdAsync: string;
    DeleteOperationLogAsync: string;
    BatchDeleteOperationLogsAsync: string;
    ClearOperationLogsAsync: string;
    GetOperationTypeStatisticsAsync: string;
    GetModuleStatisticsAsync: string;
    ExportOperationLogsAsync: string;
  };
  Dictionary: {
    GetDictionariesAsync: string;
    GetDictionaryByIdAsync: string;
    GetDictionaryByCodeAsync: string;
    GetDictionariesByTypeAsync: string;
    GetDictionariesByTypesAsync: string;
    CreateDictionaryAsync: string;
    UpdateDictionaryAsync: string;
    DeleteDictionaryAsync: string;
    EnableDictionaryAsync: string;
    DisableDictionaryAsync: string;
  };
  Message: {
    GetMessagesAsync: string;
    GetMessageByIdAsync: string;
    SendMessageAsync: string;
    SendSystemMessageAsync: string;
    BatchSendSystemMessageAsync: string;
    MarkAsReadAsync: string;
    MarkAsUnreadAsync: string;
    BatchMarkAsReadAsync: string;
    MarkAllAsReadAsync: string;
    DeleteMessageAsync: string;
    BatchDeleteMessagesAsync: string;
    GetStatisticsAsync: string;
    GetUnreadCountAsync: string;
    UpdateMessageAsync: string;
    PushMessageToAllAsync: string;
    PushMessageToRoleAsync: string;
    PushExistingMessageAsync: string;
    GetUserMessageByIdAsync: string;
    MarkUserMessageAsReadAsync: string;
    DeleteUserMessageAsync: string;
    BatchDeleteUserMessagesAsync: string;
    RevokeMessageAsync: string;
    BatchRevokeMessagesAsync: string;
  };
}

// ==================== API 配置对象 ====================
const api: ApiConfig = {
  ApiSearch: {},
  BaseApi: {},
  Login: {
    LoginAsync: 'api/Login/LoginAsync',
    GetPublicSettingsAsync: 'api/Login/GetPublicSettingsAsync',
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
  OperationLog: {
    GetOperationLogsAsync: 'api/OperationLog/GetOperationLogsAsync',
    GetOperationLogByIdAsync: 'api/OperationLog/GetOperationLogByIdAsync',
    DeleteOperationLogAsync: 'api/OperationLog/DeleteOperationLogAsync',
    BatchDeleteOperationLogsAsync: 'api/OperationLog/BatchDeleteOperationLogsAsync',
    ClearOperationLogsAsync: 'api/OperationLog/ClearOperationLogsAsync',
    GetOperationTypeStatisticsAsync: 'api/OperationLog/GetOperationTypeStatisticsAsync',
    GetModuleStatisticsAsync: 'api/OperationLog/GetModuleStatisticsAsync',
    ExportOperationLogsAsync: 'api/OperationLog/ExportOperationLogsAsync',
  },
  Dictionary: {
    GetDictionariesAsync: 'api/Dictionary/GetDictionariesAsync',
    GetDictionaryByIdAsync: 'api/Dictionary/GetDictionaryByIdAsync',
    GetDictionaryByCodeAsync: 'api/Dictionary/GetDictionaryByCodeAsync',
    GetDictionariesByTypeAsync: 'api/Dictionary/GetDictionariesByTypeAsync',
    GetDictionariesByTypesAsync: 'api/Dictionary/GetDictionariesByTypesAsync',
    CreateDictionaryAsync: 'api/Dictionary/CreateDictionaryAsync',
    UpdateDictionaryAsync: 'api/Dictionary/UpdateDictionaryAsync',
    DeleteDictionaryAsync: 'api/Dictionary/DeleteDictionaryAsync',
    EnableDictionaryAsync: 'api/Dictionary/EnableDictionaryAsync',
    DisableDictionaryAsync: 'api/Dictionary/DisableDictionaryAsync',
  },
  Message: {
    GetMessagesAsync: 'api/Message/GetMessagesAsync',
    GetMessageByIdAsync: 'api/Message/GetMessageByIdAsync',
    SendMessageAsync: 'api/Message/SendMessageAsync',
    SendSystemMessageAsync: 'api/Message/SendSystemMessageAsync',
    BatchSendSystemMessageAsync: 'api/Message/BatchSendSystemMessageAsync',
    MarkAsReadAsync: 'api/Message/MarkAsReadAsync',
    MarkAsUnreadAsync: 'api/Message/MarkAsUnreadAsync',
    BatchMarkAsReadAsync: 'api/Message/BatchMarkAsReadAsync',
    MarkAllAsReadAsync: 'api/Message/MarkAllAsReadAsync',
    DeleteMessageAsync: 'api/Message/DeleteMessageAsync',
    BatchDeleteMessagesAsync: 'api/Message/BatchDeleteMessagesAsync',
    GetStatisticsAsync: 'api/Message/GetStatisticsAsync',
    GetUnreadCountAsync: 'api/Message/GetUnreadCountAsync',
    UpdateMessageAsync: 'api/Message/UpdateMessageAsync',
    PushMessageToAllAsync: 'api/Message/PushMessageToAllAsync',
    PushMessageToRoleAsync: 'api/Message/PushMessageToRoleAsync',
    PushExistingMessageAsync: 'api/Message/PushExistingMessageAsync',
    GetUserMessageByIdAsync: 'api/Message/GetUserMessageByIdAsync',
    MarkUserMessageAsReadAsync: 'api/Message/MarkUserMessageAsReadAsync',
    DeleteUserMessageAsync: 'api/Message/DeleteUserMessageAsync',
    BatchDeleteUserMessagesAsync: 'api/Message/BatchDeleteUserMessagesAsync',
    RevokeMessageAsync: 'api/Message/RevokeMessageAsync',
    BatchRevokeMessagesAsync: 'api/Message/BatchRevokeMessagesAsync',
  },
};

export default api;
