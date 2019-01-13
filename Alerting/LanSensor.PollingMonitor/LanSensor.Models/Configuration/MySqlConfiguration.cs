using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MySqlConfiguration
    {
        [JsonProperty("connectionString")] public string ConnectionString{ get; set; }
    }
}
