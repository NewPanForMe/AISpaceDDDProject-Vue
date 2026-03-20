using System;

namespace DDDProject.Application.Common;

/// <summary>
/// API搜索注解，用于标记需要搜索的API方法
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ApiSearchAttribute : Attribute
{
    /// <summary>
    /// API名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// API描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// API分类
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public ApiSearchAttribute()
    {
        Name = "";
        Description = "";
        Category = "";
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name">API名称</param>
    /// <param name="description">API描述</param>
    /// <param name="category">API分类</param>
    public ApiSearchAttribute(string name, string description = "", string category = "")
    {
        Name = name;
        Description = description;
        Category = category;
    }
}
