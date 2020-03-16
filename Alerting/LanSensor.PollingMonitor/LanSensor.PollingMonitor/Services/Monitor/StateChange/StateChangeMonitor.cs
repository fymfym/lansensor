using System;
using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Services.Monitor.StateChange
{
    public class StateChangeMonitor : IStateChangeMonitor
    {
        public StateChangeResult GetStateChangeNotification(DeviceStateEntity deviceState, DeviceLogEntity deviceLogEntity,
            StateChangeNotification stateChangeNotification)
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
            StateChangeNotification stateChangeNotification)
        {
            if (stateChangeNotification == null) return null;

            var presence = deviceLogEntity;

            StateChangeResult result = null;

            if (presence == null)
                return null;

            if (IsFromValueReached(stateChangeNotification.OnDataValueChangeTo, deviceState.LastKnownDataValue, deviceLogEntity.DataValue))
            {
                result = new StateChangeResult
                {
                    DataValue = stateChangeNotification.OnDataValueChangeTo,
                    ChangedToValue = true
                };
            }
            else
            {
                if (IsToValueReached(stateChangeNotification.OnDataValueChangeFrom, deviceState.LastKnownDataValue, deviceLogEntity.DataValue))
                {
                    result = new StateChangeResult
                    {
                        DataValue = stateChangeNotification.OnDataValueChangeFrom,
                        ChangedFromValue = true
                    };
                }
            }

            return result;
        }

        private static bool IsFromValueReached(string monitorChangeFrom, string deviceDataValue, string stateDataValue)
        {
            if (string.IsNullOrEmpty(stateDataValue)) return false;
            if (string.IsNullOrEmpty(deviceDataValue)) return false;
            if (string.IsNullOrEmpty(monitorChangeFrom)) return false;

            if (string.Equals(deviceDataValue, stateDataValue, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return !string.Equals(deviceDataValue, monitorChangeFrom, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsToValueReached(string monitorChangeTo, string deviceDataValue, string stateDataValue)
        {
            if (string.IsNullOrEmpty(stateDataValue)) return false;
            if (string.IsNullOrEmpty(deviceDataValue)) return false;
            if (string.IsNullOrEmpty(monitorChangeTo)) return false;

            if (string.Equals(deviceDataValue, stateDataValue, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return !string.Equals(deviceDataValue, monitorChangeTo, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
