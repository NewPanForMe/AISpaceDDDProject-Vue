<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import {
  DataLine,
  Lock,
  TrendCharts,
  User,
  Management,
  DataAnalysis,
  CircleCheck,
  ChatDotRound
} from '@element-plus/icons-vue'
import { http } from '../utils/http'
import api from '../api/index'
import { showSuccessNotification, showErrorNotification } from '../utils/notification'
import { aesEncrypt } from '../utils/crypto'
import { setItem, removeItem, StorageKeys, setUserPermissions, setSystemName, getSystemName } from '@/utils/storage'
import { initRouteConfig } from '@/router/detail'
import { getAllSettings, getPublicSettings } from '@/api/role'
import type { SettingDto } from '@/api/role'



// 登录响应数据定义
interface LoginResponse {
  success: boolean
  message?: string
  data?: {
    token?: string
    userId?: string
    userName?: string
    realName?: string
  }
}

// 注册响应数据定义
interface RegisterResponse {
  success: boolean
  message?: string
  data?: {
    id?: string
    userName?: string
  }
}

// 图标组件映射
const iconComponents = {
  Management,
  DataAnalysis,
  CircleCheck,
  ChatDotRound
}

const router = useRouter()

// 系统名称（从 localStorage 读取）
const systemName = ref(getSystemName())

const formRef = ref()
const registerFormRef = ref()
const forgotPasswordFormRef = ref()

// 当前模式：'login' | 'register' | 'forgot'
const currentMode = ref<'login' | 'register' | 'forgot'>('login')

// 登录表单
const form = ref({
  userName: '',
  password: ''
})

// 注册表单
const registerForm = ref({
  userName: '',
  password: '',
  confirmPassword: '',
  email: '',
  realName: ''
})

// 忘记密码表单
const forgotPasswordForm = ref({
  userName: '',
  email: '',
  newPassword: '',
  confirmPassword: ''
})

const rememberMe = ref(false)
const loading = ref(false)
const registerLoading = ref(false)
const forgotPasswordLoading = ref(false)

// 左侧品牌文案
const brandTexts = ref([
  { title: '智能管理', icon: 'Management', desc: '统一管理所有资源' },
  { title: '数据分析', icon: 'DataAnalysis', desc: '实时数据可视化' },
  { title: '安全保障', icon: 'CircleCheck', desc: '多重安全防护' },
  { title: '高效协作', icon: 'ChatDotRound', desc: '团队协同办公' }
])

const currentTextIndex = ref(0)

// 获取图标组件
const getIconComponent = (iconName: string) => {
  return iconComponents[iconName as keyof typeof iconComponents]
}

const rules = {
  userName: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 5, max: 20, message: '长度在 5 到 20 个字符', trigger: 'blur' }
  ]
}

// 注册表单验证规则
const registerRules = {
  userName: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' },
    { pattern: /^[a-zA-Z0-9_]+$/, message: '用户名只能包含字母、数字和下划线', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' },
    { pattern: /^(?=.*[a-zA-Z])(?=.*\d)/, message: '密码必须包含字母和数字', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '请确认密码', trigger: 'blur' },
    {
      validator: (_rule: any, value: string, callback: Function) => {
        if (value !== registerForm.value.password) {
          callback(new Error('两次输入的密码不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  realName: [
    { max: 20, message: '长度不超过 20 个字符', trigger: 'blur' }
  ]
}

// 忘记密码表单验证规则
const forgotPasswordRules = {
  userName: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  newPassword: [
    { required: true, message: '请输入新密码', trigger: 'blur' },
    { min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' },
    { pattern: /^(?=.*[a-zA-Z])(?=.*\d)/, message: '密码必须包含字母和数字', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '请确认新密码', trigger: 'blur' },
    {
      validator: (_rule: any, value: string, callback: Function) => {
        if (value !== forgotPasswordForm.value.newPassword) {
          callback(new Error('两次输入的密码不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

const handleSubmit = async () => {
  if (!formRef.value) return

  loading.value = true
  try {
    // 验证表单
    await formRef.value.validate((valid: boolean) => {
      if (!valid) {
        throw new Error('表单验证失败')
      }
    })

    // 调用登录接口
    const response = await http.post<LoginResponse>(api.Login.LoginAsync, {
      userName: form.value.userName,
      password: aesEncrypt(form.value.password)
    })
    // 处理登录结果
    if (response.success) {

      // 清除旧的菜单缓存（确保不同用户看到不同的菜单）
      removeItem(StorageKeys.SidebarMenu)

      // 保存 token
      const token = response.data?.token || ''
      if (token) {
        setItem(StorageKeys.Token, token)
      }

      // 保存用户信息
      const userInfo = {
        userId: response.data?.userId,
        userName: response.data?.userName,
        realName: response.data?.realName
      }
      if (response.data?.userName) {
        setItem(StorageKeys.UserInfo, userInfo)
      }

      // 获取用户权限
      try {
        const permissionResponse = await http.get<{ success: boolean; data: { code: string }[] }>(api.Permission.GetUserPermissionsAsync, {
          params: { userId: response.data?.userId }
        })
        // 响应拦截器返回的是 ApiRequestResult 结构
        if (Array.isArray(permissionResponse.data)) {
          // 从 PermissionDto[] 中提取 code 字段
          const permissionCodes = permissionResponse.data.map(p => p.code)
          setUserPermissions(permissionCodes)
        }
      } catch (permError) {
        console.error('获取用户权限失败:', permError)
        // 权限获取失败不影响登录流程
      }

      // 获取系统配置并存入缓存
      try {
        const settingsResponse = await getAllSettings()
        if (settingsResponse.data) {
          const settings = settingsResponse.data as SettingDto[]
          settings.forEach(setting => {
            // 系统名称
            if (setting.key === 'SystemName' && setting.value) {
              setSystemName(setting.value)
            }
            // 系统描述
            if (setting.key === 'SystemDescription' && setting.value) {
              setItem(StorageKeys.SystemDescription, setting.value)
            }
          })
        }
      } catch (settingsError) {
        console.error('获取系统配置失败:', settingsError)
        // 配置获取失败不影响登录流程
      }

      // 显示成功提示
      showSuccessNotification({
        title: '登录成功',
        message: '欢迎回来，' + response.data?.realName
      })

      // 先初始化路由配置，确保动态路由加载完成
      await initRouteConfig()

      // 使用 replace 跳转，避免历史记录问题
      router.replace('/dashboard')
    } else {
      // 登录失败
      showErrorNotification({
        title: '登录失败',
        message: response.message || '登录失败，请检查用户名和密码'
      })
    }
  } catch (error: any) {
    console.error('登录错误:', error)
    const errorMessage = error?.message || '登录失败，请稍后重试'
    showErrorNotification({
      title: '登录失败',
      message: errorMessage
    })

  } finally {
    loading.value = false
  }
}

// 切换到注册模式
const switchToRegister = () => {
  currentMode.value = 'register'
  // 重置登录表单
  form.value = {
    userName: '',
    password: ''
  }
}

// 切换到登录模式
const switchToLogin = () => {
  currentMode.value = 'login'
  // 重置注册表单
  registerForm.value = {
    userName: '',
    password: '',
    confirmPassword: '',
    email: '',
    realName: ''
  }
  // 重置忘记密码表单
  forgotPasswordForm.value = {
    userName: '',
    email: '',
    newPassword: '',
    confirmPassword: ''
  }
}

// 切换到忘记密码模式
const switchToForgotPassword = () => {
  currentMode.value = 'forgot'
  // 重置登录表单
  form.value = {
    userName: '',
    password: ''
  }
}

// 注册处理
const handleRegister = async () => {
  if (!registerFormRef.value) return

  registerLoading.value = true
  try {
    // 验证表单
    await registerFormRef.value.validate((valid: boolean) => {
      if (!valid) {
        throw new Error('表单验证失败')
      }
    })

    // 调用注册接口
    const response = await http.post<RegisterResponse>(api.User.CreateUserAsync, {
      userName: registerForm.value.userName,
      password: aesEncrypt(registerForm.value.password),
      email: registerForm.value.email,
      realName: registerForm.value.realName || undefined
    })

    // 处理注册结果
    if (response.success) {
      showSuccessNotification({
        title: '注册成功',
        message: '账号已创建，请登录'
      })

      // 切换到登录模式并填充用户名
      switchToLogin()
      form.value.userName = registerForm.value.userName
      form.value.password = ''
    } else {
      showErrorNotification({
        title: '注册失败',
        message: response.message || '注册失败，请稍后重试'
      })
    }
  } catch (error: any) {
    console.error('注册错误:', error)
    const errorMessage = error?.message || '注册失败，请稍后重试'
    showErrorNotification({
      title: '注册失败',
      message: errorMessage
    })
  } finally {
    registerLoading.value = false
  }
}

// 忘记密码处理
const handleForgotPassword = async () => {
  if (!forgotPasswordFormRef.value) return

  forgotPasswordLoading.value = true
  try {
    // 验证表单
    await forgotPasswordFormRef.value.validate((valid: boolean) => {
      if (!valid) {
        throw new Error('表单验证失败')
      }
    })

    // 调用重置密码接口
    // 这里使用 UpdateUserAsync 接口来更新密码
    // 首先需要获取用户ID
    const userResponse = await http.post<{
      success: boolean
      message?: string
      data?: { id?: string; email?: string }
    }>('api/User/GetUserByUserNameAsync', {
      userName: forgotPasswordForm.value.userName
    })

    if (!userResponse.success || !userResponse.data?.id) {
      showErrorNotification({
        title: '验证失败',
        message: '用户名不存在'
      })
      return
    }

    // 验证邮箱是否匹配
    if (userResponse.data.email !== forgotPasswordForm.value.email) {
      showErrorNotification({
        title: '验证失败',
        message: '邮箱与用户名不匹配'
      })
      return
    }

    // 调用重置密码接口
    const response = await http.post<{
      success: boolean
      message?: string
    }>(api.User.ResetPasswordAsync, {
      id: userResponse.data.id,
      newPassword: aesEncrypt(forgotPasswordForm.value.newPassword)
    })

    // 处理重置结果
    if (response.success) {
      showSuccessNotification({
        title: '密码重置成功',
        message: '请使用新密码登录'
      })

      // 切换到登录模式并填充用户名
      switchToLogin()
      form.value.userName = forgotPasswordForm.value.userName
      form.value.password = ''
    } else {
      showErrorNotification({
        title: '密码重置失败',
        message: response.message || '密码重置失败，请稍后重试'
      })
    }
  } catch (error: any) {
    console.error('密码重置错误:', error)
    const errorMessage = error?.message || '密码重置失败，请稍后重试'
    showErrorNotification({
      title: '密码重置失败',
      message: errorMessage
    })
  } finally {
    forgotPasswordLoading.value = false
  }
}

// 加载公开的系统设置
const loadPublicSettings = async () => {
  try {
    const response = await getPublicSettings()
    if (response.data) {
      const settings = response.data as SettingDto[]
      settings.forEach(setting => {
        if (setting.key === 'SystemName' && setting.value) {
          systemName.value = setting.value
          setSystemName(setting.value)
        }
        if (setting.key === 'SystemDescription' && setting.value) {
          setItem(StorageKeys.SystemDescription, setting.value)
        }
      })
    }
  } catch (error) {
    console.error('获取公开设置失败:', error)
    // 获取失败时使用默认值
  }
}

// 轮播文案
onMounted(() => {
  // 加载公开的系统设置
  loadPublicSettings()

  setInterval(() => {
    currentTextIndex.value = (currentTextIndex.value + 1) % brandTexts.value.length
  }, 4000)
})
</script>

<template>
  <div class="login-page">
    <!-- 背景装饰 -->
    <div class="background-decoration">
      <div class="circle circle-1"></div>
      <div class="circle circle-2"></div>
      <div class="circle circle-3"></div>
      <div class="circle circle-4"></div>
      <div class="circle circle-5"></div>
      <div class="circle circle-6"></div>
      <!-- 网格背景 -->
      <div class="grid-bg"></div>
    </div>

    <!-- 全屏品牌背景区 -->
    <div class="brand-section">
      <div class="brand-content">
        <div class="brand-logo">
          <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M12 2L2 7L12 12L22 7L12 2Z" stroke="currentColor" stroke-width="2" stroke-linecap="round"
              stroke-linejoin="round" />
            <path d="M2 17L12 22L22 17" stroke="currentColor" stroke-width="2" stroke-linecap="round"
              stroke-linejoin="round" />
            <path d="M2 12L12 17L22 12" stroke="currentColor" stroke-width="2" stroke-linecap="round"
              stroke-linejoin="round" />
          </svg>
        </div>
        <h1 class="brand-title">{{ systemName }}</h1>
        <p class="brand-subtitle">让工作更高效，让生活更智能</p>

        <!-- 动态文案轮播 -->
        <div class="features-carousel">
          <div class="feature-item" v-for="(item, index) in brandTexts" :key="index"
            :class="{ active: index === currentTextIndex }">
            <div class="feature-icon">
              <el-icon :size="24">
                <component :is="getIconComponent(item.icon)" />
              </el-icon>
            </div>
            <div class="feature-text">
              <h3>{{ item.title }}</h3>
              <p>{{ item.desc }}</p>
            </div>
          </div>
        </div>

        <!-- 装饰性图标 -->
        <div class="decorative-icons">
          <div class="icon-item icon-1">
            <el-icon :size="40" color="rgba(255,255,255,0.3)">
              <DataLine />
            </el-icon>
          </div>
          <div class="icon-item icon-2">
            <el-icon :size="32" color="rgba(255,255,255,0.2)">
              <Lock />
            </el-icon>
          </div>
          <div class="icon-item icon-3">
            <el-icon :size="28" color="rgba(255,255,255,0.25)">
              <TrendCharts />
            </el-icon>
          </div>
          <div class="icon-item icon-4">
            <el-icon :size="36" color="rgba(255,255,255,0.15)">
              <User />
            </el-icon>
          </div>
        </div>
      </div>
    </div>

    <!-- 右侧浮动登录/注册表单 -->
    <div class="login-form-float">
      <div class="login-card">
        <!-- 登录表单 -->
        <template v-if="currentMode === 'login'">
          <div class="form-header">
            <h2 class="form-title">欢迎登录</h2>
            <p class="form-subtitle">请输入您的账号和密码</p>
          </div>

          <el-form ref="formRef" :model="form" :rules="rules" class="login-form" size="large">
            <el-form-item prop="userName">
              <el-input v-model="form.userName" placeholder="用户名" prefix-icon="User" clearable />
            </el-form-item>

            <el-form-item prop="password">
              <el-input v-model="form.password" type="password" placeholder="密码" prefix-icon="Lock" show-password
                clearable />
            </el-form-item>

            <el-form-item>
              <el-checkbox v-model="rememberMe">记住我</el-checkbox>
            </el-form-item>

            <el-form-item>
              <el-button type="primary" class="login-btn" :loading="loading" @click="handleSubmit">
                登录
              </el-button>
            </el-form-item>
          </el-form>

          <div class="form-footer">
            <span>还没有账号？</span>
            <el-link type="primary" class="register-link" @click="switchToRegister">立即注册</el-link>
            <span style="margin: 0 10px;">|</span>
            <el-link type="primary" class="register-link" @click="switchToForgotPassword">忘记密码？</el-link>
          </div>
        </template>

        <!-- 注册表单 -->
        <template v-else-if="currentMode === 'register'">
          <div class="form-header">
            <h2 class="form-title">创建账号</h2>
            <p class="form-subtitle">填写以下信息完成注册</p>
          </div>

          <el-form ref="registerFormRef" :model="registerForm" :rules="registerRules" class="login-form" size="large">
            <el-form-item prop="userName">
              <el-input v-model="registerForm.userName" placeholder="用户名" prefix-icon="User" clearable />
            </el-form-item>

            <el-form-item prop="email">
              <el-input v-model="registerForm.email" placeholder="邮箱" prefix-icon="Message" clearable />
            </el-form-item>

            <el-form-item prop="realName">
              <el-input v-model="registerForm.realName" placeholder="真实姓名（选填）" prefix-icon="UserFilled" clearable />
            </el-form-item>

            <el-form-item prop="password">
              <el-input v-model="registerForm.password" type="password" placeholder="密码" prefix-icon="Lock" show-password
                clearable />
            </el-form-item>

            <el-form-item prop="confirmPassword">
              <el-input v-model="registerForm.confirmPassword" type="password" placeholder="确认密码" prefix-icon="Lock"
                show-password clearable />
            </el-form-item>

            <el-form-item>
              <el-button type="primary" class="login-btn" :loading="registerLoading" @click="handleRegister">
                注册
              </el-button>
            </el-form-item>
          </el-form>

          <div class="form-footer">
            <span>已有账号？</span>
            <el-link type="primary" class="register-link" @click="switchToLogin">立即登录</el-link>
          </div>
        </template>

        <!-- 忘记密码表单 -->
        <template v-else-if="currentMode === 'forgot'">
          <div class="form-header">
            <h2 class="form-title">重置密码</h2>
            <p class="form-subtitle">验证身份后设置新密码</p>
          </div>

          <el-form ref="forgotPasswordFormRef" :model="forgotPasswordForm" :rules="forgotPasswordRules" class="login-form" size="large">
            <el-form-item prop="userName">
              <el-input v-model="forgotPasswordForm.userName" placeholder="用户名" prefix-icon="User" clearable />
            </el-form-item>

            <el-form-item prop="email">
              <el-input v-model="forgotPasswordForm.email" placeholder="注册邮箱" prefix-icon="Message" clearable />
            </el-form-item>

            <el-form-item prop="newPassword">
              <el-input v-model="forgotPasswordForm.newPassword" type="password" placeholder="新密码" prefix-icon="Lock" show-password
                clearable />
            </el-form-item>

            <el-form-item prop="confirmPassword">
              <el-input v-model="forgotPasswordForm.confirmPassword" type="password" placeholder="确认新密码" prefix-icon="Lock"
                show-password clearable />
            </el-form-item>

            <el-form-item>
              <el-button type="primary" class="login-btn" :loading="forgotPasswordLoading" @click="handleForgotPassword">
                重置密码
              </el-button>
            </el-form-item>
          </el-form>

          <div class="form-footer">
            <span>想起密码了？</span>
            <el-link type="primary" class="register-link" @click="switchToLogin">立即登录</el-link>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<style scoped>
.login-page {
  position: relative;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  overflow: hidden;
}

/* 背景装饰 */
.background-decoration {
  position: absolute;
  width: 100%;
  height: 100%;
  overflow: hidden;
  z-index: 1;
}

/* 网格背景 */
.grid-bg {
  position: absolute;
  width: 100%;
  height: 100%;
  background-image:
    linear-gradient(rgba(255, 255, 255, 0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255, 255, 255, 0.05) 1px, transparent 1px);
  background-size: 50px 50px;
  transform: perspective(500px) rotateX(60deg);
  transform-origin: center top;
  animation: gridMove 60s linear infinite;
  z-index: 1;
}

@keyframes gridMove {
  0% {
    transform: perspective(500px) rotateX(60deg) translateY(0);
  }

  100% {
    transform: perspective(500px) rotateX(60deg) translateY(50px);
  }
}

.circle {
  position: absolute;
  border-radius: 50%;
  filter: blur(80px);
  opacity: 0.6;
  animation: float 20s infinite alternate;
  z-index: 2;
}

/* 大型渐变圆圈 - 左侧 */
.circle-1 {
  width: 600px;
  height: 600px;
  background: linear-gradient(135deg, #764ba2 0%, #4361ee 50%, #3a0ca3 100%);
  top: -200px;
  left: -200px;
  animation-duration: 30s;
}

/* 右侧彩色圆圈 */
.circle-2 {
  width: 500px;
  height: 500px;
  background: linear-gradient(135deg, #f72585, #b5179e);
  bottom: -150px;
  right: -150px;
  animation-delay: -5s;
}

/* 中心彩色圆圈 */
.circle-3 {
  width: 400px;
  height: 400px;
  background: linear-gradient(135deg, #4cc9f0, #4895ef, #4361ee);
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  animation-delay: -10s;
  animation-duration: 25s;
}

/* 右上角彩色圆圈 */
.circle-4 {
  width: 300px;
  height: 300px;
  background: linear-gradient(135deg, #f72585, #b5179e, #9d4edd);
  top: 10%;
  right: 10%;
  animation-delay: -15s;
}

/* 左下角新增圆圈 */
.circle-5 {
  width: 350px;
  height: 350px;
  background: linear-gradient(135deg, #f9c74f, #f94144, #f72585);
  bottom: 10%;
  left: 5%;
  opacity: 0.5;
  animation-delay: -20s;
  animation-duration: 35s;
}

/* 右下角新增圆圈 */
.circle-6 {
  width: 250px;
  height: 250px;
  background: linear-gradient(135deg, #00f5d4, #00b894, #00cec9);
  top: 60%;
  right: 5%;
  opacity: 0.4;
  animation-delay: -25s;
  animation-duration: 28s;
}

@keyframes float {
  0% {
    transform: translate(0, 0) scale(1);
  }

  100% {
    transform: translate(50px, 50px) scale(1.1);
  }
}

/* 全屏品牌背景区 */
.brand-section {
  position: absolute;
  top: 0;
  left: 0;
  bottom: 0;
  width: 50%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 80px 100px;
  color: white;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 50%, #4361ee 100%);
  z-index: 10;
  overflow: hidden;
}

.brand-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background:
    radial-gradient(circle at 20% 30%, rgba(255, 255, 255, 0.1) 0, transparent 50%),
    radial-gradient(circle at 80% 70%, rgba(255, 255, 255, 0.05) 0, transparent 50%);
  pointer-events: none;
}

.brand-content {
  position: relative;
  z-index: 2;
  max-width: 500px;
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.brand-logo {
  width: 100px;
  height: 100px;
  margin-bottom: 40px;
  animation: logoPulse 3s ease-in-out infinite;
}

.brand-logo svg {
  width: 100%;
  height: 100%;
  filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.2));
}

@keyframes logoPulse {

  0%,
  100% {
    transform: scale(1);
  }

  50% {
    transform: scale(1.05);
  }
}

.brand-title {
  font-size: 48px;
  font-weight: 700;
  margin-bottom: 20px;
  text-shadow: 3px 3px 6px rgba(0, 0, 0, 0.3);
  line-height: 1.2;
  text-align: center;
}

.brand-subtitle {
  font-size: 18px;
  opacity: 0.95;
  line-height: 1.6;
  margin-bottom: 50px;
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.2);
  text-align: center;
}

/* 动态文案轮播 */
.features-carousel {
  margin-top: 40px;
  padding: 30px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  position: relative;
  overflow: hidden;
  width: 100%;
  max-width: 400px;
  margin-left: auto;
  margin-right: auto;
}

.feature-item {
  display: flex;
  align-items: center;
  gap: 20px;
  padding: 20px;
  border-radius: 12px;
  transition: all 0.5s ease;
  opacity: 0;
  transform: translateY(20px);
  position: absolute;
  width: 100%;
  box-sizing: border-box;
}

.feature-item.active {
  opacity: 1;
  transform: translateY(0);
  position: relative;
}

.feature-item:not(.active) {
  display: none;
}

.feature-icon {
  width: 50px;
  height: 50px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.2), rgba(255, 255, 255, 0.05));
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  flex-shrink: 0;
}

.feature-text {
  flex: 1;
  min-width: 0;
}

.feature-text h3 {
  font-size: 20px;
  font-weight: 600;
  margin: 0 0 5px 0;
  color: white;
}

.feature-text p {
  font-size: 14px;
  margin: 0;
  opacity: 0.9;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* 装饰性图标 */
.decorative-icons {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  pointer-events: none;
  z-index: 1;
  overflow: hidden;
  display: none;
}

.icon-item {
  position: absolute;
  animation: iconFloat 20s infinite alternate;
}

.icon-item.icon-1 {
  top: 15%;
  left: 10%;
  animation-duration: 25s;
}

.icon-item.icon-2 {
  bottom: 20%;
  right: 15%;
  animation-duration: 30s;
  animation-delay: -5s;
}

.icon-item.icon-3 {
  top: 40%;
  right: 10%;
  animation-duration: 28s;
  animation-delay: -10s;
}

.icon-item.icon-4 {
  bottom: 10%;
  left: 20%;
  animation-duration: 32s;
  animation-delay: -15s;
}

@keyframes iconFloat {
  0% {
    transform: translate(0, 0) rotate(0deg);
  }

  100% {
    transform: translate(30px, 30px) rotate(10deg);
  }
}

/* 右侧浮动登录表单 */
.login-form-float {
  position: absolute;
  right: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 50%;
  z-index: 20;
  padding: 60px 80px;
  box-sizing: border-box;
  display: flex;
  justify-content: center;
  align-items: center;
  background: transparent;
}

.login-card {
  width: 100%;
  max-width: 420px;
  padding: 50px;
  background: rgba(255, 255, 255, 0.85);
  border-radius: 20px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15);
  backdrop-filter: blur(15px);
  -webkit-backdrop-filter: blur(15px);
}

.form-header {
  margin-bottom: 40px;
}

.form-title {
  font-size: 28px;
  font-weight: 700;
  color: #333;
  margin-bottom: 10px;
}

.form-subtitle {
  font-size: 14px;
  color: #666;
}

/* 表单样式 */
.login-form {
  :deep(.el-form-item) {
    margin-bottom: 25px;
  }

  :deep(.el-input__wrapper) {
    padding: 12px 15px;
    border-radius: 10px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
    transition: all 0.3s ease;
    background: rgba(255, 255, 255, 0.9);

    &:hover {
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.12);
    }

    &.is-focus {
      box-shadow: 0 4px 16px rgba(67, 97, 238, 0.2);
    }
  }

  :deep(.el-input__prefix) {
    svg {
      width: 20px;
      height: 20px;
      color: #999;
    }
  }
}

/* 表单底部 */
.form-footer {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: 30px;
  padding-top: 25px;
  border-top: 1px solid rgba(238, 238, 238, 0.8);
  gap: 10px;
}

.register-link {
  font-size: 14px;
}

/* 登录按钮 */
.login-btn {
  width: 100%;
  padding: 14px;
  font-size: 16px;
  font-weight: 600;
  border-radius: 10px;
  background: linear-gradient(135deg, #4361ee, #3a0ca3);
  border: none;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(67, 97, 238, 0.3);

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(67, 97, 238, 0.4);
  }

  &:active {
    transform: translateY(0);
  }
}

/* 响应式 */
@media (max-width: 768px) {
  .login-page {
    flex-direction: column;
  }

  .brand-section {
    position: relative;
    width: 100%;
    padding: 60px 30px;
    flex: none;
    min-height: 400px;
  }

  .brand-title {
    font-size: 32px;
  }

  .brand-subtitle {
    font-size: 16px;
  }

  .features-carousel {
    display: none;
  }

  .decorative-icons {
    display: none;
  }

  .login-form-float {
    position: relative;
    top: 0;
    transform: none;
    width: 100%;
    padding: 40px 20px;
    background: transparent;
  }

  .login-card {
    padding: 35px 25px;
    background: rgba(255, 255, 255, 0.85);
  }
}

@media (min-width: 769px) and (max-width: 1024px) {
  .brand-section {
    padding: 60px 50px;
  }

  .brand-title {
    font-size: 40px;
  }

  .brand-logo {
    width: 80px;
    height: 80px;
  }

  .login-form-float {
    width: 50%;
    padding: 60px;
    background: transparent;
  }

  .login-card {
    background: rgba(255, 255, 255, 0.85);
  }
}
</style>
