using System.Threading;
using LanSensor.PollingMonitor.Domain.Services;

namespace LanSensor.PollingMonitor.Application.Services.Pause
{
    public class PauseService : IPauseService
    {
        public void Pause(int milliseconds)
        {
            Thread.Sleep(1000);
        }
    }
}
