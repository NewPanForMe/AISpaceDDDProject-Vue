using DDDProject.Application.Common;
using DDDProject.Application.DTOs;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// 字典服务接口
/// </summary>
public interface IDictionaryService : IApplicationService
{
    /// <summary>
    /// 获取字典列表（分页、支持筛选）
    /// </summary>
    Task<ApiRequestResult> GetDictionariesAsync(DictionaryQueryRequest request);

    /// <summary>
    /// 获取字典详情
    /// </summary>
    Task<ApiRequestResult> GetDictionaryByIdAsync(Guid id);

    /// <summary>
    /// 根据编码获取字典
    /// </summary>
    Task<ApiRequestResult> GetDictionaryByCodeAsync(string code);

    /// <summary>
    /// 根据类型获取字典列表
    /// </summary>
    Task<ApiRequestResult> GetDictionariesByTypeAsync(string type);

    /// <summary>
    /// 批量根据类型获取字典列表
    /// </summary>
    Task<ApiRequestResult> GetDictionariesByTypesAsync(List<string> types);

    /// <summary>
    /// 创建字典
    /// </summary>
    Task<ApiRequestResult> CreateDictionaryAsync(CreateDictionaryRequest request);

    /// <summary>
    /// 更新字典
    /// </summary>
    Task<ApiRequestResult> UpdateDictionaryAsync(UpdateDictionaryRequest request);

    /// <summary>
    /// 删除字典
    /// </summary>
    Task<ApiRequestResult> DeleteDictionaryAsync(Guid id);

    /// <summary>
    /// 启用字典
    /// </summary>
    Task<ApiRequestResult> EnableDictionaryAsync(Guid id);

    /// <summary>
    /// 禁用字典
    /// </summary>
    Task<ApiRequestResult> DisableDictionaryAsync(Guid id);
}