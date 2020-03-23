using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SlackConfiguration
    {
        [JsonProperty("slackApiKey")] public string ApiKey { get; set; }
    }
}
