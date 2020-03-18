using System;
using LanSensor.PollingMonitor.Domain.Repositories;
using MongoDB.Bson;

namespace LanSensor.Repository.Models.MongoDb
{
    public class DeviceStateEntityMongoDb : IInfrastructureEntity
    {
        public string DeviceGroupId { get; set; }
        public string DeviceId { get; set; }
        public string LastKnownDataValue { get; set; }
        public DateTime LastKnownDataValueDate { get; set; }
        public DateTime LastKnownKeepAliveDate { get; set; }
        public DateTime LastKeepAliveAlert { get; set; }
        public DateTime LastExecutedKeepAliveCheckDate { get; set; }
        public ObjectId Id { get; set; }
        public string EntityId { get; set; }
    }
}
