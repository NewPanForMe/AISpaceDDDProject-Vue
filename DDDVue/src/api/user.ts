import { http } from '@/utils/http'
import api from '@/api/index'
import type { PagedResult, UserDto, CreateUserRequest, UpdateUserRequest, ResetPasswordRequest } from '@/api/index'

// 分页参数接口
export interface PageParams {
  pageNum: number
  pageSize: number
  [key: string]: any
}

// 获取用户列表（分页）
export const getUsers = (params?: PageParams) => {
  return http.get<PagedResult<UserDto>>(api.User.GetUsersAsync, { params })
}

// 根据ID获取用户详情
export const getUserById = (id: string) => {
  return http.get<UserDto>(api.User.GetUserByIdAsync, { params: { id } })
}

// 创建用户
export const createUser = (data: CreateUserRequest) => {
  return http.post(api.User.CreateUserAsync, data)
}

// 更新用户
export const updateUser = (data: UpdateUserRequest) => {
  return http.put(api.User.UpdateUserAsync, data)
}

// 删除用户
export const deleteUser = (id: string) => {
  return http.delete(api.User.DeleteUserAsync, { params: { id } })
}

// 启用用户
export const enableUser = (id: string) => {
  return http.post(api.User.EnableUserAsync, {}, { params: { id } })
}

// 禁用用户
export const disableUser = (id: string) => {
  return http.post(api.User.DisableUserAsync, {}, { params: { id } })
}

// 重置密码
export const resetPassword = (data: ResetPasswordRequest) => {
  return http.post(api.User.ResetPasswordAsync, data)
}