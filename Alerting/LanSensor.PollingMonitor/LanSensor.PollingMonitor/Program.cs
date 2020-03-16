using System;
using AutoMapper;
using LanSensor.PollingMonitor.Application.Repositories;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Alert.Slack;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor.KeepAlive;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.PollingMonitor.Services.Pause;
using LanSensor.Repository;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceLog.RestService;
using LanSensor.Repository.DeviceState.MongoDb;
using LanSensor.Repository.MappingProfiles;
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

                IServiceConfiguration configuration = new ServiceConfiguration();

                try
                {
                    // IDeviceLogRepository deviceLogRepository = new MySqlDataStoreRepository(configuration);

                    var httpFactory = new HttpClientFactory(configuration);
                    IDeviceLogRepository deviceLogRepository = new RestDeviceLogRepository(httpFactory);
                    //IDeviceStateRepository deviceStateRepository = new MySqlDeviceStateRepository(configuration, logger);
                    var mapperConfig = new MapperConfiguration(cfg => {
                        cfg.AddProfile<InfrastructureAutoMapProfile>();
                    });
                    var mapper = mapperConfig.CreateMapper();
                    IDeviceState deviceStateRepository = new MongoDeviceStateService(configuration, mapper);
                    IDateTimeService getDate = new DateTimeService();
                    IAlert alerter = new SendSlackAlert(configuration, logger);
                    ITimeIntervalMonitor stateCheckComparer = new TimeIntervalComparer();
                    IKeepAliveMonitor keepAliveMonitor = new KeepAliveMonitor(deviceLogRepository, getDate);
                    IStateChangeMonitor stateChange = new StateChangeMonitor();
                    IPauseService pauseService = new PauseService();

                    System.Threading.Thread.Sleep(5000);

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