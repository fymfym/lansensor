
namespace LanSensor.PollingMonitor.Services.Monitor
{
    public interface IPollingMonitor
    {
        bool StoppedIntentionally { get; }
        void Stop();
        void RunThroughDeviceMonitors();
        int RunInLoop();
    }
}
