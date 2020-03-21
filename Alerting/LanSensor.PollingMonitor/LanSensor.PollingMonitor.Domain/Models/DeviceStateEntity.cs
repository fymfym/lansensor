using System;
using System.Collections.Generic;
using LanSensor.PollingMonitor.Domain.Repositories;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public class DeviceStateEntity : IEntity
    {
        public string DeviceGroupId { get; set; }
        public string DeviceId { get; set; }
        public string LastKnownDataValue { get; set; }
        public DateTime LastKnownDataValueDate { get; set; }
        public DateTime LastKnownKeepAliveDate { get; set; }
        public DateTime LastKeepAliveAlert { get; set; }
        public DateTime LastExecutedKeepAliveCheckDate { get; set; }
        public string EntityId { get; set; }
    }
}
