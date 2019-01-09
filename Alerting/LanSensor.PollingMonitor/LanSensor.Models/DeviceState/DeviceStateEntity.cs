

using System;

namespace LanSensor.Models
{
    public class DeviceStateEntity
    {
        public string DeviceGroipId { get; set; }
        public string DeviceId { get; set; }
        public string LastKnownDataValue { get; set; }
        public DateTime LastKnownDataValueDate { get; set; }
        public string LastKnownKeepAlive { get; set; }
        public string LastExecutedKeepaliveCheck { get; set; }
    }
}
