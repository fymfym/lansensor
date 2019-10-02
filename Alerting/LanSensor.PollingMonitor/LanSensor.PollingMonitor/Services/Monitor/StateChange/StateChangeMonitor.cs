using System;
using LanSensor.Models.DeviceState;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;

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

        public StateChangeResult GetStateChangeFromToNotification(DeviceStateEntity deviceState, DeviceLogEntity deviceLogEntity,
            StateChangeNotification stateChangeNotification)
        {
            var latestState = deviceState;

            var presence = deviceLogEntity;

            StateChangeResult result = null;

            if (presence == null)
                return null;

            if (!string.IsNullOrEmpty(stateChangeNotification.OnDataValueChangeFrom))
            {
                if (string.Equals(stateChangeNotification.OnDataValueChangeFrom, latestState.LastKnownDataValue))
                {
                    result = new StateChangeResult()
                    {
                        DataValue = stateChangeNotification.OnDataValueChangeFrom,
                        ChangedFromValue = true
                    };
                }
            }

            if (!string.IsNullOrEmpty(stateChangeNotification.OnDataValueChangeTo))
            {
                if (string.Equals(stateChangeNotification.OnDataValueChangeTo, presence.DataValue))
                {
                    result = new StateChangeResult()
                    {
                        DataValue = presence.DataValue,
                        ChangedToValue = true
                    };
                }
            }

            return result;
        }
    }
}
