using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Domain.Services
{
    public interface IAlertService
    {
        bool SendStateChangeAlert(StateChangeResult stateNotification, DeviceMonitor deviceMonitor);
        bool SendKeepAliveMissingAlert(DeviceMonitor deviceMonitor);
        bool SendTimerIntervalAlert(DeviceLogEntity presenceRecord, DeviceMonitor deviceMonitor);
        bool SendTextMessage(DeviceMonitor deviceMonitor, string message);
    }
}
