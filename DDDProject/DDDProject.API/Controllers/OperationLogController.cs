using DDDProject.Application.DTOs;
using DDDProject.Application.Interfaces;
using DDDProject.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DDDProject.API.Controllers;

/// <summary>
/// 操作日志控制器
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class OperationLogController : BaseApiController
{
    private readonly IOperationLogService _logService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logService">日志服务</param>
    public OperationLogController(IOperationLogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    /// 获取操作日志列表（分页）
    /// </summary>
    [HttpGet]
    [ActionName("GetOperationLogsAsync")]
    [ApiSearch(Name = "获取操作日志列表", Description = "返回操作日志列表（支持分页和筛选）", Category = ApiSearchCategory.Log)]
    public async Task<ApiRequestResult> GetOperationLogsAsync([FromQuery] OperationLogQueryRequest request)
    {
        return await _logService.GetOperationLogsAsync(request);
    }

    /// <summary>
    /// 获取操作日志详情
    /// </summary>
    [HttpGet]
    [ActionName("GetOperationLogByIdAsync")]
    [ApiSearch(Name = "获取操作日志详情", Description = "根据ID获取操作日志详细信息", Category = ApiSearchCategory.Log)]
    public async Task<ApiRequestResult> GetOperationLogByIdAsync([FromQuery] Guid id)
    {
        return await _logService.GetOperationLogByIdAsync(id);
    }

    /// <summary>
    /// 删除操作日志
    /// </summary>
    [HttpDelete]
    [ActionName("DeleteOperationLogAsync")]
    [Permission("log:delete")]
    [ApiSearch(Name = "删除操作日志", Description = "根据ID删除操作日志", Category = ApiSearchCategory.Log)]
    public async Task<IActionResult> DeleteOperationLogAsync([FromQuery] Guid id)
    {
        var result = await _logService.DeleteOperationLogAsync(id);
        if (!result.Success)
            return BadRequest(result);
        return NoContent();
    }

    /// <summary>
    /// 批量删除操作日志
    /// </summary>
    [HttpDelete]
    [ActionName("BatchDeleteOperationLogsAsync")]
    [Permission("log:delete")]
    [ApiSearch(Name = "批量删除操作日志", Description = "批量删除多条操作日志", Category = ApiSearchCategory.Log)]
    public async Task<IActionResult> BatchDeleteOperationLogsAsync([FromBody] List<Guid> ids)
    {
        var result = await _logService.BatchDeleteOperationLogsAsync(ids);
        if (!result.Success)
            return BadRequest(result);
        return NoContent();
    }

    /// <summary>
    /// 清空指定时间范围的操作日志
    /// </summary>
    [HttpDelete]
    [ActionName("ClearOperationLogsAsync")]
    [Permission("log:clear")]
    [ApiSearch(Name = "清空操作日志", Description = "清空指定时间范围的操作日志", Category = ApiSearchCategory.Log)]
    public async Task<ApiRequestResult> ClearOperationLogsAsync([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime)
    {
        return await _logService.ClearOperationLogsAsync(startTime, endTime);
    }

    /// <summary>
    /// 获取操作类型统计
    /// </summary>
    [HttpGet]
    [ActionName("GetOperationTypeStatisticsAsync")]
    [ApiSearch(Name = "获取操作类型统计", Description = "获取各操作类型的数量统计", Category = ApiSearchCategory.Log)]
    public async Task<ApiRequestResult> GetOperationTypeStatisticsAsync([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime)
    {
        return await _logService.GetOperationTypeStatisticsAsync(startTime, endTime);
    }

    /// <summary>
    /// 获取操作模块统计
    /// </summary>
    [HttpGet]
    [ActionName("GetModuleStatisticsAsync")]
    [ApiSearch(Name = "获取操作模块统计", Description = "获取各操作模块的数量统计", Category = ApiSearchCategory.Log)]
    public async Task<ApiRequestResult> GetModuleStatisticsAsync([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime)
    {
        return await _logService.GetModuleStatisticsAsync(startTime, endTime);
    }

    /// <summary>
    /// 导出操作日志
    /// </summary>
    [HttpGet]
    [ActionName("ExportOperationLogsAsync")]
    [Permission("log:export")]
    [ApiSearch(Name = "导出操作日志", Description = "导出操作日志数据", Category = ApiSearchCategory.Log)]
    public async Task<IActionResult> ExportOperationLogsAsync([FromQuery] OperationLogQueryRequest request)
    {
        var excelData = await _logService.ExportToExcelAsync(request);
        var fileName = $"操作日志_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}