using System.Collections.Generic;
using LanSensor.Models.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Monitor.TimeInterval
{
    public interface ITimeIntervalMonitor
    {
        Models.Configuration.TimeInterval GetFailedTimerInterval(IEnumerable<Models.Configuration.TimeInterval> timeIntervals, DeviceLogEntity presenceRecord);
    }
}
