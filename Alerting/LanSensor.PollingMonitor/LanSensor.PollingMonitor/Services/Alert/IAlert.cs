using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Alert
{
    public interface IAlert
    {
        bool SendKeepaliveMissing(DeviceMonitor deviceMonitor);
        bool SendAlert(DeviceLog presenceRecord, TimeInterval timeInterval, DeviceMonitor deviceMonitor);
    }
}
