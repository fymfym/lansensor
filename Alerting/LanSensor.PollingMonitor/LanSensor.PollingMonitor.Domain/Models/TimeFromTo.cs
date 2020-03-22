using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public class TimeFromTo
    {
        [JsonProperty("from")] public SingleTime From { get; set; }
        [JsonProperty("to")] public SingleTime To { get; set; }
    }
}
