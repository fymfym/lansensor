using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Services.Monitor.DataValueToOld
{
    public interface IDataValueToOldMonitor
    {
        bool IsDataValueToOld(DeviceLogEntity deviceEntity, Domain.Models.DataValueToOld dataValueToOld);
    }
}
