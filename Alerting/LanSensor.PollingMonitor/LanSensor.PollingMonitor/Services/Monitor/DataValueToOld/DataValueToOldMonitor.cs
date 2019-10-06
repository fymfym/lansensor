using System;
using LanSensor.Models.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Monitor.DataValueToOld
{
    public class DataValueToOldMonitor : IDataValueToOldMonitor
    {
        public bool IsDataValueToOld(DeviceLogEntity deviceEntity, Models.Configuration.DataValueToOld dataValueToOld)
        {
            var date = System.DateTime.Now;
            var span = new TimeSpan(date.Ticks - deviceEntity.DateTime.Ticks);
            return span.TotalMinutes < dataValueToOld.MaxAgeInMinutes;
        }
    }
}
