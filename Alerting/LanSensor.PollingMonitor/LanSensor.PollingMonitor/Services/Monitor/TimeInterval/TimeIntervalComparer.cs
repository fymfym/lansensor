using System.Collections.Generic;
using LanSensor.Models.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Monitor.TimeInterval
{
    public class TimeIntervalComparer : ITimeIntervalComparer
    {
        public Models.Configuration.TimeInterval GetFailedTimerInterval(IEnumerable<Models.Configuration.TimeInterval> timeIntervals, DeviceLog presenceRedocrd)
        {
            // TODO
            return new Models.Configuration.TimeInterval();
        }
    }
}
