import axios from 'axios';
import { showNotification, showErrorNotification } from './notification';

// 创建 axios 实例
const service = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5272', // 默认后端地址
  timeout: 15000, // 请求超时时间
  headers: {
    'Content-Type': 'application/json'
  }
});

// 请求拦截器
service.interceptors.request.use(
  (config) => {
    // 在请求发送前进行预处理
    // 可以在这里添加 token、加载状态等

    // 从 localStorage 获取 token 并添加到请求头
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    // 请求错误处理
    showErrorNotification({
      title: '请求错误',
      message: error.message || '请求发送失败'
    });
    return Promise.reject(error);
  }
);

// 响应拦截器
service.interceptors.response.use(
  (response) => {
    // 对响应数据进行预处理
    const res = response.data;

    // 如果返回的不是二进制文件类型，进行统一处理
    if (response.headers['content-type'] !== 'application/octet-stream') {
      // 统一处理响应格式
      // 根据后端返回的格式进行调整
      return res;
    }

    return res;
  },
  (error) => {
    // 响应错误处理
    let title = '请求失败';
    let message = error.message || '未知错误';
    let type = 'error';

    // 处理 HTTP 状态码错误
    if (error.response) {
      const status = error.response.status;

      switch (status) {
        case 401:
          // 未授权，清除 token 并跳转登录
          localStorage.removeItem('token');
          title = '未授权';
          message = '登录已过期或未登录，请重新登录';
          type = 'warning';
          break;
        case 403:
          // 禁止访问
          title = '禁止访问';
          message = '您没有权限访问此资源';
          type = 'warning';
          break;
        case 404:
          // 资源不存在
          title = '资源不存在';
          message = '请求的资源不存在';
          type = 'info';
          break;
        case 500:
          // 服务器错误
          title = '服务器错误';
          message = '服务器内部错误，请稍后重试';
          type = 'error';
          break;
        case 502:
          title = '网关错误';
          message = '网关错误，请稍后重试';
          type = 'error';
          break;
        case 503:
          title = '服务不可用';
          message = '服务暂时不可用，请稍后重试';
          type = 'error';
          break;
        case 504:
          title = '网关超时';
          message = '网关超时，请稍后重试';
          type = 'error';
          break;
        default:
          title = `错误状态码: ${status}`;
          message = error.response.data?.message || '请求失败';
          type = 'error';
      }
    } else if (error.code === 'ECONNABORTED') {
      // 请求超时
      title = '请求超时';
      message = '请求超时，请检查网络连接';
      type = 'warning';
    } else if (error.code === 'ERR_NETWORK') {
      // 网络错误
      title = '网络错误';
      message = '网络连接失败，请检查网络连接';
      type = 'error';
    }

    showNotification({
      title,
      message,
      type,
      duration: 3000
    });

    return Promise.reject(error);
  }
);

// 导出 axios 实例
export default service;

// 导出常用的 HTTP 方法
export const http = {
  get: service.get,
  post: service.post,
  put: service.put,
  delete: service.delete,
  patch: service.patch
};
