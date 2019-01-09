using System;
using FakeItEasy;
using LanSensor.Models;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceState;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class StateChangeMonitorTest
    {
        private readonly StateChangeNotification _stateChangeNotification;

        public StateChangeMonitorTest()
        {
            _stateChangeNotification = new StateChangeNotification()
            {
                OnDataValueChangeFrom = "changefrom",
                OnDataValueChangeTo = "changeto"
            };
        }

        [Fact]
        public async void GetStateChangeNotification_No_Change_Test()
        {
            var fakedDeviceLogRepository = A.Fake<IDeviceLogRepository>();
            var fakedDeviceStateRepository = A.Fake<IDeviceStateRepository>();
            IStateChangeMonitor worker = new StateChangeMonitor(fakedDeviceStateRepository, fakedDeviceLogRepository);

            var result = await worker.GetStateChangeNotification("123","456", _stateChangeNotification);

            Assert.Null(result);
        }

        [Fact]
        public async void GetStateChangeNotification_From_Change_Test_Outsiode_Value()
        {
            DateTime testDate = DateTime.Now;
            string testDataValue = "test";

            var fakedDeviceLogRepository = A.Fake<IDeviceLogRepository>();
            A.CallTo(() =>
                fakedDeviceLogRepository.GetPresenceListSince(
                    A<string>.Ignored, A<string>.Ignored,
                    A<DateTime>.Ignored)).Returns(
                new []
                {
                    new DeviceLogEntity()
                    {
                        DataType = "type",
                        DataValue = testDataValue,
                        DateTime = testDate
                    }
                }
                );

            var fakedDeviceStateRepository = A.Fake<IDeviceStateRepository>();

            A.CallTo(() => fakedDeviceStateRepository.GetLatestDeviceStateEntity(A<string>.Ignored, A<string>.Ignored))
                .Returns(
                    new DeviceStateEntity()
                    {
                        LastKnownDataValueDate = testDate.AddHours(-1),
                        LastKnownDataValue = testDataValue
                    }
                    );

            IStateChangeMonitor worker = new StateChangeMonitor(fakedDeviceStateRepository, fakedDeviceLogRepository);

            var result = await worker.GetStateChangeNotification("123", "456", _stateChangeNotification);

            A.CallTo(() => fakedDeviceStateRepository.SetDeviceStateEntity(A<DeviceStateEntity>.Ignored))
                .MustHaveHappened();
            Assert.Null(result);
        }

        [Fact]
        public async void GetStateChangeNotification_From_Change_Test_Inside_Value()
        {
            DateTime testDate = DateTime.Now;
            string testDataValue = _stateChangeNotification.OnDataValueChangeFrom;

            var fakedDeviceLogRepository = A.Fake<IDeviceLogRepository>();
            A.CallTo(() =>
                fakedDeviceLogRepository.GetPresenceListSince(
                    A<string>.Ignored, A<string>.Ignored,
                    A<DateTime>.Ignored)).Returns(
                new[]
                {
                    new DeviceLogEntity()
                    {
                        DataType = "type",
                        DataValue = testDataValue,
                        DateTime = testDate
                    }
                }
            );

            var fakedDeviceStateRepository = A.Fake<IDeviceStateRepository>();

            A.CallTo(() => fakedDeviceStateRepository.GetLatestDeviceStateEntity(A<string>.Ignored, A<string>.Ignored))
                .Returns(
                    new DeviceStateEntity()
                    {
                        LastKnownDataValueDate = testDate.AddHours(-1),
                        LastKnownDataValue = testDataValue
                    }
                );

            IStateChangeMonitor worker = new StateChangeMonitor(fakedDeviceStateRepository, fakedDeviceLogRepository);

            var result = await worker.GetStateChangeNotification("123", "456", _stateChangeNotification);

            A.CallTo(() => fakedDeviceStateRepository.SetDeviceStateEntity(A<DeviceStateEntity>.Ignored))
                .MustHaveHappened();
            Assert.NotNull(result);
            Assert.True(result.ChangedFromValue);
            Assert.Equal(_stateChangeNotification.OnDataValueChangeFrom, result.DataValue);
        }

        [Fact]
        public async void GetStateChangeNotification_To_Change_Test_Inside_Value()
        {
            DateTime testDate = DateTime.Now;
            string testDataValue = _stateChangeNotification.OnDataValueChangeTo;

            var fakedDeviceLogRepository = A.Fake<IDeviceLogRepository>();
            A.CallTo(() =>
                fakedDeviceLogRepository.GetPresenceListSince(
                    A<string>.Ignored, A<string>.Ignored,
                    A<DateTime>.Ignored)).Returns(
                new[]
                {
                    new DeviceLogEntity()
                    {
                        DataType = "type",
                        DataValue = testDataValue,
                        DateTime = testDate
                    }
                }
            );

            var fakedDeviceStateRepository = A.Fake<IDeviceStateRepository>();

            A.CallTo(() => fakedDeviceStateRepository.GetLatestDeviceStateEntity(A<string>.Ignored, A<string>.Ignored))
                .Returns(
                    new DeviceStateEntity()
                    {
                        LastKnownDataValueDate = testDate.AddHours(-1),
                        LastKnownDataValue = testDataValue
                    }
                );

            IStateChangeMonitor worker = new StateChangeMonitor(fakedDeviceStateRepository, fakedDeviceLogRepository);

            var result = await worker.GetStateChangeNotification("123", "456", _stateChangeNotification);

            A.CallTo(() => fakedDeviceStateRepository.SetDeviceStateEntity(A<DeviceStateEntity>.Ignored))
                .MustHaveHappened();
            Assert.NotNull(result);
            Assert.True(result.ChangedToValue);
            Assert.Equal(_stateChangeNotification.OnDataValueChangeTo, result.DataValue);
        }

    }
}
