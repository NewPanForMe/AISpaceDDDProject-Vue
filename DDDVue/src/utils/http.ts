import axios from "axios";
import { showNotification, showErrorNotification } from "./notification";
import router from "@/router";
import { getItem, removeItem, StorageKeys } from "@/utils/storage";

// 创建 axios 实例
const service = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5272/",
  timeout: 15000,
  headers: {
    "Content-Type": "application/json",
  },
});

// 请求拦截器
service.interceptors.request.use(
  (config) => {
    const token = getItem<string>(StorageKeys.Token);
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    showErrorNotification({
      title: "请求错误",
      message: error.message || "请求发送失败",
    });
    return Promise.reject(error);
  },
);

// 将 PascalCase 转换为 camelCase
const toCamelCase = (str: string): string => {
  if (!str) return str
  // 处理首字母小写
  return str.charAt(0).toLowerCase() + str.slice(1)
}

// 递归转换对象属性名为 camelCase
const convertToCamelCase = (obj: any): any => {
  if (Array.isArray(obj)) {
    return obj.map(item => convertToCamelCase(item))
  } else if (obj !== null && typeof obj === 'object') {
    const converted: any = {}
    for (const key in obj) {
      if (Object.prototype.hasOwnProperty.call(obj, key)) {
        const camelKey = toCamelCase(key)
        converted[camelKey] = convertToCamelCase(obj[key])
      }
    }
    return converted
  }
  return obj
}

// 响应拦截器
service.interceptors.response.use(
  (response) => {
    // 如果是 blob 类型，直接返回原始数据
    if (response.config.responseType === 'blob') {
      return response.data
    }
    // 转换后端返回的 PascalCase 属性名为 camelCase
    const data = convertToCamelCase(response.data)
    return data
  },
  (error) => {
    let title = "请求失败";
    let message = error.message || "未知错误";
    let type: "success" | "warning" | "info" | "error" = "error";

    if (axios.isAxiosError(error) && error.response) {
      const status = error.response.status;

      switch (status) {
        case 401:
          removeItem(StorageKeys.Token);
          removeItem(StorageKeys.UserInfo);
          title = "未授权";
          message = "登录已过期或未登录，请重新登录";
          type = "warning";
          // 延迟跳转，让用户看到提示信息
          setTimeout(() => {
            router.push('/');
          }, 1500);
          break;
        case 403:
          title = "禁止访问";
          message = "您没有权限访问此资源";
          type = "warning";
          break;
        case 404:
          title = "资源不存在";
          message = "请求的资源不存在";
          type = "info";
          break;
        case 500:
          title = "服务器错误";
          message = "服务器内部错误，请稍后重试";
          type = "error";
          break;
        case 502:
          title = "网关错误";
          message = "网关错误，请稍后重试";
          type = "error";
          break;
        case 503:
          title = "服务不可用";
          message = "服务暂时不可用，请稍后重试";
          type = "error";
          break;
        case 504:
          title = "网关超时";
          message = "网关超时，请稍后重试";
          type = "error";
          break;
        default:
          title = `错误状态码: ${status}`;
          message = error.response.data?.message || "请求失败";
          type = "error";
      }
    } else if (error.code === "ECONNABORTED") {
      title = "请求超时";
      message = "请求超时，请检查网络连接";
      type = "warning";
    } else if (error.code === "ERR_NETWORK") {
      title = "网络错误";
      message = "网络连接失败，请检查网络连接";
      type = "error";
    }

    showNotification({
      title,
      message,
      type,
      duration: 3000,
    });

    return Promise.reject(error);
  },
);

// 导出 axios 实例
export default service;

// 定义类型安全的 HTTP 方法
export const http = {
  get: <T = any>(url: string, config?: any) => service.get<T>(url, config),
  post: <T = any>(url: string, data?: any, config?: any) => service.post<any, T>(url, data, config),
  put: <T = any>(url: string, data?: any, config?: any) => service.put<any, T>(url, data, config),
  delete: <T = any>(url: string, config?: any) => service.delete<any, T>(url, config),
  patch: <T = any>(url: string, data?: any, config?: any) => service.patch<any, T>(url, data, config),
};
