using System;
using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Alert.Slack;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor.Keepalive;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceLog.MySQL;
using LanSensor.Repository.DeviceState;
using LanSensor.Repository.DeviceState.LiteDb;
using LanSensor.Repository.DeviceState.MySql;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

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
                    IDeviceLogRepository deviceLogRepository = new MySqlDataStoreRepository(configuration);
                    IDeviceStateRepository deviceStateRepository = new MySqlDeviceStateRepository(configuration);
                    IGetDateTime getDate = new GetDateTime();
                    IAlert alerter = new SendSlackAlert(configuration);
                    ITimeIntervalMonitor stateCheckComparer = new TimeIntervalComparer();
                    IKeepaliveMonitor keepalive = new KeepaliveMonitor(deviceLogRepository, getDate);
                    IStateChangeMonitor stateChange = new StateChangeMonitor();

                    ILoggerFactory loggerFactory = new LoggerFactory().AddNLog();


                    var monitor = new Services.Monitor.PollingMonitor(
                        configuration, 
                        deviceLogRepository, 
                        alerter, 
                        stateCheckComparer, 
                        keepalive, 
                        stateChange,
                        deviceStateRepository, 
                        deviceLogRepository);
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