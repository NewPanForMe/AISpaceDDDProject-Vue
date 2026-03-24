import type { RouteRecordRaw } from 'vue-router'

// 菜单接口
export interface MenuItem {
  id?: string | number
  name: string
  path: string
  component?: string
  icon?: string
  parentId?: string | number
  children?: MenuItem[]
}

/**
 * 将菜单树转换为 Vue Router 路由配置（单层路由）
 * @param menus 菜单数组
 * @returns 路由配置数组
 */
export const generateRoutesFromMenus = (menus: MenuItem[]): RouteRecordRaw[] => {
  const routes: RouteRecordRaw[] = []

  for (const menu of menus) {
    // 构建路由路径（单层路由，不拼接父级路径）
    const fullPath = menu.path.startsWith('/') ? menu.path : `/${menu.path}`

    // 处理组件路径
    let component: any = null
    if (menu.component) {
      // 移除前后斜杠
      let componentPath = menu.component.replace(/^\/|\/$/g, '')
      
      component = () => import(`@/views/home/${componentPath}.vue`)
    }

    // 创建路由配置
    const route: RouteRecordRaw = {
      path: fullPath,
      name: menu.name,
      component: component,
      meta: {
        icon: menu.icon,
        title: menu.name
      }
    }

    // 如果有子菜单，递归生成路由（子菜单也作为单层路由）
    if (menu.children && menu.children.length > 0) {
      const childRoutes = generateRoutesFromMenus(menu.children)
      routes.push(...childRoutes)
    }

    routes.push(route)
  }

  return routes
}

/**
 * 从 localStorage 获取菜单数据并生成路由
 * @returns 路由配置数组
 */
export const getRoutesFromStorage = (): RouteRecordRaw[] => {
  try {
    const storedMenu = localStorage.getItem('sidebarMenu')
    if (storedMenu) {
      const menuData = JSON.parse(storedMenu)
      return generateRoutesFromMenus(menuData)
    }
  } catch (e) {
    console.error('解析菜单数据失败:', e)
  }
  return []
}
