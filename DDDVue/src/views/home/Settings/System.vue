<template>
  <div class="settings-page">
    <el-card class="settings-card">
      <el-tabs v-model="activeTab" class="settings-tabs">
        <!-- JWT 配置标签页 -->
        <el-tab-pane label="JWT 配置" name="jwt">
          <div class="tab-content">
            <el-form :model="jwtForm" label-width="120px" v-loading="loading">
              <el-form-item label="颁发者">
                <el-input v-model="jwtForm.issuer" placeholder="请输入颁发者" />
              </el-form-item>
              <el-form-item label="受众">
                <el-input v-model="jwtForm.audience" placeholder="请输入受众" />
              </el-form-item>
              <el-form-item label="密钥">
                <el-input v-model="jwtForm.key" placeholder="请输入密钥" show-password />
                <span class="form-tip">用于 JWT 签名的密钥，建议使用 32 位以上的随机字符串</span>
              </el-form-item>
              <el-form-item label="过期时间">
                <el-input-number v-model="jwtForm.expireMinutes" :min="1" :max="1440" />
                <span class="form-tip">分钟（1-1440）</span>
              </el-form-item>
              <el-form-item>
                <el-button v-if="hasPermission(PermissionCodes.SETTING_SAVE_JWT)" type="primary" @click="saveJwtSettings">保存配置</el-button>
              </el-form-item>
            </el-form>
          </div>
        </el-tab-pane>

        <!-- 系统配置标签页 -->
        <el-tab-pane label="系统配置" name="system">
          <div class="tab-content">
            <el-form :model="systemForm" label-width="120px" v-loading="loading">
              <el-form-item label="系统名称">
                <el-input v-model="systemForm.systemName" placeholder="请输入系统名称" />
              </el-form-item>
              <el-form-item label="系统描述">
                <el-input v-model="systemForm.systemDescription" type="textarea" :rows="3" placeholder="请输入系统描述" />
              </el-form-item>
              <el-form-item>
                <el-button v-if="hasPermission(PermissionCodes.SETTING_SAVE_SYSTEM)" type="primary" @click="saveSystemSettings">保存配置</el-button>
              </el-form-item>
            </el-form>
          </div>
        </el-tab-pane>

        <!-- 其他配置标签页（预留） -->
        <el-tab-pane label="其他配置" name="other">
          <div class="tab-content">
            <p class="placeholder-text">其他配置功能开发中...</p>
          </div>
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { showSuccessNotification, showErrorNotification } from '@/utils/notification'
import { hasPermission, PermissionCodes } from '@/utils/storage'
import { getAllSettings, batchUpdateSettings } from '@/api/role'
import type { SettingDto, UpdateSettingRequest } from '@/api/role'

// 当前激活的标签页
const activeTab = ref('jwt')
const loading = ref(false)

// JWT 配置表单
const jwtForm = ref({
  issuer: '',
  audience: '',
  key: '',
  expireMinutes: 720
})

// 系统配置表单
const systemForm = ref({
  systemName: '',
  systemDescription: ''
})

// 设置键名常量
const SETTING_KEYS = {
  // JWT 配置
  JWT_ISSUER: 'JwtSettings_Issuer',
  JWT_AUDIENCE: 'JwtSettings_Audience',
  JWT_KEY: 'JwtSettings_Key',
  JWT_EXPIRE_MINUTES: 'JwtSettings_ExpireMinutes',
  // 系统配置
  SYSTEM_NAME: 'SystemName',
  SYSTEM_DESCRIPTION: 'SystemDescription'
}

// 加载设置数据
const loadSettings = async () => {
  loading.value = true
  try {
    const response = await getAllSettings()
    if (response.success && response.data) {
      const settings = response.data as SettingDto[]
      settings.forEach(setting => {
        // JWT 配置
        if (setting.key === SETTING_KEYS.JWT_ISSUER) {
          jwtForm.value.issuer = setting.value
        } else if (setting.key === SETTING_KEYS.JWT_AUDIENCE) {
          jwtForm.value.audience = setting.value
        } else if (setting.key === SETTING_KEYS.JWT_KEY) {
          jwtForm.value.key = setting.value
        } else if (setting.key === SETTING_KEYS.JWT_EXPIRE_MINUTES) {
          jwtForm.value.expireMinutes = parseInt(setting.value) || 720
        }
        // 系统配置
        else if (setting.key === SETTING_KEYS.SYSTEM_NAME) {
          systemForm.value.systemName = setting.value
        } else if (setting.key === SETTING_KEYS.SYSTEM_DESCRIPTION) {
          systemForm.value.systemDescription = setting.value
        }
      })
    }
  } catch (error) {
    console.error('加载设置失败:', error)
    showErrorNotification({ title: '错误', message: '加载设置失败' })
  } finally {
    loading.value = false
  }
}

// 保存 JWT 配置
const saveJwtSettings = async () => {
  loading.value = true
  try {
    const settings: UpdateSettingRequest[] = [
      { key: SETTING_KEYS.JWT_ISSUER, value: jwtForm.value.issuer },
      { key: SETTING_KEYS.JWT_AUDIENCE, value: jwtForm.value.audience },
      { key: SETTING_KEYS.JWT_KEY, value: jwtForm.value.key },
      { key: SETTING_KEYS.JWT_EXPIRE_MINUTES, value: jwtForm.value.expireMinutes.toString() }
    ]

    const response = await batchUpdateSettings({ settings })
    if (response.success) {
      showSuccessNotification({ title: '成功', message: 'JWT 配置保存成功' })
      await loadSettings()  // 刷新数据
    } else {
      showErrorNotification({ title: '错误', message: response.message || '保存失败' })
    }
  } catch (error) {
    console.error('保存设置失败:', error)
    showErrorNotification({ title: '错误', message: '保存设置失败' })
  } finally {
    loading.value = false
  }
}

// 保存系统配置
const saveSystemSettings = async () => {
  loading.value = true
  try {
    const settings: UpdateSettingRequest[] = [
      { key: SETTING_KEYS.SYSTEM_NAME, value: systemForm.value.systemName },
      { key: SETTING_KEYS.SYSTEM_DESCRIPTION, value: systemForm.value.systemDescription }
    ]

    const response = await batchUpdateSettings({ settings })
    if (response.success) {
      showSuccessNotification({ title: '成功', message: '系统配置保存成功' })
      await loadSettings()  // 刷新数据
    } else {
      showErrorNotification({ title: '错误', message: response.message || '保存失败' })
    }
  } catch (error) {
    console.error('保存设置失败:', error)
    showErrorNotification({ title: '错误', message: '保存设置失败' })
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadSettings()
})
</script>

<style scoped>
.settings-page {
  padding: 20px;
}

.settings-card {
  border-radius: 8px;
}

.settings-tabs {
  padding: 0 20px;
}

.tab-content {
  padding: 20px 0;
}

.form-tip {
  margin-left: 10px;
  color: #909399;
  font-size: 12px;
}

.placeholder-text {
  text-align: center;
  color: #909399;
  padding: 40px;
}
</style>
