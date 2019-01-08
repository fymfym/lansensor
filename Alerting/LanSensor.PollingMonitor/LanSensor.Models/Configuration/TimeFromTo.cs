using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class TimeFromTo
    {
        [JsonProperty("from")] public SingleTime From { get; set; }
        [JsonProperty("to")] public SingleTime To { get; set; }

    }
}
