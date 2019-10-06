using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SlackConfiguration
    {
        [JsonProperty("apikey")] public string ApiKey { get; set; }
    }
}
