using System;
using System.Collections.Generic;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Domain.Services
{
    public interface IMonitorTools
    {
        bool IsInsideTimeInterval(IEnumerable<TimeInterval> times, DateTime deviceLogDateValue);
        MonitorState GetMonitorState(DeviceStateEntity deviceStateEntity, DeviceMonitor monitor);
        void SetMonitorState(DeviceStateEntity deviceStateEntity, DeviceMonitor monitor, MonitorState monitorState);
    }
}
