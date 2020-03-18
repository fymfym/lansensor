using System;
using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Services.Monitor.StateChange
{
    public class StateChangeMonitor : IStateChangeMonitor
    {
        public StateChangeResult GetStateChangeNotification(
            DeviceStateEntity deviceState,
            DeviceLogEntity deviceLogEntity,
            StateChangeNotification monitorChangeNotification)
        {
            StateChangeResult result = null;

            if (!string.Equals(deviceState.LastKnownDataValue?.ToLower(),
                deviceLogEntity.DataValue?.ToLower(), StringComparison.Ordinal))
            {
                result = new StateChangeResult
                {
                    DataValue = deviceLogEntity.DataValue,
                    ChangedToValue = true
                };
            }

            return result;
        }

        public StateChangeResult GetStateChangeFromToNotification(
            DeviceStateEntity deviceState,
            DeviceLogEntity deviceLogEntity,
            StateChangeNotification monitorChangeNotification)
        {
            if (monitorChangeNotification == null) return null;

            var presence = deviceLogEntity;

            StateChangeResult result = null;

            if (presence == null)
                return null;

            if (IsValueReached(
                deviceState.LastKnownDataValue,
                deviceLogEntity.DataValue,
                monitorChangeNotification.OnDataValueChangeTo))
            {
                result = new StateChangeResult
                {
                    DataValue = monitorChangeNotification.OnDataValueChangeTo,
                    ChangedToValue = true
                };
            }
            else
            {
                if (IsValueReached(
                    deviceLogEntity.DataValue,
                    deviceState.LastKnownDataValue,
                    monitorChangeNotification.OnDataValueChangeFrom))
                {
                    result = new StateChangeResult
                    {
                        DataValue = monitorChangeNotification.OnDataValueChangeFrom,
                        ChangedFromValue = true
                    };
                }
            }

            return result;
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
