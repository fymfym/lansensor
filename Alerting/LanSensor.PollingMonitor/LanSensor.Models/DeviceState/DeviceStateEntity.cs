using System;

namespace LanSensor.Models.DeviceState
{
    public class DeviceStateEntity
    {
        public string DeviceGroupId { get; set; }
        public string DeviceId { get; set; }
        public string LastKnownDataValue { get; set; }
        public DateTime LastKnownDataValueDate { get; set; }
        public DateTime LastKnownKeepAlive { get; set; }
        public DateTime LastKeepAliveAlert { get; set; }
        public DateTime LastExecutedKeepAliveCheckDate { get; set; }
    }
}
