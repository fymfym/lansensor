using System;
using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Alert.Slack;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor.Keepalive;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceLog.MySQL;

namespace LanSensor.PollingMonitor
{
    static class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                IConfiguration configuration = new Configuration(null);

                try
                {
                    IDeviceLogRepository repository = new MySqlDataStoreRepository(configuration);
                    IGetDateTime getDate = new GetDateTime();
                    IAlert alerter = new SendSlackAlert(configuration);
                    ITimeIntervalComparer stateCheckComparer = new TimeIntervalComparer();
                    IKeepaliveMonitor keepalive = new KeepaliveMonitor(repository,getDate);
                    var monitor = new Services.Monitor.PollingMonitor(configuration, repository, alerter, stateCheckComparer, keepalive);
                    Task.Run(() => monitor.Run());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
