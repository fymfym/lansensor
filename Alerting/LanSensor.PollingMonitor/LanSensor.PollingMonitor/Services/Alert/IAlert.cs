﻿using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;
using LanSensor.Models.DeviceState;

namespace LanSensor.PollingMonitor.Services.Alert
{
    public interface IAlert
    {
        bool SendStateChangeAlert(StateChangeResult stateNotification, DeviceMonitor deviceMonitor);
        bool SendKeepaliveMissingAlert(DeviceMonitor deviceMonitor);
        bool SendTimerIntervalAlert(DeviceLogEntity presenceRecord, TimeInterval timeInterval, DeviceMonitor deviceMonitor);
        bool SendDataValueToOld(DeviceLogEntity presenceRecord, DeviceMonitor deviceMonitor);
    }
}
