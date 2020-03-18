using System;
using System.Collections.Generic;
using System.Text;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Domain.Services
{
    public interface IMonitorTools
    {
        bool IsInsideTimeInterval(IEnumerable<TimeInterval> times, DateTime deviceLogDateValue);
    }
}
