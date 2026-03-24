<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted, markRaw } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { User, Fold, Expand, Collection, Menu, Setting } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'
import * as menuApi from '@/api/menu'

// 菜单数据
interface MenuItem {
  id?: string | number
  path: string
  name: string
  icon?: string
  parentId?: string | number
  children?: MenuItem[]
}

interface UserInfo {
  userId?: string
  userName?: string
  realName?: string
}

const menuList = ref<MenuItem[]>([])
const router = useRouter()
const isSidebarCollapsed = ref(false)
const isMobile = ref(false)
const isMobileMenuVisible = ref(false)
const userInfo = ref<UserInfo>({})
const isMenuLoaded = ref(false)

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

// 从 localStorage 获取菜单数据
const getMenuFromStorage = (): MenuItem[] | null => {
  try {
    const storedMenu = localStorage.getItem('sidebarMenu')
    if (storedMenu) {
      return JSON.parse(storedMenu)
    }
  } catch (e) {
    console.error('解析菜单数据失败', e)
  }
  return null
}

// 将菜单数据保存到 localStorage
const saveMenuToStorage = (menuList: MenuItem[]) => {
  try {
    localStorage.setItem('sidebarMenu', JSON.stringify(menuList))
  } catch (e) {
    console.error('保存菜单数据失败', e)
  }
}


// 获取菜单树并转换为侧边栏菜单格式
const loadMenuTree = async () => {
  // 优先从 localStorage 获取
  const cachedMenu = getMenuFromStorage()
  if (cachedMenu) {
    menuList.value = cachedMenu
    // 动态添加路由
    isMenuLoaded.value = true
    return
  }

  try {
    const response = await menuApi.getSidebarMenuTree()
    if (response.data && Array.isArray(response.data)) {
      // 后端已返回树形结构，无需再次转换
      menuList.value = response.data
      // 动态添加路由
      addDynamicRoutes(response.data)
      saveMenuToStorage(response.data)
      isMenuLoaded.value = true
    }
  } catch (error) {
    console.error('加载菜单数据失败:', error)
    ElMessage.error('加载菜单数据失败')
  }
}

// 根据图标名称获取图标组件
const getIconByName = (iconName: string) => {
  if (!iconName) return null

  const iconMap: Record<string, any> = {
    'Collection': markRaw(Collection),
    'Menu': markRaw(Menu),
    'User': markRaw(User),
    'Setting': markRaw(Setting)
  }

  return iconMap[iconName] || null
}

// 清除菜单缓存（用于菜单更新后刷新）
const clearMenuCache = () => {
  localStorage.removeItem('sidebarMenu')
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
const handleMenuClick = (item: MenuItem) => {
  // 如果有子菜单，不进行路由跳转
  if (item.children && item.children.length > 0) {
    return
  }

  handleMobileMenuClick()
  if (item.path) {
    router.push(item.path)
  }
}

// 退出登录
const handleLogout = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('userInfo')
  localStorage.removeItem('sidebarMenu')
  ElMessage.success('已退出登录')
  router.push('/')
}

// 清除缓存
const clearCache = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('userInfo')
  localStorage.removeItem('sidebarMenu')
  ElMessage.success('缓存已清除')
  // 刷新页面以重新加载菜单和路由
  window.location.href = '/'
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
  loadMenuTree() // 从后端 API 获取菜单数据
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
        <el-button type="danger" size="small" @click="clearCache" style="margin-right: 10px;">
          清除缓存
        </el-button>
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
          text-color="#606266" active-text-color="#4361ee" :default-active="$route.path" class="layout-menu"
          :unique-opened="true">
          <template v-for="item in menuList" :key="item.path">
            <!-- 有子菜单的菜单项 -->
            <el-sub-menu v-if="item.children && item.children.length > 0" :index="item.path">
              <template #title>
                <el-icon v-if="item.icon" :size="20">
                  <component :is="getIconByName(item.icon)" />
                </el-icon>
                <span v-if="!isSidebarCollapsed || isMobile" class="menu-text">{{ item.name }}</span>
              </template>
              <el-menu-item v-for="child in item.children" :key="child.path" :index="child.path"
                @click="handleMenuClick(child)">
                <el-icon v-if="child.icon" :size="16">
                  <component :is="getIconByName(child.icon)" />
                </el-icon>
                <span>{{ child.name }}</span>
              </el-menu-item>
            </el-sub-menu>

            <!-- 没有子菜单的菜单项 -->
            <el-menu-item v-else :index="item.path" @click="handleMenuClick(item)">
              <el-icon v-if="item.icon" :size="20">
                <component :is="getIconByName(item.icon)" />
              </el-icon>
              <span class="menu-text">{{ item.name }}</span>
            </el-menu-item>
          </template>
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

/* 子菜单样式优化 */
.layout-sidebar .el-sub-menu__title {
  padding-right: 20px !important;
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

/* 子菜单展开动画优化 */
.layout-menu .el-sub-menu__title:hover {
  background-color: #f5f7fa !important;
}

.layout-menu .el-menu-item:hover {
  background-color: #f5f7fa !important;
}

@media (min-width: 769px) {
  .layout-sidebar.mobile-visible {
    display: none !important;
  }
}
</style>
