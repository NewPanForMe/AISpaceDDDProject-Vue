// API 接口统一管理文件
// 自动生成于 2026-03-19
// 接口文档来源: http://localhost:5272/api/ApiSearch/SearchStr

// API 配置接口定义
export interface ApiConfig {
  ApiSearch: Record<string, any>;
  BaseApi: Record<string, any>;
  Login: {
    Login: string;
  };
}

// API 配置对象
const api: ApiConfig = {
  ApiSearch: {},
  BaseApi: {},
  Login: {
    // 登录接口
    Login: 'api/Login/LoginAsync',
  },
};

export default api;
