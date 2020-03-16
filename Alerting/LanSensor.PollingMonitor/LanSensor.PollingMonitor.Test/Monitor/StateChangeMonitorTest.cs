using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class StateChangeMonitorTest
    {
        private readonly StateChangeNotification _stateChangeNotification;


        public StateChangeMonitorTest()
        {
            _stateChangeNotification = new StateChangeNotification
            {
                OnDataValueChangeFrom = "changefrom",
                OnDataValueChangeTo = "changeto"
            };
        }

        [Fact]
        public void GetStateChangeNotification_No_Change_Test()
        {
            var deviceState = GetDeviceState("one");
            var deviceLog = GetDeviceLog("two");

            IStateChangeMonitor worker = new StateChangeMonitor();


            var result = worker.GetStateChangeFromToNotification(deviceState, deviceLog, _stateChangeNotification);

            Assert.Null(result);
        }

        [Fact]
        public void GetStateChangeNotification_From_Change_Test_Outside_Value()
        {
            var deviceState = GetDeviceState("one");
            var deviceLog = GetDeviceLog("two");

            IStateChangeMonitor worker = new StateChangeMonitor();
            var result = worker.GetStateChangeFromToNotification(deviceState, deviceLog, _stateChangeNotification);

            Assert.Null(result);
        }

        [Fact]
        public void GetStateChangeNotification_From_Change_Test_Inside_Value()
        {
            var deviceState = GetDeviceState(_stateChangeNotification.OnDataValueChangeFrom);
            var deviceLog = GetDeviceLog("one");

            IStateChangeMonitor worker = new StateChangeMonitor();
            var result = worker.GetStateChangeFromToNotification(deviceState, deviceLog, _stateChangeNotification);

            Assert.NotNull(result);
            Assert.True(result.ChangedFromValue);
            Assert.Equal(_stateChangeNotification.OnDataValueChangeFrom, result.DataValue);
        }

        [Fact]
        public void GetStateChangeNotification_To_Change_Test_Inside_Value()
        {
            var deviceState = GetDeviceState("one");
            var deviceLog = GetDeviceLog(_stateChangeNotification.OnDataValueChangeTo);

            IStateChangeMonitor worker = new StateChangeMonitor();
            var result = worker.GetStateChangeFromToNotification(deviceState, deviceLog, _stateChangeNotification);

            Assert.NotNull(result);
            Assert.True(result.ChangedToValue);
            Assert.Equal(_stateChangeNotification.OnDataValueChangeTo, result.DataValue);
        }

        [Fact]
        public void GetStateChangeNotificationIsChanged()
        {
            var deviceState = GetDeviceState("one");
            var deviceLog = GetDeviceLog(_stateChangeNotification.OnDataValueChangeTo);

            IStateChangeMonitor worker = new StateChangeMonitor();
            var result = worker.GetStateChangeNotification(deviceState, deviceLog, _stateChangeNotification);

            Assert.NotNull(result);
            Assert.True(result.ChangedToValue);
            Assert.Equal(_stateChangeNotification.OnDataValueChangeTo, result.DataValue);
        }

        [Fact]
        public void GetStateChangeNotificationNotChanged()
        {
            var deviceState = GetDeviceState("one");
            var deviceLog = GetDeviceLog("one");

            IStateChangeMonitor worker = new StateChangeMonitor();
            var result = worker.GetStateChangeNotification(deviceState, deviceLog, _stateChangeNotification);

            Assert.Null(result);
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
