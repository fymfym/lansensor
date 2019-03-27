using System;
using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Alert.Slack;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor.DataValueToOld;
using LanSensor.PollingMonitor.Services.Monitor.Keepalive;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceLog.MySqlDeviceLog;
using LanSensor.Repository.DeviceState;
using LanSensor.Repository.DeviceState.MySqlDeviceState;

namespace LanSensor.PollingMonitor
{
    static class Program
    {
        public static void Main(string[] args)
        {
            int globlaRetryCount = 10;
            while (globlaRetryCount > 0)
            {
                IConfiguration configuration = new Configuration();

                try
                {
                    IDeviceLogRepository deviceLogRepository = new MySqlDataStoreRepository(configuration);
                    IDeviceStateRepository deviceStateRepository = new MySqlDeviceStateRepository(configuration);
                    IGetDateTime getDate = new GetDateTime();
                    IAlert alerter = new SendSlackAlert(configuration);
                    ITimeIntervalMonitor stateCheckComparer = new TimeIntervalComparer();
                    IKeepaliveMonitor keepalive = new KeepaliveMonitor(deviceLogRepository, getDate);
                    IStateChangeMonitor stateChange = new StateChangeMonitor();
                    IDataValueToOldMonitor dataValueToOldMonitor = new DataValueToOldMonitor();
                    var monitor = new Services.Monitor.PollingMonitor(
                        configuration, 
                        deviceLogRepository, 
                        alerter, 
                        stateCheckComparer, 
                        keepalive, 
                        stateChange,
                        deviceStateRepository, 
                        getDate,
                        dataValueToOldMonitor);
                    var runTask =  monitor.Run();
                    Task.WaitAll(runTask);
                    globlaRetryCount = 10;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    globlaRetryCount--;
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}