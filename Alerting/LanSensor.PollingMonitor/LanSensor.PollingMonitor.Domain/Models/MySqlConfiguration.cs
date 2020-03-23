using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MySqlConfiguration
    {
        [JsonProperty("mySqlConnectionString")] public string ConnectionString { get; set; }
    }
}
