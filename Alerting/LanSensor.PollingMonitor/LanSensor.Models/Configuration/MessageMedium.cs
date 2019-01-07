using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class MessageMedium
    {
        [JsonProperty("mediumType")] public string MediumType { get; set; }
        [JsonProperty("receiverId")] public string ReceiverId { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
    }
}
