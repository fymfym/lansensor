using AutoMapper;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.Repository.Models.MongoDb;

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
    }
}
