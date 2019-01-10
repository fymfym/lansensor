using System.Threading.Tasks;

namespace LanSensor.PollingMonitor.Services.Monitor
{
    public interface IPollingMonitor
    {
        bool StoppedIntentionaly { get; }
        void Stop();
        Task<int> Run();
    }
}
