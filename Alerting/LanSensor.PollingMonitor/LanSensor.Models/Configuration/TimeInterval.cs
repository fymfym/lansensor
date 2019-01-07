using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public enum EWeekDays
    {
        Mon,Tue,Wed,Thu,Fri,Sau,Sun
    }

    public class TimeInterval
    {
        [JsonProperty("weekdays")] public IEnumerable<EWeekDays> Weekdays { get; set; }
        [JsonProperty("times")] public IEnumerable<TimeFromTo> Times { get; set; }
        [JsonProperty("alertMessage")] public string AlertMessage { get; set; }
        [JsonProperty("dataValue")] public string DataValue { get; set; }
        [JsonProperty("messageMediums")] public IEnumerable<MessageMedium> MessageMediums { get; set; }
    }
}
