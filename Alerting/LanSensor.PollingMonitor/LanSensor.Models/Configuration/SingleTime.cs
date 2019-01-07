using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class SingleTime
    {
        [JsonProperty("houre")] public int Houre { get; set; }
        [JsonProperty("minute")] public int Minute { get; set; }
    }
}
