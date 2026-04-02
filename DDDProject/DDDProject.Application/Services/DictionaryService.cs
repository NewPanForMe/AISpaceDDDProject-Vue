using DDDProject.Application.Common;
using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Domain.Entities;
using DDDProject.Domain.Repositories;

namespace DDDProject.Application.Services;

/// <summary>
/// 字典服务实现
/// </summary>
public class DictionaryService : IDictionaryService
{
    private readonly IRepository<Dictionary> _repository;

    public DictionaryService(IRepository<Dictionary> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 获取字典列表（分页、支持筛选）
    /// </summary>
    public async Task<ApiRequestResult> GetDictionariesAsync(DictionaryQueryRequest request)
    {
        var skipCount = (request.PageNumber - 1) * request.PageSize;

        // 构建查询条件
        var dictionaries = await _repository.GetListAsync(
            d => true,
            q => q.OrderBy(d => d.Type).ThenBy(d => d.SortOrder),
            skipCount,
            request.PageSize
        );

        // 筛选
        var filteredList = dictionaries.ToList();

        if (!string.IsNullOrEmpty(request.Code))
        {
            filteredList = filteredList.Where(d => d.Code.Contains(request.Code)).ToList();
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            filteredList = filteredList.Where(d => d.Name.Contains(request.Name)).ToList();
        }

        if (!string.IsNullOrEmpty(request.Type))
        {
            filteredList = filteredList.Where(d => d.Type == request.Type).ToList();
        }

        if (request.Status.HasValue)
        {
            filteredList = filteredList.Where(d => d.Status == request.Status.Value).ToList();
        }

        // 获取总数
        var total = await _repository.CountAsync(d => true);

        // 转换为 DTO
        var dictionaryDtos = filteredList.Select(d => new DictionaryDto
        {
            Id = d.Id,
            Code = d.Code,
            Name = d.Name,
            Value = d.Value,
            Type = d.Type,
            Status = d.Status,
            SortOrder = d.SortOrder,
            Description = d.Description,
            Remark = d.Remark,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        }).ToList();

        var pagedResult = new PagedResult<DictionaryDto>
        {
            List = dictionaryDtos,
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = pagedResult };
    }

    /// <summary>
    /// 获取字典详情
    /// </summary>
    public async Task<ApiRequestResult> GetDictionaryByIdAsync(Guid id)
    {
        var dictionary = await _repository.FindAsync(id);
        if (dictionary is null)
        {
            return new ApiRequestResult { Success = false, Message = "字典不存在" };
        }

        var dictionaryDto = new DictionaryDto
        {
            Id = dictionary.Id,
            Code = dictionary.Code,
            Name = dictionary.Name,
            Value = dictionary.Value,
            Type = dictionary.Type,
            Status = dictionary.Status,
            SortOrder = dictionary.SortOrder,
            Description = dictionary.Description,
            Remark = dictionary.Remark,
            CreatedAt = dictionary.CreatedAt,
            UpdatedAt = dictionary.UpdatedAt
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = dictionaryDto };
    }

    /// <summary>
    /// 根据编码获取字典
    /// </summary>
    public async Task<ApiRequestResult> GetDictionaryByCodeAsync(string code)
    {
        var dictionary = await _repository.GetFirstAsync(d => d.Code == code);
        if (dictionary is null)
        {
            return new ApiRequestResult { Success = false, Message = "字典不存在" };
        }

        var dictionaryDto = new DictionaryDto
        {
            Id = dictionary.Id,
            Code = dictionary.Code,
            Name = dictionary.Name,
            Value = dictionary.Value,
            Type = dictionary.Type,
            Status = dictionary.Status,
            SortOrder = dictionary.SortOrder,
            Description = dictionary.Description,
            Remark = dictionary.Remark,
            CreatedAt = dictionary.CreatedAt,
            UpdatedAt = dictionary.UpdatedAt
        };

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = dictionaryDto };
    }

    /// <summary>
    /// 根据类型获取字典列表
    /// </summary>
    public async Task<ApiRequestResult> GetDictionariesByTypeAsync(string type)
    {
        var dictionaries = await _repository.GetListAsync(d => d.Type == type && d.Status == 1);
        var dictionaryDtos = dictionaries.Select(d => new DictionaryDto
        {
            Id = d.Id,
            Code = d.Code,
            Name = d.Name,
            Value = d.Value,
            Type = d.Type,
            Status = d.Status,
            SortOrder = d.SortOrder,
            Description = d.Description,
            Remark = d.Remark,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        }).OrderBy(d => d.SortOrder).ToList();

        return new ApiRequestResult { Success = true, Message = "操作成功", Data = dictionaryDtos };
    }

    /// <summary>
    /// 创建字典
    /// </summary>
    public async Task<ApiRequestResult> CreateDictionaryAsync(CreateDictionaryRequest request)
    {
        // 检查编码是否已存在
        var existingDictionary = await _repository.GetFirstAsync(d => d.Code == request.Code);
        if (existingDictionary is not null)
        {
            return new ApiRequestResult { Success = false, Message = "字典编码已存在" };
        }

        var dictionary = Dictionary.Create(
            request.Code,
            request.Name,
            request.Value,
            request.Type,
            request.SortOrder,
            request.Description
        );

        if (!string.IsNullOrEmpty(request.Remark))
        {
            dictionary.Update(remark: request.Remark);
        }

        await _repository.AddAsync(dictionary);
        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "创建成功", Data = dictionary.Id };
    }

    /// <summary>
    /// 更新字典
    /// </summary>
    public async Task<ApiRequestResult> UpdateDictionaryAsync(UpdateDictionaryRequest request)
    {
        var dictionary = await _repository.FindAsync(request.Id);
        if (dictionary is null)
        {
            return new ApiRequestResult { Success = false, Message = "字典不存在" };
        }

        // 检查编码是否已被其他字典使用
        if (!string.IsNullOrEmpty(request.Code))
        {
            var existingDictionary = await _repository.GetFirstAsync(d => d.Code == request.Code && d.Id != request.Id);
            if (existingDictionary is not null)
            {
                return new ApiRequestResult { Success = false, Message = "字典编码已被使用" };
            }
        }

        dictionary.Update(
            request.Code,
            request.Name,
            request.Value,
            request.Type,
            request.SortOrder,
            request.Description,
            request.Remark
        );

        _repository.Update(dictionary);
        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "更新成功" };
    }

    /// <summary>
    /// 删除字典
    /// </summary>
    public async Task<ApiRequestResult> DeleteDictionaryAsync(Guid id)
    {
        var dictionary = await _repository.FindAsync(id);
        if (dictionary is null)
        {
            return new ApiRequestResult { Success = false, Message = "字典不存在" };
        }

        _repository.Remove(dictionary);
        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "删除成功" };
    }

    /// <summary>
    /// 启用字典
    /// </summary>
    public async Task<ApiRequestResult> EnableDictionaryAsync(Guid id)
    {
        var dictionary = await _repository.FindAsync(id);
        if (dictionary is null)
        {
            return new ApiRequestResult { Success = false, Message = "字典不存在" };
        }

        dictionary.Enable();
        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "启用成功" };
    }

    /// <summary>
    /// 禁用字典
    /// </summary>
    public async Task<ApiRequestResult> DisableDictionaryAsync(Guid id)
    {
        var dictionary = await _repository.FindAsync(id);
        if (dictionary is null)
        {
            return new ApiRequestResult { Success = false, Message = "字典不存在" };
        }

        dictionary.Disable();
        await _repository.SaveChangesAsync();

        return new ApiRequestResult { Success = true, Message = "禁用成功" };
    }
}