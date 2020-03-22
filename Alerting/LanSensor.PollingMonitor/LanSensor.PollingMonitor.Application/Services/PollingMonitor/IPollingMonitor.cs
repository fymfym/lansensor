namespace LanSensor.PollingMonitor.Application.Services.PollingMonitor
{
    public interface IPollingMonitor
    {
        bool StoppedIntentionally { get; }
        void Stop();
        void RunThroughDeviceMonitors();
        int RunInLoop();
    }
}
