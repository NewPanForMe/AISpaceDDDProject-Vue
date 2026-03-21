import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import Layout from '../views/layout/Layout.vue'

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
      name:"layout",
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

// 导航守卫：访问 / 时自动跳转到 /dashboard
export default router
