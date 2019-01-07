using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class SlackConfiguration
    {
        [JsonProperty("apikey")] public string ApiKey{ get; set; }
    }
}
