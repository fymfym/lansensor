using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LiteDbConfiguration
    {
        [JsonProperty("filename")] public string Filename { get; set; }
    }
}
