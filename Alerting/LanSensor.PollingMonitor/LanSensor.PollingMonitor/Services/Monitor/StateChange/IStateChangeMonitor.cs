using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Services.Monitor.StateChange
{
    public interface IStateChangeMonitor
    {
        StateChangeResult GetStateChangeNotification(
            DeviceStateEntity deviceState,
            DeviceLogEntity deviceLogEntity,
            StateChangeNotification monitorChangeNotification);

        StateChangeResult GetStateChangeFromToNotification(
            DeviceStateEntity deviceState,
            DeviceLogEntity deviceLogEntity,
            StateChangeNotification monitorChangeNotification);
    }
}
