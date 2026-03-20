<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted, markRaw } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { User, Fold, Expand, Collection, Menu, Setting } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'

// 菜单数据
interface MenuItem {
  path: string
  name: string
  icon: any
}

interface UserInfo {
  userId?: string
  userName?: string
  realName?: string
}

const menuList = ref<MenuItem[]>([
  { path: '/dashboard', name: '首页', icon: markRaw(Collection) },
  { path: '/users', name: '用户管理', icon: markRaw(User) },
  { path: '/products', name: '产品管理', icon: markRaw(Menu) },
  { path: '/settings', name: '系统设置', icon: markRaw(Setting) }
])

const router = useRouter()
const isSidebarCollapsed = ref(false)
const isMobile = ref(false)
const isMobileMenuVisible = ref(false)
const userInfo = ref<UserInfo>({})

// 获取用户信息
const getUserInfo = () => {
  const storedInfo = localStorage.getItem('userInfo')
  if (storedInfo) {
    try {
      userInfo.value = JSON.parse(storedInfo)
    } catch (e) {
      console.error('解析用户信息失败', e)
    }
  }
}

// 侧边栏折叠切换
const toggleSidebar = () => {
  if (isMobile.value) {
    isMobileMenuVisible.value = !isMobileMenuVisible.value
  } else {
    isSidebarCollapsed.value = !isSidebarCollapsed.value
  }
}

// 移动端菜单点击后关闭
const handleMobileMenuClick = () => {
  if (isMobile.value) {
    isMobileMenuVisible.value = false
  }
}

// 菜单点击处理
const handleMenuClick = (path: string) => {
  handleMobileMenuClick()
  router.push(path)
}

// 退出登录
const handleLogout = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('userInfo')
  ElMessage.success('已退出登录')
  router.push('/')
}

// 响应式检测
const checkMobile = () => {
  isMobile.value = window.innerWidth < 768
  if (isMobile.value) {
    isSidebarCollapsed.value = true
    isMobileMenuVisible.value = false
  } else {
    isMobileMenuVisible.value = false
  }
}

onMounted(() => {
  checkMobile()
  getUserInfo()
  window.addEventListener('resize', checkMobile)
})

onUnmounted(() => {
  window.removeEventListener('resize', checkMobile)
})

// 监听路由变化，关闭移动端菜单
watch(
  () => router.currentRoute.value.path,
  () => {
    if (isMobile.value) {
      isMobileMenuVisible.value = false
    }
  }
)
</script>

<template>
  <div class="layout-container">
    <!-- 顶部导航栏 -->
    <el-header class="layout-header">
      <div class="header-left">
        <el-icon class="toggle-btn" @click="toggleSidebar" :size="24">
          <Fold v-if="isSidebarCollapsed" />
          <Expand v-else />
        </el-icon>
        <h1 class="logo" :class="{ 'hidden': isMobile }">智能管理系统</h1>
      </div>
      <div class="header-right">
        <el-dropdown trigger="hover">
          <div class="user-info">
            <el-icon :size="20">
              <User />
            </el-icon>
            <span class="user-name">{{ userInfo.realName || userInfo.userName || '管理员' }}</span>
          </div>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item>
                <RouterLink to="/profile" class="dropdown-item">个人中心</RouterLink>
              </el-dropdown-item>
              <el-dropdown-item divided @click="handleLogout">
                退出登录
              </el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>
    </el-header>

    <el-container class="layout-main">
      <!-- 移动端菜单遮罩 -->
      <div class="mobile-menu-overlay" v-if="isMobile && isMobileMenuVisible" @click="isMobileMenuVisible = false">
      </div>

      <!-- 侧边栏菜单 -->
      <el-aside :width="isSidebarCollapsed ? '64px' : '200px'" class="layout-sidebar"
        :class="{ 'collapsed': isSidebarCollapsed, 'mobile-visible': isMobile && isMobileMenuVisible }">
        <el-menu :collapse="isSidebarCollapsed && !isMobile" :collapse-transition="false" background-color="#fff"
          text-color="#606266" active-text-color="#4361ee" :default-active="$route.path" class="layout-menu">
          <el-menu-item v-for="item in menuList" :key="item.path" :index="item.path"
            @click="handleMenuClick(item.path)">
            <el-icon :size="20">
              <component :is="item.icon" />
            </el-icon>
            <span>{{ item.name }}</span>
          </el-menu-item>
        </el-menu>
      </el-aside>

      <!-- 主内容区 -->
      <el-main class="layout-content">
        <div class="content-wrapper">
          <router-view />
        </div>
      </el-main>
    </el-container>
  </div>
</template>

<style scoped>
.layout-container {
  height: 100vh;
  background: #f5f7fa;
}

.layout-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20px;
  background: #fff;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  z-index: 100;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 20px;
}

.toggle-btn {
  cursor: pointer;
  transition: transform 0.3s;
}

.toggle-btn:hover {
  color: #4361ee;
}

.logo {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.logo.hidden {
  display: none;
}

.header-right {
  display: flex;
  align-items: center;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  border-radius: 8px;
  cursor: pointer;
  transition: background 0.3s;
}

.user-info:hover {
  background: #f5f7fa;
}

.user-name {
  font-size: 14px;
  color: #333;
}

.layout-main {
  display: flex;
  height: calc(100vh - 60px);
}

.layout-sidebar {
  background: #fff;
  box-shadow: 2px 0 8px rgba(0, 0, 0, 0.08);
  transition: width 0.3s;
  overflow-x: hidden;
}

.layout-content {
  flex: 1;
  padding: 20px;
  overflow-y: auto;
}

.content-wrapper {
  height: 100%;
}

/* 折叠状态样式 */
.layout-sidebar.collapsed {
  width: 64px;
}

.layout-sidebar.collapsed .el-menu {
  border-right: none;
}

.layout-sidebar.collapsed .el-menu-item {
  justify-content: center;
  padding: 0 20px;
}

.layout-sidebar.collapsed .el-menu-item .el-icon {
  margin-right: 0;
}

.layout-sidebar.collapsed .el-menu-item span {
  display: none;
}

/* 响应式 */
@media (max-width: 768px) {
  .layout-header h1.logo {
    display: none;
  }

  .layout-sidebar {
    display: none;
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    z-index: 1000;
    height: 100vh;
    box-shadow: 2px 0 8px rgba(0, 0, 0, 0.2);
  }

  .layout-sidebar.mobile-visible {
    display: block;
  }

  .layout-main {
    height: calc(100vh - 60px);
  }
}

/* 移动端菜单样式 */
.mobile-menu-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 99;
}

@media (max-width: 768px) {
  .layout-sidebar {
    display: none;
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    z-index: 1000;
    height: 100vh;
    box-shadow: 2px 0 8px rgba(0, 0, 0, 0.2);
    width: 200px !important;
  }

  .layout-sidebar.mobile-visible {
    display: block;
  }

  .layout-sidebar.collapsed {
    width: 200px !important;
  }

  .layout-sidebar.collapsed .menu-text {
    opacity: 1;
    width: auto;
  }

  .layout-sidebar.collapsed .menu-item {
    justify-content: flex-start;
    padding: 12px 20px;
  }

  .layout-sidebar.collapsed .menu-item .el-icon {
    margin-right: 12px;
  }
}

@media (min-width: 769px) {
  .layout-sidebar.mobile-visible {
    display: none !important;
  }
}
</style>
