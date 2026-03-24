<template>
  <div class="profile-container">
    <el-card class="profile-card">
      <template #header>
        <span class="card-title">个人中心</span>
      </template>
      
      <div class="profile-content">
        <div class="avatar-section">
          <el-avatar :size="80" :src="avatarUrl">
            <span>{{ userInfo.realName?.[0] || userInfo.userName?.[0] || 'U' }}</span>
          </el-avatar>
          <div class="user-info">
            <h2 class="user-name">{{ userInfo.realName || userInfo.userName || '用户' }}</h2>
            <p class="user-id">ID: {{ userInfo.userId || 'N/A' }}</p>
          </div>
        </div>

        <el-divider />

        <el-form :model="userInfo" label-width="100px" class="form-section">
          <el-form-item label="用户名">
            <span>{{ userInfo.userName || '无' }}</span>
          </el-form-item>
          <el-form-item label="真实姓名">
            <span>{{ userInfo.realName || '无' }}</span>
          </el-form-item>
          <el-form-item label="用户ID">
            <span>{{ userInfo.userId || '无' }}</span>
          </el-form-item>
        </el-form>

        <el-divider />

        <div class="action-section">
          <el-button type="primary" @click="handleEditProfile">编辑资料</el-button>
          <el-button @click="handleChangePassword">修改密码</el-button>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { getItem, StorageKeys } from '@/utils/storage'

interface UserInfo {
  userId?: string
  userName?: string
  realName?: string
}

const userInfo = ref<UserInfo>({})

// 从 localStorage 获取用户信息
const getUserInfo = () => {
  const storedInfo = getItem<UserInfo>(StorageKeys.UserInfo)
  if (storedInfo) {
    userInfo.value = storedInfo
  }
}

const avatarUrl = computed(() => {
  // 可以根据用户信息生成头像 URL
  return ''
})

const handleEditProfile = () => {
  // TODO: 编辑资料逻辑
  console.log('编辑资料')
}

const handleChangePassword = () => {
  // TODO: 修改密码逻辑
  console.log('修改密码')
}

getUserInfo()
</script>

<style scoped>
.profile-container {
  max-width: 800px;
  margin: 0 auto;
}

.profile-card {
  margin-bottom: 20px;
}

.card-title {
  font-size: 18px;
  font-weight: 600;
}

.profile-content {
  padding: 20px 0;
}

.avatar-section {
  display: flex;
  align-items: center;
  gap: 20px;
  margin-bottom: 20px;
}

.user-info {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.user-name {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: #333;
}

.user-id {
  margin: 0;
  font-size: 14px;
  color: #999;
}

.form-section {
  margin-top: 20px;
}

.action-section {
  display: flex;
  gap: 10px;
  margin-top: 20px;
}
</style>
