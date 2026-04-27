using Microsoft.AspNetCore.Mvc;
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;
using Microsoft.AspNetCore.Authorization;

namespace DDDProject.API.Controllers;

/// <summary>
/// 字典管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class DictionaryController : BaseApiController
{
    private readonly IDictionaryService _dictionaryService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dictionaryService">字典服务</param>
    public DictionaryController(IDictionaryService dictionaryService)
    {
        _dictionaryService = dictionaryService;
    }

    /// <summary>
    /// 获取字典列表（分页）
    /// </summary>
    [HttpGet]
    [ActionName("GetDictionariesAsync")]
    [ApiSearch(Name = "获取字典列表", Description = "返回字典列表（支持分页和筛选）", Category = ApiSearchCategory.Dictionary)]
    public async Task<ApiRequestResult> GetDictionariesAsync([FromQuery] int pageNum = 1, [FromQuery] int pageSize = 10, [FromQuery] string? code = null, [FromQuery] string? name = null, [FromQuery] string? type = null, [FromQuery] int? status = null)
    {
        var request = new DictionaryQueryRequest
        {
            PageNumber = pageNum,
            PageSize = pageSize,
            Code = code,
            Name = name,
            Type = type,
            Status = status
        };
        return await _dictionaryService.GetDictionariesAsync(request);
    }

    /// <summary>
    /// 获取字典详情
    /// </summary>
    [HttpGet]
    [ActionName("GetDictionaryByIdAsync")]
    [ApiSearch(Name = "获取字典详情", Description = "根据ID获取字典详细信息", Category = ApiSearchCategory.Dictionary)]
    public async Task<ApiRequestResult> GetDictionaryByIdAsync([FromQuery] Guid id)
    {
        return await _dictionaryService.GetDictionaryByIdAsync(id);
    }

    /// <summary>
    /// 根据编码获取字典
    /// </summary>
    [HttpGet]
    [ActionName("GetDictionaryByCodeAsync")]
    [ApiSearch(Name = "根据编码获取字典", Description = "根据编码获取字典信息", Category = ApiSearchCategory.Dictionary)]
    public async Task<ApiRequestResult> GetDictionaryByCodeAsync([FromQuery] string code)
    {
        return await _dictionaryService.GetDictionaryByCodeAsync(code);
    }

    /// <summary>
    /// 根据类型获取字典列表
    /// </summary>
    [HttpGet]
    [ActionName("GetDictionariesByTypeAsync")]
    [ApiSearch(Name = "根据类型获取字典", Description = "根据类型获取字典列表", Category = ApiSearchCategory.Dictionary)]
    public async Task<ApiRequestResult> GetDictionariesByTypeAsync([FromQuery] string type)
    {
        return await _dictionaryService.GetDictionariesByTypeAsync(type);
    }

    /// <summary>
    /// 批量根据类型获取字典列表
    /// </summary>
    [HttpPost]
    [ActionName("GetDictionariesByTypesAsync")]
    [ApiSearch(Name = "批量根据类型获取字典", Description = "根据多个类型批量获取字典列表，返回按类型分组的字典数据", Category = ApiSearchCategory.Dictionary)]
    public async Task<ApiRequestResult> GetDictionariesByTypesAsync([FromBody] List<string> types)
    {
        return await _dictionaryService.GetDictionariesByTypesAsync(types);
    }

    /// <summary>
    /// 创建字典
    /// </summary>
    [HttpPost]
    [ActionName("CreateDictionaryAsync")]
    [Permission("dictionary:add")]
    [ApiSearch(Name = "创建字典", Description = "创建新的字典", Category = ApiSearchCategory.Dictionary)]
    public async Task<ApiRequestResult> CreateDictionaryAsync([FromBody] CreateDictionaryRequest request)
    {
        return await _dictionaryService.CreateDictionaryAsync(request);
    }

    /// <summary>
    /// 更新字典
    /// </summary>
    [HttpPut]
    [ActionName("UpdateDictionaryAsync")]
    [Permission("dictionary:edit")]
    [ApiSearch(Name = "更新字典", Description = "更新现有字典", Category = ApiSearchCategory.Dictionary)]
    public async Task<ApiRequestResult> UpdateDictionaryAsync([FromBody] UpdateDictionaryRequest request)
    {
        return await _dictionaryService.UpdateDictionaryAsync(request);
    }

    /// <summary>
    /// 删除字典
    /// </summary>
    [HttpDelete]
    [ActionName("DeleteDictionaryAsync")]
    [Permission("dictionary:delete")]
    [ApiSearch(Name = "删除字典", Description = "根据ID删除字典", Category = ApiSearchCategory.Dictionary)]
    public async Task<ApiRequestResult> DeleteDictionaryAsync([FromQuery] Guid id)
    {
        return await _dictionaryService.DeleteDictionaryAsync(id);
    }

    /// <summary>
    /// 启用字典
    /// </summary>
    [HttpPost]
    [ActionName("EnableDictionaryAsync")]
    [Permission("dictionary:enable")]
    [ApiSearch(Name = "启用字典", Description = "启用一个被禁用的字典", Category = ApiSearchCategory.Dictionary)]
    public async Task<ApiRequestResult> EnableDictionaryAsync([FromQuery] Guid id)
    {
        return await _dictionaryService.EnableDictionaryAsync(id);
    }

    /// <summary>
    /// 禁用字典
    /// </summary>
    [HttpPost]
    [ActionName("DisableDictionaryAsync")]
    [Permission("dictionary:disable")]
    [ApiSearch(Name = "禁用字典", Description = "禁用一个启用的字典", Category = ApiSearchCategory.Dictionary)]
    public async Task<ApiRequestResult> DisableDictionaryAsync([FromQuery] Guid id)
    {
        return await _dictionaryService.DisableDictionaryAsync(id);
    }
}