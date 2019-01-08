using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class SingleTime
    {
        [JsonProperty("hour")] public int Hour { get; set; }
        [JsonProperty("minute")] public int Minute { get; set; }

        public long GetNumber()
        {
            return (Hour * 60) + Minute;
        }

    }
}
