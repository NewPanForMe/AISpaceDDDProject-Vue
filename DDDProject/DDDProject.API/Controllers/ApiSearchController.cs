using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using DDDProject.Application.Interfaces;
using DDDProject.Application.DTOs;

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
    [ActionName("SearchAsync")]
    public async Task<ApiRequestResult> SearchAsync()
    {
        var result = _apiSearchService.GetApiSearchList();
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Data = result
        });
    }

    /// <summary>
    /// 获取所有标记了ApiSearch注解的API（字符串格式）
    /// </summary>
    /// <returns>API列表的JSON字符串</returns>
    [HttpGet]
    [ActionName("SearchStrAsync")]
    public async Task<ApiRequestResult> SearchStrAsync()
    {
        var result = _apiSearchService.GetApiSearchListStr();
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Data = result
        });
    }

    /// <summary>
    /// 根据分类获取API列表
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns>API列表的JSON字符串</returns>
    [HttpGet]
    [ActionName("SearchByCategoryAsync")]
    public async Task<ApiRequestResult> SearchByCategoryAsync(string category)
    {
        var result = _apiSearchService.GetApiSearchListByCategory(category);
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Data = result
        });
    }

    /// <summary>
    /// 根据关键词搜索API
    /// </summary>
    /// <param name="keyword">关键词</param>
    /// <returns>API列表的JSON字符串</returns>
    [HttpGet]
    [ActionName("SearchByKeyWordAsync")]
    public async Task<ApiRequestResult> SearchByKeyWordAsync(string keyword)
    {
        var result = _apiSearchService.GetApiSearchListByKeyWord(keyword);
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Data = result
        });
    }
}