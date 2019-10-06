using LanSensor.Models.DeviceLog;
using LanSensor.Models.DeviceState;

namespace LanSensor.PollingMonitor.Services.Monitor.StateChange
{
    public interface IStateChangeMonitor
    {
        StateChangeResult GetStateChangeNotification(
            DeviceStateEntity deviceState,
            DeviceLogEntity deviceLogEntity,
            Models.Configuration.StateChangeNotification stateChangeNotification);

        StateChangeResult GetStateChangeFromToNotification(
            DeviceStateEntity deviceState,
            DeviceLogEntity deviceLogEntity,
            Models.Configuration.StateChangeNotification stateChangeNotification);
    }
}
