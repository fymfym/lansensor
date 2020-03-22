using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Domain.Services
{
    public interface IMonitorExecuter
    {
        bool CanMonitorRun(DeviceMonitor monitor);
        DeviceStateEntity Run(DeviceStateEntity state, DeviceMonitor monitor);
    }
}
