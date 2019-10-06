using System;
using System.Linq;
using SlackAPI;
using System.Configuration;

namespace LawnMowerNotifier
{
    public class Worker
    {

        Channel _notificationChannel;

        public void Run()
        {
            string slackApiKey = ConfigurationManager.AppSettings.Get("slackApiKey");

            var slack = new SlackClient(slackApiKey);
            slack.GetChannelList(SlackChannelListResponse);

            int count = 1000;
            while (true)
            {
                if (_notificationChannel != null) break;
                System.Threading.Thread.Sleep(200);
                if (count-- < 0)
                {
                    throw new Exception("No connection to slack");
                }
            }

            string keepaliveDataType = ConfigurationManager.AppSettings.Get("keepaliveDataType");
            var keepAlive = GetPrecense(keepaliveDataType,null);

            var keepAliveTimeSpan = new TimeSpan(DateTime.Now.Ticks - keepAlive.Ticks);
            if (keepAliveTimeSpan.TotalHours > 2)
            {
                Console.WriteLine("Keep alive is missing");
                slack.PostMessage(null, _notificationChannel.id, "Keepalive missing - Raspberry PI monitor failed for.");
            }

            string dataType = ConfigurationManager.AppSettings.Get("monitorValueDataType");
            string monitorValueDataValue = ConfigurationManager.AppSettings.Get("monitorValueDataValue");
            string monitorValueOkDataValue = ConfigurationManager.AppSettings.Get("monitorValueOkDataValue");

            var outDate = GetPrecense(dataType, monitorValueDataValue);
            var homeDate = GetPrecense(dataType, monitorValueOkDataValue);
            if (outDate > homeDate)
            {
                var outTimeSpan = new TimeSpan(DateTime.Now.Ticks - outDate.Ticks);
                if (outTimeSpan.TotalHours > 2)
                {
                    string monitorNotifyText = ConfigurationManager.AppSettings.Get("monitorNotifyText");
                    slack.PostMessage(null, _notificationChannel.id, monitorNotifyText);
                }
            }
        }

        private void SlackChannelListResponse(ChannelListResponse channelListResponse)
        {

            var channelList = channelListResponse.channels.ToList();
            string slackChannelNaame = ConfigurationManager.AppSettings.Get("slackChannelNaame");
            _notificationChannel = channelList.Find(x => x.name.Equals(slackChannelNaame));
        }

        private static DateTime GetPrecense(string dataType, string dataValue)
        {
            DateTime res = DateTime.MinValue;
            string sql;
            if (string.IsNullOrEmpty(dataValue))
            {
                sql = "select DateTime from sensorstate where DataType = '" + dataType + "'  order by DateTime desc limit 1";
            }
            else
            {
                sql = "select DateTime from sensorstate where DataType = '" + dataType + "' and DataValue = '" + dataValue + "' order by DateTime desc limit 1";
            }

            string mySqlConnectionString = ConfigurationManager.AppSettings.Get("mySqlConnectionString");
            using (var mysql = new MySql.Data.MySqlClient.MySqlConnection())
            {
                var strConnect = mySqlConnectionString;
                mysql.ConnectionString = strConnect;
                mysql.Open();

                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, mysql))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            res = rdr.GetDateTime(0);
                        }
                    }
                }
            }
            return res;

        }

    }
}
