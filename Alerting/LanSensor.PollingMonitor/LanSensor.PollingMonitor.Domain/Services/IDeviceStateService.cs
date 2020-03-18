using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Domain.Services
{
    public interface IDeviceStateService
    {
        Task<DeviceStateEntity> GetDeviceState(string deviceGroupId, string deviceId);
        Task<DeviceStateEntity> SaveDeviceState(DeviceStateEntity deviceStateEntity);
    }
}
