using LanSensor.Models.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Monitor.DataValueToOld
{
    public interface IDataValueToOldMonitor
    {
        bool IsDataValueToOld(DeviceLogEntity deviceEntity, Models.Configuration.DataValueToOld dataValueToOld);
    }
}
