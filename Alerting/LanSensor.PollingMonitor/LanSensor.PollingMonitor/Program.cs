using System;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Alert.Slack;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor.KeepAlive;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.PollingMonitor.Services.Pause;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceLog.MySQL;
using LanSensor.Repository.DeviceState;
using LanSensor.Repository.DeviceState.MySql;
using NLog.Web;

namespace LanSensor.PollingMonitor
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            IConfiguration configuration = new Configuration(null);

            logger.Info(configuration.ApplicationConfiguration.MySqlConfiguration);

            IDeviceLogRepository deviceLogRepository = new MySqlDataStoreRepository(configuration);
            IDeviceStateRepository deviceStateRepository = new MySqlDeviceStateRepository(configuration, logger);
            IDateTimeService date = new DateTimeServiceService();
            IAlert alerter = new SendSlackAlert(configuration, logger);
            ITimeIntervalMonitor stateCheckComparer = new TimeIntervalComparer();
            IKeepaliveMonitor keepAlive = new KeepAliveMonitor(deviceLogRepository, date);
            IStateChangeMonitor stateChange = new StateChangeMonitor();
            IPauseService pauseService = new PauseService();

            var retry = 10;
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
                        logger,
                        pauseService);

                    monitor.RunInLoop();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
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