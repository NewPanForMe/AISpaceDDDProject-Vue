using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using DDDProject.Application.Interfaces;

namespace DDDProject.API.Controllers;

/// <summary>
/// API搜索控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class ApiSearchController : BaseApiController
{
    private readonly IApiSearchService _apiSearchService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="apiSearchService">API搜索服务</param>
    public ApiSearchController(IApiSearchService apiSearchService)
    {
        _apiSearchService = apiSearchService;
    }

    /// <summary>
    /// 获取所有标记了ApiSearch注解的API
    /// </summary>
    /// <returns>API列表的JSON字符串</returns>
    [HttpGet]
    public string Search()
    {
        return _apiSearchService.GetApiSearchList();
    }
    /// <summary>
    /// 获取所有标记了ApiSearch注解的API
    /// </summary>
    /// <returns>API列表的JSON字符串</returns>
    [HttpGet]
    public string SearchStr()
    {
        return _apiSearchService.GetApiSearchListStr();
    }

    /// <summary>
    /// 根据分类获取API列表
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns>API列表的JSON字符串</returns>
    [HttpGet]
    public string SearchByCategory(string category)
    {
        return _apiSearchService.GetApiSearchListByCategory(category);
    }

    /// <summary>
    /// 根据关键词搜索API
    /// </summary>
    /// <param name="keyword">关键词</param>
    /// <returns>API列表的JSON字符串</returns>
    [HttpGet]
    public string SearchByKeyWord(string keyword)
    {
        return _apiSearchService.GetApiSearchListByKeyWord(keyword);
    }
}
