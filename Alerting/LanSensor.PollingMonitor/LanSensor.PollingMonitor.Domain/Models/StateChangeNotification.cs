using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class StateChangeNotification
    {
        [JsonProperty("onDataValueChangeFrom")] public string OnDataValueChangeFrom { get; set; }
        [JsonProperty("onDataValueChangeTo")] public string OnDataValueChangeTo { get; set; }
        [JsonProperty("onEveryChange")] public bool OnEveryChange { get; set; }
    }
}
