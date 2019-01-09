using System.Threading.Tasks;
using LanSensor.Models;

namespace LanSensor.Repository.DeviceState
{
    public interface IDeviceStateRepository
    {
        Task<DeviceStateEntity> GetLatestDeviceStateEntity(string devideGroupId, string deviceId);
        Task<DeviceStateEntity> SetDeviceStateEntity(DeviceStateEntity deviceStateEntity);
    }
}
