using System;
using System.Collections.Generic;
using System.Text;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public class MonitorState
    {
        public string MonitorName { get; set; }
        public string Value { get; set; }
    }
}
