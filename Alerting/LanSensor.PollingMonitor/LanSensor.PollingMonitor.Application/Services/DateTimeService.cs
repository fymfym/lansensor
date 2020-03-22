
using LanSensor.PollingMonitor.Domain.Services;

namespace LanSensor.PollingMonitor.Application.Services
{
    public class DateTimeService : IDateTimeService
    {
        public System.DateTime Now => System.DateTime.Now;
    }
}
