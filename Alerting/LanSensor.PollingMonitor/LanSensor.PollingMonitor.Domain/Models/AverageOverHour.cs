using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AverageOverHour
    {
        [JsonProperty("hours")] public int Hours { get; set; }
        [JsonProperty("dataValue")] public string DataValue { get; set; }
        [JsonProperty("alartBelow")] public double? AlertBelow { get; set; }
        [JsonProperty("alartAbove")] public double? AlertAbove { get; set; }
    }
}
