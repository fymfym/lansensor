using System;
using AutoMapper;
using LanSensor.PollingMonitor.Application.Repositories;
using LanSensor.PollingMonitor.Application.Services;
using LanSensor.PollingMonitor.Application.Services.Alert.Slack;
using LanSensor.PollingMonitor.Application.Services.Pause;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.CalculateAverage;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.DataValueToOld;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.KeepAlive;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.MonitorAliveMessage;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.StateChange;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Tools;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.PollingMonitor.Infrastructure.DeviceLog.RestService;
using LanSensor.PollingMonitor.Infrastructure.DeviceState.MongoDb;
using LanSensor.PollingMonitor.Infrastructure.MappingProfiles;
using LanSensor.PollingMonitor.Infrastructure.Repositories;
using LanSensor.PollingMonitor.Infrastructure.RestServices.Slack;
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

                    logger.Info("Instantiating http client factory");
                    var httpFactory = new HttpClientFactory(configuration);

                    logger.Info("Instantiating Http service");
                    var httpService = new HttpCallExecuteService(configuration.ApplicationConfiguration.SlackConfiguration.ApiUrl);

                    logger.Info("Instantiating IMessage service");
                    var messageService = new SlackMessageSender(configuration, httpService);

                    messageService.SendMessage("LanSensor.PollingMonitor is starting");

                    logger.Info("Instantiating IDeviceLogRepository");
                    IDeviceLogRepository deviceLogRepository = new RestDeviceLogRepository(httpFactory);

                    logger.Info("Instantiating IDeviceLogService");
                    IDeviceLogService deviceLogService = new DeviceLogService(deviceLogRepository);

                    logger.Info("Instantiating MapperConfiguration");
                    var mapperConfig = new MapperConfiguration(cfg => {
                        cfg.AddProfile<InfrastructureAutoMapProfile>();
                    });
                    var mapper = mapperConfig.CreateMapper();

                    logger.Info("Instantiating the rest of the stuff");
                    IDeviceStateService deviceStateService = new MongoDeviceStateService(configuration, mapper);
                    IDateTimeService dateTimeService = new DateTimeService();
                    IAlertService alertService = new SendSlackAlertService(configuration, messageService, logger);
                    IPauseService pauseService = new PauseService();
                    IMonitorTools monitorTools = new MonitorTools();

                    logger.Info("Instantiating IMonitorExecuter list and executers");
                    var monitorExecuterList = new IMonitorExecuter[]
                    {
                        new KeepAliveMonitor(deviceLogService, dateTimeService, alertService),
                        new StateChangeMonitor(deviceLogService, alertService),
                        new DataValueToOldMonitor(deviceLogService, alertService, dateTimeService),
                        new CalculateAverageOverHoursMonitor(deviceLogService, dateTimeService, alertService, monitorTools),
                        new MonitorAliveMessageMonitor(dateTimeService, alertService, monitorTools)
                    };

                    System.Threading.Thread.Sleep(5000);

                    logger.Info("Instantiating PollingMonitor");
                    var monitor = new Application.Services.PollingMonitor.PollingMonitor(
                        configuration,
                        alertService,
                        deviceStateService,
                        monitorExecuterList,
                        logger,
                        pauseService,
                        monitorTools,
                        dateTimeService
                    );

                    logger.Info("Run in loop");
                    monitor.RunInLoop();

                    globalRetryCount = 10;
                }
                catch (Exception ex)
                {
                    logger.Fatal($"globalRetryCount:{globalRetryCount}");
                    logger.Fatal(ex.ToString());
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