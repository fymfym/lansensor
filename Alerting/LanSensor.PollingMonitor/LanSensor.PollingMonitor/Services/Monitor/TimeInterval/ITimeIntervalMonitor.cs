using System.Collections.Generic;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Services.Monitor.TimeInterval
{
    public interface ITimeIntervalMonitor
    {
        Domain.Models.TimeInterval GetFailedTimerInterval(IEnumerable<Domain.Models.TimeInterval> timeIntervals, DeviceLogEntity presenceRecord);
    }
}
