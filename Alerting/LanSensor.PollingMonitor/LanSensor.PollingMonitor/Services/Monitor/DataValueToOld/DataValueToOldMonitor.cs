using System;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Services.Monitor.DataValueToOld
{
    public class DataValueToOldMonitor : IDataValueToOldMonitor
    {
        public bool IsDataValueToOld(DeviceLogEntity deviceEntity, Domain.Models.DataValueToOld dataValueToOld)
        {
            var date = System.DateTime.Now;
            var span = new TimeSpan(date.Ticks - deviceEntity.DateTime.Ticks);
            return span.TotalMinutes < dataValueToOld.MaxAgeInMinutes;
        }
    }
}
