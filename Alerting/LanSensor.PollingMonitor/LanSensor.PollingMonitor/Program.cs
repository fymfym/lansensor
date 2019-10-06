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
using LanSensor.Repository.DeviceLog.MySqlDeviceLog;
using LanSensor.Repository.DeviceState;
using LanSensor.Repository.DeviceState.MySqlDeviceState;
using NLog.Web;

namespace LanSensor.PollingMonitor
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var globalRetryCount = 10;
            while (globalRetryCount > 0)
            {
                var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

                IConfiguration configuration = new Configuration();

                try
                {
                    IDeviceLogRepository deviceLogRepository = new MySqlDataStoreRepository(configuration);
                    IDeviceStateRepository deviceStateRepository = new MySqlDeviceStateRepository(configuration, logger);
                    IDateTimeService getDate = new DateTimeService();
                    IAlert alerter = new SendSlackAlert(configuration, logger);
                    ITimeIntervalMonitor stateCheckComparer = new TimeIntervalComparer();
                    IKeepAliveMonitor keepAliveMonitor = new KeepAliveMonitor(deviceLogRepository, getDate);
                    IStateChangeMonitor stateChange = new StateChangeMonitor();
                    IPauseService pauseService = new PauseService();

                    var monitor = new Services.Monitor.PollingMonitor(
                        configuration,
                        alerter,
                        stateCheckComparer,
                        keepAliveMonitor,
                        stateChange,
                        deviceStateRepository,
                        deviceLogRepository,
                        logger,
                        pauseService
                    );

                    monitor.RunInLoop();

                    globalRetryCount = 10;

                    monitor.RunInLoop();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    globalRetryCount--;
                    if (globalRetryCount < 0)
                    {
                        break;
                    }
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}