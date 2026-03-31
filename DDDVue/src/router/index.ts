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

// 动态添加路由到 Layout 的子路由
const addDynamicRoutesToRouter = async (router: any) => {
  // 获取最新路由配置
  const dynamicRoutes = await getRoutes()
  // 转换为 Vue Router 格式
  const routerFormatRoutes = convertToRouterFormat(dynamicRoutes)

  // 获取 Layout 路由
  const layoutRoute = router.getRoutes().find((r: any) => r.name === 'Layout')
  if (!layoutRoute) return

  // 添加新的动态路由到 Layout 的子路由
  routerFormatRoutes.forEach((route: any) => {
    // 检查路由是否已存在
    const existingRoute = router.resolve(route.path).name
    if (existingRoute === 'Error') {
      // 路由不存在，添加它
      router.addRoute('Layout', route)
    }
  })
}

// 导航守卫：检查登录状态
const setupNavigationGuards = (router: any) => {
  router.beforeEach(async (to: any, _from: any) => {
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
      }

      // 检查路由是否存在，如果不存在可能是动态路由还没加载完成
      const routeExists = router.resolve(to.path).name !== 'Error'
      if (!routeExists && to.path !== '/404') {
        // 等待动态路由加载并添加到 router
        await addDynamicRoutesToRouter(router)
        // 重新检查路由是否存在
        const retryRoute = router.resolve(to.path)
        if (retryRoute.name === 'Error') {
          // 路由确实不存在，返回 404
          return true
        }
        // 路由已添加，继续导航到目标页面
        return to.fullPath
      }

      return true
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
