using System.Collections.Generic;
using LanSensor.Models.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Monitor.TimeInterval
{
    public interface ITimeIntervalComparer
    {
        Models.Configuration.TimeInterval GetFailedTimerInterval(IEnumerable<Models.Configuration.TimeInterval> timeIntervals, DeviceLog presenceRedocrd);
    }
}
