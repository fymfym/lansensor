using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TimeInterval
    {
        [JsonProperty("weekdays")] public IEnumerable<DayOfWeek> Weekdays { get; set; }
        [JsonProperty("times")] public IEnumerable<TimeFromTo> Times { get; set; }
        [JsonProperty("atTime")] public SingleTime SpecificTime { get; set; }
    }
}
