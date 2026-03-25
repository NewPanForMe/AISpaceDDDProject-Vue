using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using DDDProject.Application.Common;
using DDDProject.Application.Interfaces;

namespace DDDProject.Application.Services;

/// <summary>
/// API搜索服务
/// </summary>
public class ApiSearchService : IApiSearchService
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    public ApiSearchService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 获取所有标记了ApiSearch注解的API列表
    /// </summary>
    /// <returns>JSON字符串</returns>
    public string GetApiSearchList()
    {
        var apiList = GetApiSearchAttributes();
        return JsonSerializer.Serialize(apiList, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// 根据分类获取API列表
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns>JSON字符串</returns>
    public string GetApiSearchListByCategory(string category)
    {
        var apiList = GetApiSearchAttributes()
            .Where(x => string.IsNullOrEmpty(category) ||
                   (x.Category is not null && x.Category.Contains(category, StringComparison.OrdinalIgnoreCase)) ||
                   Enum.TryParse<ApiSearchCategory>(category, true, out var enumValue) && x.Category == enumValue.ToString())
            .ToList();
        return JsonSerializer.Serialize(apiList, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// 根据关键词搜索API列表
    /// </summary>
    /// <param name="keyword">关键词</param>
    /// <returns>JSON字符串</returns>
    public string GetApiSearchListByKeyWord(string keyword)
    {
        var apiList = GetApiSearchAttributes()
            .Where(x => string.IsNullOrEmpty(keyword) ||
                   (x.Name is not null && x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                   (x.Description is not null && x.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                   (x.ControllerName is not null && x.ControllerName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        return JsonSerializer.Serialize(apiList, new JsonSerializerOptions { WriteIndented = true });
    }

    public string GetApiSearchListStr()
    {
        return GetApiSearchAttributesStr();
    }

    /// <summary>
    /// 获取所有标记了ApiSearch注解的方法
    /// </summary>
    /// <returns>API信息列表</returns>
    private List<ApiSearchInfo> GetApiSearchAttributes()
    {
        var apiList = new List<ApiSearchInfo>();

        // 获取所有控制器类型
        var controllerTypes = Assembly.Load("DDDProject.API")
            .GetTypes()
            .Where(t => t.Name.EndsWith("Controller") && t.IsClass && !t.IsAbstract);

        foreach (var controllerType in controllerTypes)
        {
            // 获取控制器上的路由
            var controllerRoute = controllerType.GetCustomAttributes(true)
                .OfType<RouteAttribute>()
                .FirstOrDefault()?.Template;

            // 获取控制器名称
            var controllerName = controllerType.Name.Replace("Controller", "");

            foreach (var method in controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                // 获取方法上的ApiSearch注解
                var apiSearchAttr = method.GetCustomAttribute<ApiSearchAttribute>();
                if (apiSearchAttr is null)
                {
                    continue;
                }

                // 获取方法上的HTTP动词属性
                var httpMethod = GetHttpMethodName(method);

                // 构建完整的API路径
                var actionRoute = method.GetCustomAttributes(true)
                    .OfType<HttpGetAttribute>()
                    .FirstOrDefault()?.Template ??
                    method.GetCustomAttributes(true)
                    .OfType<HttpPostAttribute>()
                    .FirstOrDefault()?.Template ??
                    method.GetCustomAttributes(true)
                    .OfType<HttpPutAttribute>()
                    .FirstOrDefault()?.Template ??
                    method.GetCustomAttributes(true)
                    .OfType<HttpDeleteAttribute>()
                    .FirstOrDefault()?.Template ?? "";

                var fullPath = $"/{controllerRoute?.Replace("[controller]", controllerName) ?? $"/{controllerName}"}";
                if (!string.IsNullOrEmpty(actionRoute))
                {
                    fullPath += $"/{actionRoute}";
                }

                apiList.Add(new ApiSearchInfo
                {
                    ControllerName = controllerName,
                    ActionName = method.Name,
                    Name = apiSearchAttr.Name,
                    Description = apiSearchAttr.Description,
                    Category = apiSearchAttr.Category.ToString(),
                    CategoryEnum = apiSearchAttr.Category,
                    HttpMethod = httpMethod,
                    Path = fullPath
                });
            }
        }



        return apiList;
    }



    /// <summary>
    /// 获取所有标记了ApiSearch注解的方法
    /// </summary>
    /// <returns>API信息列表</returns>
    private string GetApiSearchAttributesStr()
    {
        var res = "const api={内容}; export default api;";
        var str = string.Empty;

        // 获取所有控制器类型
        var controllerTypes = Assembly.Load("DDDProject.API")
            .GetTypes()
            .Where(t => t.Name.EndsWith("Controller") && t.IsClass && !t.IsAbstract);

        foreach (var controllerType in controllerTypes)
        {
            // 获取控制器上的路由
            var controllerRoute = controllerType.GetCustomAttributes(true)
                .OfType<RouteAttribute>()
                .FirstOrDefault()?.Template;
            // 获取控制器名称
            var controllerName = controllerType.Name.Replace("Controller", "");
            str += controllerName + ":{";
            foreach (var method in controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                // 获取方法上的ApiSearch注解
                var apiSearchAttr = method.GetCustomAttribute<ApiSearchAttribute>();
                var methodName = method.Name;
                if (apiSearchAttr is null)
                {
                    continue;
                }

                var fullPath = controllerRoute!.Replace("[controller]", controllerName);
                fullPath = fullPath!.Replace("[action]", methodName);
                str += methodName + $":'{fullPath}',";
            }
            str += "},";
        }
        return res.Replace("内容", str);
    }

    /// <summary>
    /// 获取HTTP方法名称
    /// </summary>
    private string GetHttpMethodName(MethodInfo method)
    {
        if (method.GetCustomAttribute<HttpGetAttribute>() is not null) return "GET";
        if (method.GetCustomAttribute<HttpPostAttribute>() is not null) return "POST";
        if (method.GetCustomAttribute<HttpPutAttribute>() is not null) return "PUT";
        if (method.GetCustomAttribute<HttpDeleteAttribute>() is not null) return "DELETE";
        if (method.GetCustomAttribute<HttpPatchAttribute>() is not null) return "PATCH";
        return "UNKNOWN";
    }
}

/// <summary>
/// API搜索信息
/// </summary>
public class ApiSearchInfo
{
    /// <summary>
    /// 控制器名称
    /// </summary>
    public string ControllerName { get; set; } = string.Empty;

    /// <summary>
    /// 方法名称
    /// </summary>
    public string ActionName { get; set; } = string.Empty;

    /// <summary>
    /// API名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// API描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// API分类
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// API分类枚举
    /// </summary>
    public ApiSearchCategory CategoryEnum { get; set; }

    /// <summary>
    /// HTTP方法
    /// </summary>
    public string HttpMethod { get; set; } = string.Empty;

    /// <summary>
    /// API路径
    /// </summary>
    public string Path { get; set; } = string.Empty;
}
