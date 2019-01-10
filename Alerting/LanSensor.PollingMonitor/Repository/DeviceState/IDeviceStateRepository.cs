using System.Threading.Tasks;
using LanSensor.Models;
using LanSensor.Models.DeviceState;

namespace LanSensor.Repository.DeviceState
{
    public interface IDeviceStateRepository
    {
        Task<DeviceStateEntity> GetLatestDeviceStateEntity(string devideGroupId, string deviceId);
        Task<DeviceStateEntity> SetDeviceStateEntity(DeviceStateEntity deviceStateEntity);
    }
}
