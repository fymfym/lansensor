using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TimeInterval
    {
        [JsonProperty("weekdays")] public IEnumerable<DayOfWeek> Weekdays { get; set; }
        [JsonProperty("times")] public IEnumerable<TimeFromTo> Times { get; set; }
        [JsonProperty("dataValue")] public string DataValue { get; set; }
    }
}
