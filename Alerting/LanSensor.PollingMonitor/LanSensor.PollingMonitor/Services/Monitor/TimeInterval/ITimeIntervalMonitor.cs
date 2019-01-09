using System.Collections.Generic;
using System.Threading.Tasks;
using LanSensor.Models.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Monitor.TimeInterval
{
    public interface ITimeIntervalMonitor
    {
        Models.Configuration.TimeInterval GetFailedTimerInterval(IEnumerable<Models.Configuration.TimeInterval> timeIntervals, DeviceLogEntity presenceRedocrd);
    }
}
