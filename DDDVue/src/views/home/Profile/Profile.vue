<template>
  <div class="profile-container">
    <el-card class="profile-card">
      <template #header>
        <span class="card-title">个人中心</span>
      </template>
      
      <div class="profile-content">
        <!-- 头像区域 -->
        <div class="avatar-section">
          <el-avatar :size="100" :src="userInfo.avatar" class="user-avatar">
            <span class="avatar-text">{{ avatarText }}</span>
          </el-avatar>
          <div class="user-info">
            <h2 class="user-name">{{ userInfo.realName || userInfo.userName || '用户' }}</h2>
            <p class="user-id">
              <el-tag type="info" size="small">ID: {{ userInfo.id || 'N/A' }}</el-tag>
              <el-tag :type="userInfo.status === 1 ? 'success' : 'danger'" size="small" style="margin-left: 8px;">
                {{ userInfo.status === 1 ? '正常' : '禁用' }}
              </el-tag>
            </p>
          </div>
        </div>

        <el-divider />

        <!-- 基本信息 -->
        <div class="info-section">
          <h3 class="section-title">
            <el-icon><User /></el-icon>
            基本信息
          </h3>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="用户名">
              {{ userInfo.userName || '-' }}
            </el-descriptions-item>
            <el-descriptions-item label="真实姓名">
              {{ userInfo.realName || '-' }}
            </el-descriptions-item>
            <el-descriptions-item label="邮箱">
              {{ userInfo.email || '-' }}
            </el-descriptions-item>
            <el-descriptions-item label="手机号码">
              {{ userInfo.phoneNumber || '-' }}
            </el-descriptions-item>
            <el-descriptions-item label="用户ID">
              {{ userInfo.id || '-' }}
            </el-descriptions-item>
            <el-descriptions-item label="状态">
              <el-tag :type="userInfo.status === 1 ? 'success' : 'danger'" size="small">
                {{ userInfo.status === 1 ? '正常' : '禁用' }}
              </el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="角色" :span="2">
              <div class="role-tags">
                <el-tag v-if="userRoles.length === 0" type="info" size="small">未分配角色</el-tag>
                <el-tag
                  v-for="role in userRoles"
                  :key="role.id"
                  :type="role.code === 'SUPER_ADMIN' ? 'danger' : role.code === 'ADMIN' ? 'warning' : 'primary'"
                  size="small"
                  class="role-tag"
                >
                  {{ role.name }}
                </el-tag>
              </div>
            </el-descriptions-item>
          </el-descriptions>
        </div>

        <el-divider />

        <!-- 登录信息 -->
        <div class="info-section">
          <h3 class="section-title">
            <el-icon><Clock /></el-icon>
            登录信息
          </h3>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="最后登录时间">
              {{ formatDateTime(userInfo.lastLoginTime) }}
            </el-descriptions-item>
            <el-descriptions-item label="最后登录IP">
              {{ userInfo.lastLoginIp || '-' }}
            </el-descriptions-item>
            <el-descriptions-item label="创建时间">
              {{ formatDateTime(userInfo.createdAt) }}
            </el-descriptions-item>
            <el-descriptions-item label="更新时间">
              {{ formatDateTime(userInfo.updatedAt) }}
            </el-descriptions-item>
          </el-descriptions>
        </div>

        <el-divider />

        <!-- 操作区域 -->
        <div class="action-section">
          <el-button type="primary" :icon="Edit" @click="openEditDialog">编辑资料</el-button>
          <el-button :icon="Lock" @click="openPasswordDialog">修改密码</el-button>
          <el-button :icon="RefreshRight" @click="refreshUserInfo">刷新信息</el-button>
        </div>
      </div>
    </el-card>

    <!-- 编辑资料对话框 -->
    <el-dialog
      v-model="editDialogVisible"
      title="编辑资料"
      width="500px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="editFormRef"
        :model="editForm"
        :rules="editRules"
        label-width="100px"
      >
        <el-form-item label="用户名">
          <el-input v-model="userInfo.userName" disabled placeholder="用户名不可修改" />
        </el-form-item>
        <el-form-item label="真实姓名" prop="realName">
          <el-input v-model="editForm.realName" placeholder="请输入真实姓名" />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input v-model="editForm.email" placeholder="请输入邮箱" />
        </el-form-item>
        <el-form-item label="手机号码" prop="phoneNumber">
          <el-input v-model="editForm.phoneNumber" placeholder="请输入手机号码" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="editDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="editLoading" @click="handleUpdateProfile">保存</el-button>
      </template>
    </el-dialog>

    <!-- 修改密码对话框 -->
    <el-dialog
      v-model="passwordDialogVisible"
      title="修改密码"
      width="450px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="passwordFormRef"
        :model="passwordForm"
        :rules="passwordRules"
        label-width="100px"
      >
        <el-form-item label="原密码" prop="oldPassword">
          <el-input
            v-model="passwordForm.oldPassword"
            type="password"
            placeholder="请输入原密码"
            show-password
          />
        </el-form-item>
        <el-form-item label="新密码" prop="newPassword">
          <el-input
            v-model="passwordForm.newPassword"
            type="password"
            placeholder="请输入新密码（6-20位）"
            show-password
          />
        </el-form-item>
        <el-form-item label="确认密码" prop="confirmPassword">
          <el-input
            v-model="passwordForm.confirmPassword"
            type="password"
            placeholder="请再次输入新密码"
            show-password
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="passwordDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="passwordLoading" @click="handleChangePassword">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { type FormInstance, type FormRules } from 'element-plus'
import { User, Clock, Edit, Lock, RefreshRight } from '@element-plus/icons-vue'
import { getItem, setItem, StorageKeys, type UserDto } from '@/utils/storage'
import { updateProfile, changePassword, getUserById } from '@/api/user'
import { getUserRoles } from '@/api/role'
import type { RoleDto } from '@/api/index'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'
import { aesEncrypt } from '@/utils/crypto'

const router = useRouter()

// 用户信息
const userInfo = ref<UserDto>({
  id: '',
  userName: '',
  email: '',
  phoneNumber: '',
  realName: '',
  avatar: '',
  status: 1,
  lastLoginTime: '',
  lastLoginIp: '',
  remark: '',
  createdAt: '',
  updatedAt: ''
})

// 用户角色列表
const userRoles = ref<RoleDto[]>([])

// 头像文字
const avatarText = computed(() => {
  return userInfo.value.realName?.[0] || userInfo.value.userName?.[0] || 'U'
})

// 编辑资料对话框
const editDialogVisible = ref(false)
const editLoading = ref(false)
const editFormRef = ref<FormInstance>()
const editForm = ref({
  realName: '',
  email: '',
  phoneNumber: ''
})

const editRules: FormRules = {
  email: [
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  phoneNumber: [
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ]
}

// 修改密码对话框
const passwordDialogVisible = ref(false)
const passwordLoading = ref(false)
const passwordFormRef = ref<FormInstance>()
const passwordForm = ref({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const validateConfirmPassword = (_rule: any, value: string, callback: any) => {
  if (value !== passwordForm.value.newPassword) {
    callback(new Error('两次输入的密码不一致'))
  } else {
    callback()
  }
}

const passwordRules: FormRules = {
  oldPassword: [
    { required: true, message: '请输入原密码', trigger: 'blur' }
  ],
  newPassword: [
    { required: true, message: '请输入新密码', trigger: 'blur' },
    { min: 6, max: 20, message: '密码长度在 6 到 20 个字符', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '请再次输入新密码', trigger: 'blur' },
    { validator: validateConfirmPassword, trigger: 'blur' }
  ]
}

// 格式化日期时间
const formatDateTime = (dateStr?: string) => {
  if (!dateStr) return '-'
  try {
    const date = new Date(dateStr)
    return date.toLocaleString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    })
  } catch {
    return dateStr
  }
}

// 从 localStorage 获取用户信息
const getUserInfo = async () => {
  const storedInfo = getItem<UserDto>(StorageKeys.UserInfo)
  if (storedInfo && storedInfo.id) {
    // 尝试从服务器获取最新的用户信息
    try {
      const response = await getUserById(storedInfo.id)
      if (response.data) {
        userInfo.value = response.data
        // 更新本地存储
        setItem(StorageKeys.UserInfo, {
          id: response.data.id,
          userName: response.data.userName,
          realName: response.data.realName,
          email: response.data.email,
          phoneNumber: response.data.phoneNumber,
          avatar: response.data.avatar,
          status: response.data.status
        })

        // 获取用户角色
        const rolesResponse = await getUserRoles(response.data.id)
        if (rolesResponse.data) {
          userRoles.value = rolesResponse.data
        }
      } else {
        // 如果获取失败，使用本地存储的信息
        userInfo.value = {
          id: storedInfo.id || '',
          userName: storedInfo.userName || '',
          realName: storedInfo.realName || '',
          email: storedInfo.email || '',
          phoneNumber: storedInfo.phoneNumber || '',
          avatar: storedInfo.avatar || '',
          status: storedInfo.status || 1,
          createdAt: '',
          updatedAt: ''
        }

        // 尝试获取用户角色
        try {
          const rolesResponse = await getUserRoles(storedInfo.id)
          if (rolesResponse.data) {
            userRoles.value = rolesResponse.data
          }
        } catch {
          userRoles.value = []
        }
      }
    } catch {
      // 网络错误时使用本地存储的信息
      userInfo.value = {
        id: storedInfo.id || '',
        userName: storedInfo.userName || '',
        realName: storedInfo.realName || '',
        email: storedInfo.email || '',
        phoneNumber: storedInfo.phoneNumber || '',
        avatar: storedInfo.avatar || '',
        status: storedInfo.status || 1,
        createdAt: '',
        updatedAt: ''
      }
      userRoles.value = []
    }
  }
}

// 刷新用户信息
const refreshUserInfo = async () => {
  await getUserInfo()
  showSuccessNotification({ title: '成功', message: '用户信息已刷新' })
}

// 打开编辑资料对话框
const openEditDialog = () => {
  editForm.value = {
    realName: userInfo.value.realName || '',
    email: userInfo.value.email || '',
    phoneNumber: userInfo.value.phoneNumber || ''
  }
  editDialogVisible.value = true
}

// 更新资料
const handleUpdateProfile = async () => {
  if (!editFormRef.value) return
  
  await editFormRef.value.validate(async (valid) => {
    if (!valid) return
    
    editLoading.value = true
    try {
      const response = await updateProfile(editForm.value)
      if (response.success) {
        showSuccessNotification({ title: '成功', message: '资料更新成功' })
        editDialogVisible.value = false
        // 更新本地用户信息
        if (response.data) {
          userInfo.value = response.data
          setItem(StorageKeys.UserInfo, {
            id: response.data.id,
            userName: response.data.userName,
            realName: response.data.realName,
            email: response.data.email,
            phoneNumber: response.data.phoneNumber,
            avatar: response.data.avatar,
            status: response.data.status
          })
        } else {
          await getUserInfo()
        }
      } else {
        showErrorNotification({ title: '失败', message: response.message || '更新资料失败' })
      }
    } catch (error: any) {
      showErrorNotification({ title: '错误', message: error.message || '更新资料失败' })
    } finally {
      editLoading.value = false
    }
  })
}

// 打开修改密码对话框
const openPasswordDialog = () => {
  passwordForm.value = {
    oldPassword: '',
    newPassword: '',
    confirmPassword: ''
  }
  passwordDialogVisible.value = true
}

// 修改密码
const handleChangePassword = async () => {
  if (!passwordFormRef.value) return
  
  await passwordFormRef.value.validate(async (valid) => {
    if (!valid) return
    
    passwordLoading.value = true
    try {
      const response = await changePassword({
        oldPassword: aesEncrypt(passwordForm.value.oldPassword),
        newPassword: aesEncrypt(passwordForm.value.newPassword)
      })
      if (response.success) {
        showSuccessNotification({ title: '成功', message: '密码修改成功，请重新登录' })
        passwordDialogVisible.value = false
        // 清除登录信息，跳转到登录页
        setTimeout(() => {
          localStorage.clear()
          router.push('/')
        }, 1500)
      } else {
        showErrorNotification({ title: '失败', message: response.message || '修改密码失败' })
      }
    } catch (error: any) {
      showErrorNotification({ title: '错误', message: error.message || '修改密码失败' })
    } finally {
      passwordLoading.value = false
    }
  })
}

onMounted(() => {
  getUserInfo()
})
</script>

<style scoped>
.profile-container {
  max-width: 900px;
  margin: 0 auto;
  padding: 20px;
}

.profile-card {
  border-radius: 8px;
}

.card-title {
  font-size: 18px;
  font-weight: 600;
}

.profile-content {
  padding: 20px 0;
}

/* 头像区域 */
.avatar-section {
  display: flex;
  align-items: center;
  gap: 24px;
  margin-bottom: 20px;
}

.user-avatar {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.avatar-text {
  font-size: 36px;
  font-weight: 600;
  color: white;
}

.user-info {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.user-name {
  margin: 0;
  font-size: 24px;
  font-weight: 600;
  color: #303133;
  text-align: left;
}

.user-id {
  margin: 0;
  display: flex;
  align-items: center;
}

/* 信息区域 */
.info-section {
  margin-top: 20px;
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  margin-bottom: 16px;
  display: flex;
  align-items: center;
  gap: 8px;
}

/* 操作区域 */
.action-section {
  display: flex;
  gap: 12px;
  margin-top: 20px;
}

/* 角色标签样式 */
.role-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.role-tag {
  margin: 0;
}

/* 描述列表样式 */
:deep(.el-descriptions) {
  margin-bottom: 10px;
}

:deep(.el-descriptions__label) {
  font-weight: 500;
  color: #606266;
}

/* 响应式 */
@media (max-width: 768px) {
  .profile-container {
    padding: 10px;
  }
  
  .avatar-section {
    flex-direction: column;
    text-align: center;
  }
  
  :deep(.el-descriptions) {
    --el-descriptions-table-border: none;
  }
  
  :deep(.el-descriptions__body) {
    .el-descriptions__table {
      display: block;
      
      .el-descriptions__cell {
        display: block;
        width: 100%;
        padding: 8px 0;
      }
    }
  }
}
</style>