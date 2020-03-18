using System;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;

namespace LanSensor.PollingMonitor.Services.PollingMonitor.Monitors.DataValueToOld
{
    public class DataValueToOldMonitor : IMonitorExecuter
    {
        public bool IsDataValueToOld(DeviceLogEntity deviceEntity, Domain.Models.DataValueToOld dataValueToOld)
        {
            var date = System.DateTime.Now;
            var span = new TimeSpan(date.Ticks - deviceEntity.DateTime.Ticks);
            return span.TotalMinutes < dataValueToOld.MaxAgeInMinutes;
        }

        public bool CanMonitorRun(DeviceMonitor monitor)
        {
            return monitor.DataValueToOld?.MaxAgeInMinutes > 0 && monitor.DataType != null;
        }

        public DeviceStateEntity Run(DeviceStateEntity state, DeviceMonitor monitor)
        {
            if (!CanMonitorRun(monitor)) return state;

            throw new NotImplementedException();
        }
    }
}
