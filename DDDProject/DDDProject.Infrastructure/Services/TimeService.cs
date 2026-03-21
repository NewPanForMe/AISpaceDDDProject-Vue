using System.Globalization;

namespace DDDProject.Infrastructure.Services
{
    /// <summary>
    /// 时间服务，统一处理中国标准时间（UTC+8）
    /// </summary>
    public interface ITimeService
    {
        DateTime GetCurrentTime();
        DateTimeOffset GetCurrentOffsetTime();
    }
    
    /// <summary>
    /// 中国标准时间服务实现
    /// </summary>
    public class ChinaStandardTimeService : ITimeService
    {
        private static readonly TimeZoneInfo ChinaTimeZone = 
            TimeZoneInfo.FindSystemTimeZoneById("China Standard Time") ?? 
            TimeZoneInfo.Local; // Fallback to local if China Standard Time is not available
        
        public DateTime GetCurrentTime()
        {
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, ChinaTimeZone);
        }
        
        public DateTimeOffset GetCurrentOffsetTime()
        {
            return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, ChinaTimeZone);
        }
    }
}