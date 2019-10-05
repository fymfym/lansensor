using System.Threading;

namespace LanSensor.PollingMonitor.Services.Pause
{
    public class PauseService : IPauseService
    {
        public void Pause(int milliseconds)
        {
            Thread.Sleep(1000);
        }
    }
}
