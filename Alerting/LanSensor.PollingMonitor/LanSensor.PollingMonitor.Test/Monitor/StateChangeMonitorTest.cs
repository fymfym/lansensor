using System;
using System.Collections.Generic;
using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.StateChange;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class StateChangeMonitorTest
    {
        private readonly StateChangeNotification _stateChangeNotification;
        private readonly IDeviceLogService _fakedDeviceLogService;
        private readonly IAlertService _fakedAlertService;


        public StateChangeMonitorTest()
        {
            _stateChangeNotification = new StateChangeNotification
            {
                OnDataValueChangeFrom = "changefrom",
                OnDataValueChangeTo = "changeto"
            };

            _fakedDeviceLogService = A.Fake<IDeviceLogService>();
            _fakedAlertService = A.Fake<IAlertService>();
        }

        [Fact]
        public void GetStateChangeNotificationCanMonitorRun_NoObject_ReturnFalse()
        {
            var monitor = new StateChangeMonitor(_fakedDeviceLogService, _fakedAlertService);

            var res = monitor.CanMonitorRun(new DeviceMonitor());

            Assert.False(res);
        }

        [Fact]
        public void GetStateChangeNotificationCanMonitorRun_Null_ReturnFalse()
        {
            var monitor = new StateChangeMonitor(_fakedDeviceLogService, _fakedAlertService);

            var res = monitor.CanMonitorRun(null);

            Assert.False(res);
        }


        [Fact]
        public void GetStateChangeNotification_No_Change_Test()
        {
            var deviceLogList = new List<DeviceLogEntity>
            {
                GetDeviceLog("one")
            };

            var fakedDeviceLogService = A.Fake<IDeviceLogService>();

            A.CallTo(() => fakedDeviceLogService.GetPresenceListSince(
                A<string>.Ignored,
                A<string>.Ignored,
                A<DateTime>.Ignored)).Returns(deviceLogList);

            // IMonitorExecuter worker = new StateChangeMonitor(fakedDeviceLogService, fakedAlertService);

            //var result = worker.Run(deviceState, GetDeviceMonitor("one","one", false));

            //A.CallTo(() =>
            //    fakedAlertService.SendStateChangeAlert(
            //        A<StateChangeResult>.Ignored,
            //        A<DeviceMonitor>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void GetStateChangeNotification_From_Change_Test_Outside_Value()
        {
            var deviceState = GetDeviceState("one");
            var deviceLog = GetDeviceLog("one");

            //IStateChangeMonitor worker = new StateChangeMonitor();
            //var result = worker.GetStateChangeFromToNotification(deviceState, deviceLog, _stateChangeNotification);

            //Assert.Null(result);
        }

        [Fact]
        public void GetStateChangeNotification_From_Change_Test_Inside_Value()
        {
            var deviceState = GetDeviceState("two");
            var deviceLog = GetDeviceLog("one");
            _stateChangeNotification.OnDataValueChangeTo = null;

            //IStateChangeMonitor worker = new StateChangeMonitor();
            //var result = worker.GetStateChangeFromToNotification(deviceState, deviceLog, _stateChangeNotification);

            //Assert.NotNull(result);
            //Assert.True(result.ChangedFromValue);
            //Assert.Equal(_stateChangeNotification.OnDataValueChangeFrom, result.DataValue);
        }

        [Fact]
        public void GetStateChangeNotification_ToChangeTestInsideValue_ReturnsNotNull()
        {
            var deviceState = GetDeviceState("two");
            var deviceLog = GetDeviceLog("four");

            //IStateChangeMonitor worker = new StateChangeMonitor();
            //var result = worker.GetStateChangeFromToNotification(deviceState, deviceLog, _stateChangeNotification);

            //Assert.NotNull(result);
            //Assert.True(result.ChangedToValue);
            //Assert.Equal(_stateChangeNotification.OnDataValueChangeTo, result.DataValue);
        }

        [Fact]
        public void GetStateChangeNotificationIsChanged()
        {
            var deviceState = GetDeviceState("one");
            var deviceLog = GetDeviceLog(_stateChangeNotification.OnDataValueChangeTo);

            //IStateChangeMonitor worker = new StateChangeMonitor();
            //var result = worker.GetStateChangeNotification(deviceState, deviceLog, _stateChangeNotification);

            //Assert.NotNull(result);
            //Assert.True(result.ChangedToValue);
            //Assert.Equal(_stateChangeNotification.OnDataValueChangeTo, result.DataValue);
        }

        [Fact]
        public void GetStateChangeNotificationNotChanged()
        {
            var deviceState = GetDeviceState("one");
            var deviceLog = GetDeviceLog("one");

            //IStateChangeMonitor worker = new StateChangeMonitor();
            //var result = worker.GetStateChangeNotification(deviceState, deviceLog, _stateChangeNotification);

            //Assert.Null(result);
        }

        private static DeviceMonitor GetDeviceMonitor(string fromValue, string toValue, bool onEveryChange)
        {
            return new DeviceMonitor
            {
                DeviceId = "",
                DeviceGroupId = "",
                StateChangeNotification = new StateChangeNotification
                {
                    OnDataValueChangeTo = toValue,
                    OnDataValueChangeFrom = fromValue,
                    OnEveryChange = onEveryChange
                }
            };
        }

        private static DeviceLogEntity GetDeviceLog(string dataValue)
        {
            return new DeviceLogEntity
            {
                DataValue = dataValue
            };
        }

        private static DeviceStateEntity GetDeviceState(string lastKnownDataValue)
        {
            return new DeviceStateEntity
            {
                LastKnownDataValue = lastKnownDataValue
            };
        }
    }
}
