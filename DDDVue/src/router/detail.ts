import { ref } from 'vue'
import * as menuApi from '@/api/menu'
import type { RouteConfig } from '@/api/menu'
import Login from '../views/Login.vue'
import Error from '../views/Error/404.vue'
import Layout from '../views/layout/Layout.vue'
import ClearCache from '../views/home/ClearCache/ClearCache.vue'
import { getItem, setItem, StorageKeys } from '@/utils/storage'

// 路由数据
const routes = ref<RouteConfig[]>([])
const isLoading = ref(true)

// 组件映射表 - 将组件名称映射到实际组件
const componentMap: Record<string, any> = {
    'Login': Login,
    'Error': Error,
    'Layout': Layout,
    'ClearCache': ClearCache
}

// 使用 import.meta.glob 预加载所有 views/home 下的组件（按目录分组）
const dashboardComponents = import.meta.glob('../views/home/Dashboard/*.vue', { eager: true })
const all = import.meta.glob('../views/home.*/*.vue', { eager: true })
const profileComponents = import.meta.glob('../views/home/Profile/*.vue', { eager: true })
const clearCacheComponents = import.meta.glob('../views/home/ClearCache/*.vue', { eager: true })
const menuComponents = import.meta.glob('../views/home/menu/*.vue', { eager: true })
const settingsComponents = import.meta.glob('../views/home/Settings/*.vue', { eager: true })
const usersComponents = import.meta.glob('../views/home/Users/*.vue', { eager: true })
const productsComponents = import.meta.glob('../views/home/Products/*.vue', { eager: true })

// 合并所有组件
const allComponents = {
    ...all,
    ...dashboardComponents,
    ...profileComponents,
    ...clearCacheComponents,
    ...menuComponents,
    ...settingsComponents,
    ...usersComponents,
    ...productsComponents,
}

// 将组件路径映射到组件实例
const componentPathMap: Record<string, any> = {}
Object.keys(allComponents).forEach((key: string) => {
    // 移除路径前缀和 .vue 后缀
    let componentName = key.replace('../views/home/', '').replace('.vue', '')
    // 获取默认导出
    const component = allComponents[key].default || allComponents[key]
    componentPathMap[componentName] = component
    console.log('预加载组件:', componentName, '->', key)
})

// 从 API 获取路由配置
const loadRoutesFromAPI = async () => {
    try {
        const response = await menuApi.getRoutes()
        if (response.data && Array.isArray(response.data)) {
            routes.value = response.data
        }
    } catch (error) {
        console.error('从 API 获取路由配置失败:', error)
    } finally {
        isLoading.value = false
    }
}

// 从 localStorage 获取路由配置
const loadRoutesFromStorage = (): RouteConfig[] => {
    return getItem<RouteConfig[]>(StorageKeys.SidebarMenu) || []
}

// 保存路由配置到 localStorage
const saveRoutesToStorage = (routes: RouteConfig[]) => {
    setItem(StorageKeys.SidebarMenu, routes)
}

// 初始化路由配置
const initRoutes = async () => {
    // 优先从 localStorage 获取
    const cachedRoutes = loadRoutesFromStorage()

    if (cachedRoutes.length > 0) {
        routes.value = cachedRoutes
        isLoading.value = false
        return
    }

    // 如果 localStorage 中没有，从 API 获取
    await loadRoutesFromAPI()

    // 保存到 localStorage
    if (routes.value.length > 0) {
        saveRoutesToStorage(routes.value)
    }
}

// 导出初始化函数
export const initRouteConfig = async () => {
    await initRoutes()
    return routes.value
}

// 导出路由配置（异步获取）
export const getRoutes = async (): Promise<RouteConfig[]> => {
    if (routes.value.length === 0 && isLoading.value) {
        await initRoutes()
    }
    return routes.value
}

// 导出加载状态
export { isLoading }

// 将路由配置转换为 Vue Router 格式
export const convertToRouterFormat = (routeConfigs: RouteConfig[]): any[] => {
    return routeConfigs.map(route => {
        // 构建组件路径（处理 component 字段）
        let component: any = null
        if (route.component && route.component.trim() !== '') {
            // 调试：输出 component 值
            console.log('原始 component 值:', route.component)

            // 后端返回的 component 可能是：
            // 1. 简单组件名（如 "Dashboard"）
            // 2. 完整路径（如 "views/home/Dashboard/Dashboard.vue"）

            const componentPath = route.component.trim()
            console.log('转换后的componentPath:', componentPath)

            // 如果是完整路径，尝试转换为相对导入路径
            if (componentPath.startsWith('views/')) {
                // 移除 "views/" 前缀
                let relativePath = componentPath.replace(/^views\/home\//, '')

                // 移除 .vue 后缀
                if (relativePath.endsWith('.vue')) {
                    relativePath = relativePath.slice(0, -4)
                }

                // 移除 /index 后缀
                if (relativePath.endsWith('/index')) {
                    relativePath = relativePath.slice(0, -6)
                }

                console.log('转换后的路径:', relativePath)

                // 从预加载的组件中查找
                component = componentPathMap[relativePath] || null
            } else {
                // 简单组件名，从映射表中查找
                component = componentMap[componentPath] || null
                // 如果没有找到，尝试从预加载组件中查找
                if (!component) {
                    component = componentPathMap[componentPath] || null
                }
            }

            // 如果组件不存在，使用 404 错误页面
            if (!component) {
                console.warn(`未找到组件 "${componentPath}"，使用 404 错误页面`)
                component = Error
            }
        }

        return {
            path: route.path,
            name: route.name,
            component: component,
            icon: route.icon,
            parentId: route.parentId,
            sortOrder: route.sortOrder,
            status: route.status,
            children: route.children ? convertToRouterFormat(route.children) : undefined
        }
    })
}

// 导出路由配置数组（用于 router/index.ts）
export default routes
