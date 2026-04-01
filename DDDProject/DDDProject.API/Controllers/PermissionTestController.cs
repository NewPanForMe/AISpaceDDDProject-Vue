using Microsoft.AspNetCore.Mvc;
using DDDProject.Application.Common;
using DDDProject.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DDDProject.API.Controllers;

/// <summary>
/// 权限测试控制器 - 用于测试 Permission 注解的各种用法
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class PermissionTestController : BaseApiController
{
    /// <summary>
    /// 测试单个权限 - 需要 test:single 权限
    /// </summary>
    [HttpGet]
    [ActionName("TestSinglePermissionAsync")]
    [Permission("test:single")]
    public async Task<ApiRequestResult> TestSinglePermissionAsync()
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "单个权限验证通过！您拥有 test:single 权限",
            Data = new { PermissionCode = "test:single", Timestamp = DateTime.Now }
        });
    }

    /// <summary>
    /// 测试多个权限（满足任一）- 需要 test:read 或 test:admin 任一权限
    /// </summary>
    [HttpGet]
    [ActionName("TestAnyPermissionAsync")]
    [Permission(new[] { "test:read", "test:admin" }, PermissionLogic.Any)]
    public async Task<ApiRequestResult> TestAnyPermissionAsync()
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "多权限（Any）验证通过！您拥有 test:read 或 test:admin 权限",
            Data = new { PermissionCodes = new[] { "test:read", "test:admin" }, Logic = "Any", Timestamp = DateTime.Now }
        });
    }

    /// <summary>
    /// 测试多个权限（满足所有）- 需要同时拥有 test:write 和 test:admin 权限
    /// </summary>
    [HttpGet]
    [ActionName("TestAllPermissionAsync")]
    [Permission(new[] { "test:write", "test:admin" }, PermissionLogic.All)]
    public async Task<ApiRequestResult> TestAllPermissionAsync()
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "多权限（All）验证通过！您同时拥有 test:write 和 test:admin 权限",
            Data = new { PermissionCodes = new[] { "test:write", "test:admin" }, Logic = "All", Timestamp = DateTime.Now }
        });
    }

    /// <summary>
    /// 测试创建权限
    /// </summary>
    [HttpPost]
    [ActionName("TestCreateAsync")]
    [Permission("test:create")]
    public async Task<ApiRequestResult> TestCreateAsync([FromBody] object data)
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "创建权限验证通过！您拥有 test:create 权限",
            Data = new { PermissionCode = "test:create", ReceivedData = data, Timestamp = DateTime.Now }
        });
    }

    /// <summary>
    /// 测试更新权限
    /// </summary>
    [HttpPut]
    [ActionName("TestUpdateAsync")]
    [Permission("test:update")]
    public async Task<ApiRequestResult> TestUpdateAsync([FromBody] object data)
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "更新权限验证通过！您拥有 test:update 权限",
            Data = new { PermissionCode = "test:update", ReceivedData = data, Timestamp = DateTime.Now }
        });
    }

    /// <summary>
    /// 测试删除权限
    /// </summary>
    [HttpDelete]
    [ActionName("TestDeleteAsync")]
    [Permission("test:delete")]
    public async Task<ApiRequestResult> TestDeleteAsync([FromQuery] string id)
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "删除权限验证通过！您拥有 test:delete 权限",
            Data = new { PermissionCode = "test:delete", DeletedId = id, Timestamp = DateTime.Now }
        });
    }

    /// <summary>
    /// 测试管理员权限
    /// </summary>
    [HttpGet]
    [ActionName("TestAdminOnlyAsync")]
    [Permission("test:super_admin")]
    public async Task<ApiRequestResult> TestAdminOnlyAsync()
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "管理员权限验证通过！您拥有 test:super_admin 权限",
            Data = new { PermissionCode = "test:super_admin", IsAdmin = true, Timestamp = DateTime.Now }
        });
    }

    /// <summary>
    /// 测试无权限要求（仅需要登录）
    /// </summary>
    [HttpGet]
    [ActionName("TestNoPermissionAsync")]
    public async Task<ApiRequestResult> TestNoPermissionAsync()
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "此接口无需特定权限，只需登录即可访问",
            Data = new { RequiresPermission = false, Timestamp = DateTime.Now }
        });
    }

    /// <summary>
    /// 测试复杂权限组合 - 需要 test:module1:read 和 (test:module2:write 或 test:admin)
    /// 实际使用多个 Permission 特性叠加
    /// </summary>
    [HttpPost]
    [ActionName("TestComplexPermissionAsync")]
    [Permission("test:module1:read")]
    [Permission(new[] { "test:module2:write", "test:admin" }, PermissionLogic.Any)]
    public async Task<ApiRequestResult> TestComplexPermissionAsync([FromBody] object data)
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "复杂权限组合验证通过！",
            Data = new
            {
                RequiredPermissions = new[] { "test:module1:read", "test:module2:write 或 test:admin" },
                Timestamp = DateTime.Now
            }
        });
    }

    /// <summary>
    /// 测试批量操作权限
    /// </summary>
    [HttpPost]
    [ActionName("TestBatchOperationAsync")]
    [Permission(new[] { "test:batch:create", "test:batch:update", "test:batch:delete" }, PermissionLogic.Any)]
    public async Task<ApiRequestResult> TestBatchOperationAsync([FromBody] List<object> items)
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "批量操作权限验证通过！您拥有批量操作权限之一",
            Data = new
            {
                PermissionCodes = new[] { "test:batch:create", "test:batch:update", "test:batch:delete" },
                Logic = "Any",
                ItemCount = items?.Count ?? 0,
                Timestamp = DateTime.Now
            }
        });
    }

    /// <summary>
    /// 测试报告导出权限
    /// </summary>
    [HttpGet]
    [ActionName("TestExportReportAsync")]
    [Permission("test:report:export")]
    public async Task<ApiRequestResult> TestExportReportAsync([FromQuery] string reportType)
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "报告导出权限验证通过！",
            Data = new { PermissionCode = "test:report:export", ReportType = reportType, Timestamp = DateTime.Now }
        });
    }

    /// <summary>
    /// 测试系统配置权限
    /// </summary>
    [HttpPost]
    [ActionName("TestSystemConfigAsync")]
    [Permission(new[] { "test:config:read", "test:config:write" }, PermissionLogic.All)]
    public async Task<ApiRequestResult> TestSystemConfigAsync([FromBody] Dictionary<string, string> config)
    {
        return await Task.FromResult(new ApiRequestResult
        {
            Success = true,
            Message = "系统配置权限验证通过！您同时拥有读写配置权限",
            Data = new
            {
                PermissionCodes = new[] { "test:config:read", "test:config:write" },
                Logic = "All",
                Config = config,
                Timestamp = DateTime.Now
            }
        });
    }
} 
