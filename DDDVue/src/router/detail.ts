import { ref } from 'vue'
import * as menuApi from '@/api/menu'
import type { RouteConfig } from '@/api/menu'
import { getItem, setItem, StorageKeys } from '@/utils/storage'

// 路由数据
const routes = ref<RouteConfig[]>([])
const isLoading = ref(true)

// 简单的 404 组件（避免循环依赖）
const ErrorComponent = {
    name: 'Error404',
    template: '<div style="padding: 40px; text-align: center;"><h1>404</h1><p>页面不存在</p></div>'
}

// 使用 import.meta.glob 预加载所有 views/home 下的组件
const allComponents = import.meta.glob('../views/home/**/*.vue', { eager: true })

// 将组件路径映射到组件实例
const componentPathMap: Record<string, any> = {}
Object.keys(allComponents).forEach((key: string) => {
    // 移除路径前缀和 .vue 后缀
    let componentName = key.replace('../views/home/', '').replace('.vue', '')
    // 获取默认导出
    const moduleDef = allComponents[key] as { default?: any }
    const component = moduleDef.default || allComponents[key]
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

// 刷新路由配置（清除缓存并重新从 API 获取）
export const refreshRoutes = async () => {
    // 清除 localStorage 缓存
    setItem(StorageKeys.SidebarMenu, [])

    // 重新从 API 获取
    await loadRoutesFromAPI()

    // 保存到 localStorage
    if (routes.value.length > 0) {
        saveRoutesToStorage(routes.value)
    }

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

// 将路由配置转换为 Vue Router 格式（扁平化处理）
export const convertToRouterFormat = (routeConfigs: RouteConfig[]): any[] => {
    const result: any[] = []

    // 递归处理路由，扁平化所有有组件的路由
    const processRoute = (route: RouteConfig) => {
        // 如果有组件，添加到结果中
        if (route.component && route.component.trim() !== '') {
            let component: any = null

            console.log('原始 component 值:', route.component)

            // 统一将反斜杠转换为正斜杠
            let componentPath = route.component.trim().replace(/\\/g, '/')
            console.log('转换后的componentPath:', componentPath)

            // 如果是完整路径，尝试转换为相对导入路径
            if (componentPath.startsWith('views/')) {
                let relativePath = componentPath.replace(/^views\/home\//, '')

                if (relativePath.endsWith('.vue')) {
                    relativePath = relativePath.slice(0, -4)
                }

                if (relativePath.endsWith('/index')) {
                    relativePath = relativePath.slice(0, -6)
                }

                console.log('转换后的路径:', relativePath)
                component = componentPathMap[relativePath] || null
            } else {
                // 目录/文件格式或简单组件名，直接从预加载组件中查找
                component = componentPathMap[componentPath] || null
            }

            if (!component) {
                console.warn(`未找到组件 "${componentPath}"，使用 404 错误页面`)
                component = ErrorComponent
            }

            result.push({
                path: route.path,
                name: route.name,
                component: component,
                meta: {
                    icon: route.icon,
                    parentId: route.parentId,
                    sortOrder: route.sortOrder,
                    status: route.status
                }
            })
        }

        // 递归处理子路由
        if (route.children && route.children.length > 0) {
            route.children.forEach(child => processRoute(child))
        }
    }

    routeConfigs.forEach(route => processRoute(route))
    return result
}

// 导出路由配置数组（用于 router/index.ts）
export default routes
