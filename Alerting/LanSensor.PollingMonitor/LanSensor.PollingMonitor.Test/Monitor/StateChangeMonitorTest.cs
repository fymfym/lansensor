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
        public void GetStateChangeNotification_NoLog_ReturnsSameState()
        {
            IMonitorExecuter worker = new StateChangeMonitor(_fakedDeviceLogService, _fakedAlertService);

            var deviceState = new DeviceStateEntity
            {
                LastKnownDataValueDate = DateTime.MinValue
            };

            var deviceMonitor = GetDeviceMonitor("one", "one", false);
            worker.Run(deviceState, deviceMonitor);

            A.CallTo(() =>
                _fakedAlertService.SendStateChangeAlert(
                    A<StateChangeResult>.Ignored,
                    A<DeviceMonitor>.Ignored)).MustNotHaveHappened();
        }


        [Fact]
        public void GetStateChangeNotification_NoChangeInState_NoMessageSend()
        {
            var deviceLogList = GetDeviceLogList("one");

            A.CallTo(() => _fakedDeviceLogService.GetPresenceListSince(
                A<string>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<DateTime>.Ignored)).Returns(deviceLogList);

            IMonitorExecuter worker = new StateChangeMonitor(_fakedDeviceLogService, _fakedAlertService);

            var deviceState = new DeviceStateEntity
            {
                LastKnownDataValueDate = DateTime.MinValue
            };

            var deviceMonitor = GetDeviceMonitor("None", "one", false);
            worker.Run(deviceState, deviceMonitor);

            A.CallTo(() =>
                _fakedAlertService.SendStateChangeAlert(
                    A<StateChangeResult>.Ignored,
                    A<DeviceMonitor>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void GetStateChangeNotification_StateChange_OutsideRequiredValues()
        {
            var deviceState = GetDeviceState("one");

            IMonitorExecuter worker = new StateChangeMonitor(_fakedDeviceLogService, _fakedAlertService);
            var monitor = GetDeviceMonitor("", "", false);
            var result = worker.Run(deviceState, monitor);

            Assert.NotNull(result);
            Assert.Equal(deviceState.LastKnownDataValueDate, result.LastKnownDataValueDate);
        }

        [Fact]
        public void GetStateChangeNotification_StateChangeFromOneToTwo_SendMessage()
        {
            var deviceState = GetDeviceState("one");
            _stateChangeNotification.OnDataValueChangeTo = null;
            var deviceLogList = GetDeviceLogList("two");

            IMonitorExecuter worker = new StateChangeMonitor(_fakedDeviceLogService, _fakedAlertService);

            A.CallTo(() => _fakedDeviceLogService.GetPresenceListSince(
                A<string>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<DateTime>.Ignored)).Returns(deviceLogList);

            var monitor = GetDeviceMonitor("one", "two", false);
            var result = worker.Run(deviceState, monitor);

            A.CallTo(() =>
                    _fakedAlertService.SendStateChangeAlert(A<StateChangeResult>._, A<DeviceMonitor>._))
                .MustHaveHappened();
            Assert.Equal("two", result.LastKnownDataValue);
        }

        [Fact]
        public void GetStateChangeNotification_ChangeToWantedValue_NoMessage()
        {
            var deviceState = GetDeviceState("two");
            var deviceLogList = GetDeviceLogList("four");

            A.CallTo(() => _fakedDeviceLogService.GetPresenceListSince(
                A<string>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<DateTime>.Ignored)).Returns(deviceLogList);

            IMonitorExecuter worker = new StateChangeMonitor(_fakedDeviceLogService, _fakedAlertService);
            var monitor = GetDeviceMonitor(null, "two", false);
            worker.Run(deviceState, monitor);

            A.CallTo(() =>
                    _fakedAlertService.SendStateChangeAlert(A<StateChangeResult>._, A<DeviceMonitor>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public void GetStateChangeNotification_ChangeToWantedValueMessageAlreadySend_NoMessage()
        {
            var deviceState = GetDeviceState("two");
            var deviceLogList = GetDeviceLogList("one");

            A.CallTo(() => _fakedDeviceLogService.GetPresenceListSince(
                A<string>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<DateTime>.Ignored)).Returns(deviceLogList);

            IMonitorExecuter worker = new StateChangeMonitor(_fakedDeviceLogService, _fakedAlertService);
            var monitor = GetDeviceMonitor(null, "two", false);
            worker.Run(deviceState, monitor);

            A.CallTo(() =>
                    _fakedAlertService.SendStateChangeAlert(A<StateChangeResult>._, A<DeviceMonitor>._))
                .MustNotHaveHappened();
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

        private static List<DeviceLogEntity> GetDeviceLogList(string dataValue)
        {
            return new List<DeviceLogEntity>
            {
                GetDeviceLog(dataValue)
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
