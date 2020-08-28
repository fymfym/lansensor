using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Infrastructure.Models.Slack
{
    public class SlackPostMessage
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
