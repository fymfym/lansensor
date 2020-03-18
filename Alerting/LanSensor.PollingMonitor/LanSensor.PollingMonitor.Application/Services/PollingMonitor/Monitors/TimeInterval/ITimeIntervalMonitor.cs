using System;
using System.Collections.Generic;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.TimeInterval
{
    [Obsolete]
    public interface ITimeIntervalMonitor
    {
        Domain.Models.TimeInterval GetFailedTimerInterval(IEnumerable<Domain.Models.TimeInterval> timeIntervals, DeviceLogEntity presenceRecord);
    }
}
