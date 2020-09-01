// ReSharper disable InconsistentNaming

namespace LanSensor.PollingMonitor.Infrastructure.Models.Slack
{
    public class SlackPostMessageResponse
    {
        public bool ok { get; set; }
        public string channel { get; set; }
        public string ts { get; set; }
        public Message message { get; set; }
        public string warning { get; set; }
    }

    public class Message
    {
        public string bot_id { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public string user { get; set; }
        public string ts { get; set; }
        public string team { get; set; }
        public Bot_Profile bot_profile { get; set; }
    }

    public class Bot_Profile
    {
        public string id { get; set; }
        public bool deleted { get; set; }
        public string name { get; set; }
        public int updated { get; set; }
        public string app_id { get; set; }
        public string team_id { get; set; }
    }
}