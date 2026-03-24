import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import Layout from '../views/layout/Layout.vue'
import { generateRoutesFromMenus, getRoutesFromStorage } from '@/utils/routeGenerator'
import type { MenuItem } from '@/utils/routeGenerator'

// 动态路由函数
const createDynamicRouter = () => {
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
        name: 'layout',
        component: Layout,
        children: [
          {
            path: 'dashboard',
            name: 'dashboard',
            component: () => import('../views/home/Dashboard/Dashboard.vue')
          },
          {
            path: 'users',
            name: 'users',
            component: () => import('../views/home/Users/Users.vue')
          },
          {
            path: 'products',
            name: 'products',
            component: () => import('../views/home/Products/Products.vue')
          },
          {
            path: 'settings',
            name: 'settings',
            component: () => import('../views/home/Settings/Settings.vue')
          },
          {
            path: 'menu',
            name: 'menu',
            component: () => import('../views/home/menu/Menu.vue')
          }
        ]
      }
    ]
  })

  return router
}

const router = createDynamicRouter()

// 动态添加路由
export const addDynamicRoutes = (menus: MenuItem[]) => {
  const routes = generateRoutesFromMenus(menus)

  routes.forEach(route => {
    router.addRoute('layout', route)
  })
}

// 从 localStorage 添加路由
export const addRoutesFromStorage = () => {
  const menus = getRoutesFromStorage()
  if (menus.length > 0) {
    addDynamicRoutes(menus)
  }
}

// 导航守卫：检查登录状态
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  
  // 如果访问的是登录页
  if (to.path === '/') {
    // 如果已登录，跳转到仪表盘
    if (token) {
      next('/dashboard')
    } else {
      next()
    }
  } 
  // 如果访问的是受保护的页面
  else {
    // 如果未登录，跳转到登录页
    if (!token) {
      next('/')
    } else {
      next()
    }
  }
})

// 导航守卫：访问 / 时自动跳转到 /dashboard
export default router
