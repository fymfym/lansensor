using System.Collections.Generic;

namespace LanSensor.PollingMonitor.Infrastructure.Models.Slack
{
    public class SlackChannelListResponse
    {
        public string Ok { get; set; }
        public List<SlackChannel> Channels { get; set; }
    }
}
