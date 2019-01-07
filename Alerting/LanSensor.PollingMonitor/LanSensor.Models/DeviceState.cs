
namespace LanSensor.Models
{
    public class DeviceState
    {
        public string DeviceGroipId { get; set; }
        public string DeviceId { get; set; }
        public string LastKnownDataValue { get; set; }
        public string LastKnownKeepAlive { get; set; }
        public string LastExecutedKeepaliveCheck { get; set; }
    }
}
