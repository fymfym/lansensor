using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public class DataValueToOld
    {
        [JsonProperty("dataValue")] public string DataValue;

        [JsonProperty("maxAgeInMinutes")] public long MaxAgeInMinutes;
    }
}
