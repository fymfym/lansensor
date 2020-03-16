using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.Repository
{
    public interface IDeviceState
    {
        Task<DeviceStateEntity> GetLatestDeviceStateEntity(string deviceGroupId, string deviceId);
        Task<DeviceStateEntity> SetDeviceStateEntity(DeviceStateEntity deviceStateEntity);
    }
}
