using System;
using FakeItEasy;
using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.PollingMonitor.Services.Pause;
using LanSensor.Repository;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceState;
using NLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class PollingMonitorKeepAliveTest
    {
        private readonly IStateChangeMonitor _fakedStateChange;
        private readonly ILogger _fakedLogger;
        private readonly ITimeIntervalMonitor _fakedTimeIntervalMonitor;
        private readonly IPauseService _fakedPauseService;

        public PollingMonitorKeepAliveTest()
        {
            _fakedStateChange = A.Fake<IStateChangeMonitor>();
            _fakedLogger = A.Fake<ILogger>();
            _fakedTimeIntervalMonitor = A.Fake<ITimeIntervalMonitor>();
            _fakedPauseService = A.Fake<IPauseService>();
        }

        [Fact]
        public void KeepAliveAlertNotDone()
        {
            var testTime = new DateTime(2019, 1, 1, 0, 0, 0);

            var keepAliveDateTime = A.Fake<IDateTimeService>();
            var deviceStateRepository = A.Fake<IDeviceState>();

            A.CallTo(() => deviceStateRepository.GetLatestDeviceStateEntity(A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceStateEntity
                {
                    LastExecutedKeepAliveCheckDate = testTime,
                    LastKeepAliveAlert = testTime.AddHours(-1),
                    LastKnownKeepAliveDate = testTime.AddHours(-1)
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
                    MonitorConfiguration = new MonitorConfiguration()
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

            var alert = A.Fake<IAlert>();

            var keepAliveMonitor = new Services.Monitor.KeepAlive.KeepAliveMonitor(deviceLogRepository, nowDateTime);

            IPollingMonitor pollingMonitor = new Services.Monitor.PollingMonitor(
                config,
                alert,
                _fakedTimeIntervalMonitor,
                keepAliveMonitor,
                _fakedStateChange,
                deviceStateRepository,
                deviceLogRepository,
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
            var deviceStateRepository = A.Fake<IDeviceState>();

            A.CallTo(() => deviceStateRepository.GetLatestDeviceStateEntity(A<string>.Ignored, A<string>.Ignored)).Returns(
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
                    MonitorConfiguration = new MonitorConfiguration()
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

            var alert = A.Fake<IAlert>();

            var keepAliveMonitor = new Services.Monitor.KeepAlive.KeepAliveMonitor(deviceLogRepository, nowDateTime);

            IPollingMonitor pollingMonitor = new Services.Monitor.PollingMonitor(
                config,
                alert,
                _fakedTimeIntervalMonitor,
                keepAliveMonitor,
                _fakedStateChange,
                deviceStateRepository,
                deviceLogRepository,
                _fakedLogger,
                _fakedPauseService);

            pollingMonitor.RunThroughDeviceMonitors();
            pollingMonitor.Stop();

            A.CallTo(() => alert.SendKeepAliveMissingAlert(A<DeviceMonitor>.Ignored)).MustNotHaveHappened();

            Assert.NotNull(pollingMonitor);
        }
    }
}
