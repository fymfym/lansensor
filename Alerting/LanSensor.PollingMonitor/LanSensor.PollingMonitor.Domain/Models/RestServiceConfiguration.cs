using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public class RestServiceConfiguration
    {
        [JsonProperty("deviceRestApiBasePath")] public string DeviceRestApiBasePath { get; set; }
    }
}
