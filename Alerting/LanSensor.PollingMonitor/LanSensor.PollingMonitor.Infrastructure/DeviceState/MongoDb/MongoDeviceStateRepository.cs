using System.Threading.Tasks;
using AutoMapper;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Infrastructure.Models.MongoDb;
using LanSensor.PollingMonitor.Infrastructure.Repositories;

namespace LanSensor.PollingMonitor.Infrastructure.DeviceState.MongoDb
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
