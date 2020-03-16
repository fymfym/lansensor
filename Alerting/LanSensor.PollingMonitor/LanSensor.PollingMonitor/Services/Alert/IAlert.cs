using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Services.Alert
{
    public interface IAlert
    {
        bool SendStateChangeAlert(StateChangeResult stateNotification, DeviceMonitor deviceMonitor);
        bool SendKeepAliveMissingAlert(DeviceMonitor deviceMonitor);
        bool SendTimerIntervalAlert(DeviceLogEntity presenceRecord, TimeInterval timeInterval, DeviceMonitor deviceMonitor);
        bool SendTextMessage(DeviceMonitor deviceMonitor, string message);
    }
}
