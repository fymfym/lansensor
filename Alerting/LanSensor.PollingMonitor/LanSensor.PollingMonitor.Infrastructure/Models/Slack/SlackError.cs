namespace LanSensor.PollingMonitor.Infrastructure.Models.Slack
{
    public class SlackError

    {
        public bool ok { get; set; }
        public string error { get; set; }
        public string warning { get; set; }
        public Response_Metadata response_metadata { get; set; }
    }

    public class Response_Metadata
    {
        public string[] warnings { get; set; }
    }
}