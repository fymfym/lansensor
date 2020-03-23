using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MongoDbConfiguration
    {
        [JsonProperty("mongoDbConnectionString")] public string ConnectionString { get; set; }
    }
}
