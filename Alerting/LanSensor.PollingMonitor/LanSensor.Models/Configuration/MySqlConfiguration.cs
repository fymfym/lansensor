using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class MySqlConfiguration
    {
        [JsonProperty("connectionString")] public string ConnectionString{ get; set; }
    }
}
