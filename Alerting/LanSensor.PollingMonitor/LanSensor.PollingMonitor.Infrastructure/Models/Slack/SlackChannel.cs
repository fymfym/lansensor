// ReSharper disable InconsistentNaming

namespace LanSensor.PollingMonitor.Infrastructure.Models.Slack
{
    public class SlackChannel
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool is_channel { get; set; }
        public bool is_group { get; set; }
        public bool is_im { get; set; }
        public int created { get; set; }
        public bool is_archived { get; set; }
        public bool is_general { get; set; }
        public int unlinked { get; set; }
        public string name_normalized { get; set; }
        public bool is_shared { get; set; }
        public object parent_conversation { get; set; }
        public string creator { get; set; }
        public bool is_ext_shared { get; set; }
        public bool is_org_shared { get; set; }
        public string[] shared_team_ids { get; set; }
        public object[] pending_shared { get; set; }
        public object[] pending_connected_team_ids { get; set; }
        public bool is_pending_ext_shared { get; set; }
        public bool is_member { get; set; }
        public bool is_private { get; set; }
        public bool is_mpim { get; set; }
        public object[] previous_names { get; set; }
        public int num_members { get; set; }
    }
}