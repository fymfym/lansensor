using System;
using System.Linq;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;

namespace LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.StateChange
{
    public class StateChangeMonitor : IMonitorExecuter
    {
        private readonly IDeviceLogService _deviceLogService;
        private readonly IAlertService _alert;

        public StateChangeMonitor(
            IDeviceLogService deviceLogService,
            IAlertService alert
            )
        {
            _deviceLogService = deviceLogService;
            _alert = alert;
        }

        public bool CanMonitorRun(DeviceMonitor monitor)
        {
            return monitor?.StateChangeNotification?.OnDataValueChangeTo != null
                   || monitor?.StateChangeNotification?.OnDataValueChangeFrom != null;
        }

        public DeviceStateEntity Run(DeviceStateEntity state, DeviceMonitor monitor)
        {
            if (!CanMonitorRun(monitor)) return state;

            var presenceListTask = _deviceLogService.GetPresenceListSince(
                monitor.DeviceGroupId,
                monitor.DeviceId,
                DateTime.MinValue
                );

            presenceListTask.Wait();

            var deviceLogEntity = presenceListTask.Result.FirstOrDefault();

            if (deviceLogEntity == null)
                return state;

            if (IsValueReached(
                    state.LastKnownDataValue,
                    deviceLogEntity.DataValue,
                monitor.StateChangeNotification.OnDataValueChangeTo))
            {
                _alert.SendStateChangeAlert(new StateChangeResult
                {
                    DataValue = deviceLogEntity.DataValue,
                    ChangedToValue = true
                }, monitor);
                state.LastKnownDataValue = deviceLogEntity.DataValue;
                state.LastKnownDataValueDate = deviceLogEntity.DateTime;
            }
            else
            {
                if (!IsValueReached(
                    deviceLogEntity.DataValue,
                    deviceLogEntity.DataValue,
                    monitor.StateChangeNotification.OnDataValueChangeFrom)) return state;

                _alert.SendStateChangeAlert(new StateChangeResult
                {
                    DataValue = deviceLogEntity.DataValue,
                    ChangedFromValue = true
                }, monitor);

                state.LastKnownDataValue = deviceLogEntity.DataValue;
                state.LastKnownDataValueDate = deviceLogEntity.DateTime;
            }

            return state;
        }

        private static bool IsValueReached(string stateDataValue, string deviceDataValue, string wantedDataValue)
        {
            if (string.IsNullOrEmpty(stateDataValue)) return false;
            if (string.IsNullOrEmpty(deviceDataValue)) return false;
            if (string.IsNullOrEmpty(wantedDataValue)) return false;

            if (string.Equals(deviceDataValue, stateDataValue, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return !string.Equals(deviceDataValue, wantedDataValue, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
