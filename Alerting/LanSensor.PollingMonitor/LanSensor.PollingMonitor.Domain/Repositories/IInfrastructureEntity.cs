using MongoDB.Bson;

namespace LanSensor.PollingMonitor.Domain.Repositories
{
    public interface IInfrastructureEntity
    {
        ObjectId Id { get; set; }
        string EntityId { get; set; }
    }
}
