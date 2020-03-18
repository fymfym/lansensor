using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Domain.Services
{
    public interface IAlertService
    {
        bool SendStateChangeAlert(StateChangeResult stateNotification, DeviceMonitor deviceMonitor);
        bool SendKeepAliveMissingAlert(DeviceMonitor deviceMonitor);
        bool SendTimerIntervalAlert(DeviceLogEntity presenceRecord, TimeInterval timeInterval, DeviceMonitor deviceMonitor);
        bool SendTextMessage(DeviceMonitor deviceMonitor, string message);
    }
}
