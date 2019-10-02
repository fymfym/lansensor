using System;
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
using LanSensor.Repository.DeviceState.MySql;
using NLog.Web;

namespace LanSensor.PollingMonitor
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            IConfiguration configuration = new Configuration(null);

            logger.Info(configuration.ApplicationConfiguration.MySqlConfiguration);
            Console.WriteLine(configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString);

            IDeviceLogRepository deviceLogRepository = new MySqlDataStoreRepository(configuration);
            IDeviceStateRepository deviceStateRepository = new MySqlDeviceStateRepository(configuration);
            IGetDateTime getDate = new GetDateTime();
            IAlert alerter = new SendSlackAlert(configuration, logger);
            ITimeIntervalMonitor stateCheckComparer = new TimeIntervalComparer();
            IKeepaliveMonitor keepAlive = new KeepaliveMonitor(deviceLogRepository, getDate);
            IStateChangeMonitor stateChange = new StateChangeMonitor();

            int retry = 10;
            while (true)
            {
                try
                {
                    var monitor = new Services.Monitor.PollingMonitor(
                        configuration,
                        deviceLogRepository,
                        alerter,
                        stateCheckComparer,
                        keepAlive,
                        stateChange,
                        deviceStateRepository,
                        deviceLogRepository,
                        logger);

                    monitor.Run();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString);
                    Console.WriteLine(ex.ToString());
                    retry--;
                    if (retry < 0)
                    {
                        break;
                    }
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}