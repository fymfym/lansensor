using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class StateChangeNotification
    {
        [JsonProperty("onDataValueChangeFrom")] public string OnDataValueChangeFrom { get; set; }
        [JsonProperty("onDataValueChangeTo")] public string OnDataValueChangeTo { get; set; }
        [JsonProperty("onEveryChange")] public bool OnEveryChange { get; set; }
    }
}
