using DDDProject.Application.Common;

namespace DDDProject.Application.Interfaces;

/// <summary>
/// API搜索服务接口
/// </summary>
public interface IApiSearchService : IApplicationService
{
    /// <summary>
    /// 获取所有标记了ApiSearch注解的API列表
    /// </summary>
    /// <returns>JSON字符串</returns>
    string GetApiSearchList();
    /// <summary>
    /// 获取所有标记了ApiSearch注解的API列表
    /// </summary>
    /// <returns></returns>
    string GetApiSearchListStr();

    /// <summary>
    /// 根据分类获取API列表
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns>JSON字符串</returns>
    string GetApiSearchListByCategory(string category);

    /// <summary>
    /// 根据关键词搜索API列表
    /// </summary>
    /// <param name="keyword">关键词</param>
    /// <returns>JSON字符串</returns>
    string GetApiSearchListByKeyWord(string keyword);
}
