using System;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public class DeviceLogEntity
    {
        public string DeviceGroupId;
        public string DeviceId;
        public string DataType;
        public string DataValue;
        public DateTime DateTime;
    }
}
