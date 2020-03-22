using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Infrastructure.Models.MongoDb;

namespace LanSensor.PollingMonitor.Infrastructure.Repositories
{
    public interface IDeviceStateRepository : IRepository<DeviceStateEntity, DeviceStateEntityMongoDb>
    {
    }
}
