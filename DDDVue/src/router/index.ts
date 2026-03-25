import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import Error from '../views/Error/404.vue'
import Layout from '../views/layout/Layout.vue'
import ClearCache from '../views/home/ClearCache/ClearCache.vue'
import { getItem, StorageKeys } from '@/utils/storage'
import { initRouteConfig, getRoutes, convertToRouterFormat } from './detail'

// 动态路由函数
const createDynamicRouter = async () => {
  // 初始化路由配置
  await initRouteConfig()
  
  // 获取路由配置
  const dynamicRoutes = await getRoutes()
  
  // 转换为 Vue Router 格式
  const routerFormatRoutes = convertToRouterFormat(dynamicRoutes)

  const router = createRouter({
    history: createWebHistory(),
    routes: [
      {
        path: '/',
        name: 'login',
        component: Login
      },
      {
        path: '/',
        name: 'Layout',
        component: Layout,
        children: [
          // 动态路由：从 API 获取
          ...routerFormatRoutes,
          // 固定路由：系统页面
          {
              path: 'user-role',
              name: 'user-role',
              component: () => import('../views/home/UserRole/UserRole.vue')
          },
          {
              path: 'profile',
              name: 'profile',
              component: () => import('../views/home/Profile/Profile.vue')
          },
          {
              path: 'clear-cache',
              name: 'clear-cache',
              component: ClearCache
          },
          // 通配符路由，必须放在最后
          {
            path: '/:pathMatch(.*)*',
            name: 'Error',
            component: Error
          },
        ]
      },
    ]
  })

  return router
}

// 初始化路由
let routerInstance: any = null
export const initAppRouter = async () => {
  if (!routerInstance) {
    routerInstance = await createDynamicRouter()
  }
  return routerInstance
}

// 导航守卫：检查登录状态
const setupNavigationGuards = (router: any) => {
  router.beforeEach((to: any, from: any) => {
    const token = getItem<string>(StorageKeys.Token)

    // 如果访问的是登录页
    if (to.path === '/') {
      // 如果已登录，跳转到仪表盘
      if (token) {
        return '/dashboard'
      } else {
        return true
      }
    }
    // 如果访问的是受保护的页面
    else {
      // 如果未登录，跳转到登录页
      if (!token) {
        return '/'
      } else {
        return true
      }
    }
  })
}

// 初始化路由并设置导航守卫
const initRouter = async () => {
  const router = await createDynamicRouter()
  setupNavigationGuards(router)
  return router
}

// 导出初始化函数
export const initAppRouterWithGuards = async () => {
  const router = await initRouter()
  return router
}

// 导出默认路由实例
export default await initRouter()
