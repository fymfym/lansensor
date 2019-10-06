using System.Threading.Tasks;

namespace LanSensor.PollingMonitor.Services.Monitor
{
    public interface IPollingMonitor
    {
        bool StoppedIntentionally { get; }
        void Stop();
        int RunInLoop();
    }
}
