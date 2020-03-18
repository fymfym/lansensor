using System;
using System.Collections.Generic;
using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.TimeInterval;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.PollingMonitor.Services.Monitor;
using LanSensor.PollingMonitor.Services.Pause;
using LanSensor.Repository.Repositories;
using NLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class PollingMonitorKeepAliveTest
    {
        private readonly ILogger _fakedLogger;
        private readonly ITimeIntervalMonitor _fakedTimeIntervalMonitor;
        private readonly IPauseService _fakedPauseService;

        public PollingMonitorKeepAliveTest()
        {
            //_fakedStateChange = A.Fake<IStateChangeMonitor>();
            //_fakedLogger = A.Fake<ILogger>();
            //_fakedTimeIntervalMonitor = A.Fake<ITimeIntervalMonitor>();
            //_fakedPauseService = A.Fake<IPauseService>();
        }

        [Fact]
        public void KeepAliveAlertNotDone()
        {
            var testTime = new DateTime(2019, 1, 1, 0, 0, 0);

            var keepAliveDateTime = A.Fake<IDateTimeService>();
            var deviceStateService = A.Fake<IDeviceStateService>();

            //A.CallTo(() => deviceStateRepository.GetDeviceState(A<string>.Ignored, A<string>.Ignored)).Returns(
            //    new DeviceStateEntity
            //    {
            //        LastExecutedKeepAliveCheckDate = testTime,
            //        LastKeepAliveAlert = testTime.AddHours(-1),
            //        LastKnownKeepAliveDate = testTime.AddHours(-1)
            //    }
            //    );

            var nowDateTime = A.Fake<IDateTimeService>();
            A.CallTo(() => keepAliveDateTime.Now).Returns(testTime);
            A.CallTo(() => nowDateTime.Now).Returns(testTime.AddMinutes(61));

            var config = A.Fake<IServiceConfiguration>();
            A.CallTo(() => config.ApplicationConfiguration).Returns(
                new ApplicationConfiguration
                {
                    DeviceMonitors = new[]
                    {
                        new DeviceMonitor
                        {
                            DeviceGroupId = "dg",
                            DeviceId = "di",
                            KeepAlive = new KeepAlive
                            {
                                KeepAliveDataType = "keepalive",
                                MaxMinutesSinceKeepAlive = 60,
                                NotifyOnceOnly = true
                            }
                        }
                    },
                    MonitorConfiguration = new MonitorConfiguration
                    {
                        PollingIntervalSeconds = 1
                    }});

            var deviceLogRepository = A.Fake<IDeviceLogRepository>();
            A.CallTo(() => deviceLogRepository.GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceLogEntity
                {
                    DataType = "sometype",
                    DeviceGroupId = "dg",
                    DeviceId = "di",
                    DateTime = testTime
                }
            );

            A.CallTo(() => deviceLogRepository.GetLatestKeepAlive(A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceLogEntity
                {
                    DataType = "keepalive",
                    DeviceGroupId = "dg",
                    DeviceId = "di",
                    DateTime = testTime
                }
                );

            var alert = A.Fake<IAlertService>();
            var list = new List<IMonitorExecuter>();

            IPollingMonitor pollingMonitor = new Application.Services.PollingMonitor.PollingMonitor(
                config,
                alert,
                deviceStateService,
                list,
                _fakedLogger,
                _fakedPauseService);

            pollingMonitor.RunThroughDeviceMonitors();
            pollingMonitor.Stop();

            A.CallTo(() => alert.SendKeepAliveMissingAlert(A<DeviceMonitor>.Ignored)).MustNotHaveHappened();

            Assert.NotNull(pollingMonitor);
        }

        [Fact]
        public void KeepAliveAlertDone()
        {
            var testTime = new DateTime(2019, 1, 1, 0, 0, 0);

            var keepAliveDateTime = A.Fake<IDateTimeService>();
            var deviceStateRepository = A.Fake<IDeviceStateService>();

            A.CallTo(() => deviceStateRepository.GetDeviceState(A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceStateEntity
                {
                    LastExecutedKeepAliveCheckDate = testTime,
                    LastKeepAliveAlert = testTime.AddHours(-1),
                    LastKnownKeepAliveDate = testTime.AddHours(-2)
                }
                );

            var nowDateTime = A.Fake<IDateTimeService>();
            A.CallTo(() => keepAliveDateTime.Now).Returns(testTime);
            A.CallTo(() => nowDateTime.Now).Returns(testTime.AddMinutes(61));

            var config = A.Fake<IServiceConfiguration>();
            A.CallTo(() => config.ApplicationConfiguration).Returns(
                new ApplicationConfiguration
                {
                    DeviceMonitors = new[]
                    {
                        new DeviceMonitor
                        {
                            DeviceGroupId = "dg",
                            DeviceId = "di",
                            KeepAlive = new KeepAlive
                            {
                                KeepAliveDataType = "keepalive",
                                MaxMinutesSinceKeepAlive = 60,
                                NotifyOnceOnly = true
                            }
                        }
                    },
                    MonitorConfiguration = new MonitorConfiguration
                    {
                        PollingIntervalSeconds = 1
                    }});

            var deviceLogRepository = A.Fake<IDeviceLogRepository>();
            A.CallTo(() => deviceLogRepository.GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceLogEntity
                {
                    DataType = "sometype",
                    DeviceGroupId = "dg",
                    DeviceId = "di",
                    DateTime = testTime
                }
            );

            A.CallTo(() => deviceLogRepository.GetLatestKeepAlive(A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceLogEntity
                {
                    DataType = "keepalive",
                    DeviceGroupId = "dg",
                    DeviceId = "di",
                    DateTime = testTime
                }
                );

            var alert = A.Fake<IAlertService>();
            var list = new List<IMonitorExecuter>();

            IPollingMonitor pollingMonitor = new Application.Services.PollingMonitor.PollingMonitor(
                config,
                alert,
                deviceStateRepository,
                list,
                _fakedLogger,
                _fakedPauseService);

            pollingMonitor.RunThroughDeviceMonitors();
            pollingMonitor.Stop();

            A.CallTo(() => alert.SendKeepAliveMissingAlert(A<DeviceMonitor>.Ignored)).MustNotHaveHappened();

            Assert.NotNull(pollingMonitor);
        }
    }
}
