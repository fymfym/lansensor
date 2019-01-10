
using System.Threading.Tasks;
using LanSensor.Models;
using LanSensor.Models.DeviceState;

namespace LanSensor.Repository.DeviceState.LiteDb
{
    public class LiteDbDeviceStateRepository : IDeviceStateRepository
    {
        public Task<DeviceStateEntity> GetLatestDeviceStateEntity(string devideGroupId, string deviceId)
        {
            throw new System.NotImplementedException();
        }

        public Task<DeviceStateEntity> SetDeviceStateEntity(DeviceStateEntity deviceStateEntity)
        {
            throw new System.NotImplementedException();
        }
    }
}
