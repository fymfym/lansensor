using System.Threading.Tasks;
using AutoMapper;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.Repository.Models.MongoDb;
using LanSensor.Repository.Repositories;

namespace LanSensor.Repository.DeviceState.MongoDb
{
    public class MongoDeviceStateRepository : MongoDbRepository<DeviceStateEntity, DeviceStateEntityMongoDb>, IDeviceStateRepository
    {
        public MongoDeviceStateRepository(
            string databaseName,
            string collection,
            IServiceConfiguration configuration,
            IMapper mapper
            ) : base(databaseName, collection, configuration, mapper)
        {
        }

        public Task<DeviceStateEntity> GetDeviceState(string deviceGroupId, string deviceId)
        {
            throw new System.NotImplementedException();
        }

        public Task<DeviceStateEntity> SaveDeviceState(DeviceStateEntity deviceStateEntity)
        {
            throw new System.NotImplementedException();
        }
    }
}
