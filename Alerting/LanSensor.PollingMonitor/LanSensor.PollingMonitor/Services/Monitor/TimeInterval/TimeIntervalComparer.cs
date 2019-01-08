using System.Collections.Generic;
using System.Linq;
using LanSensor.Models.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Monitor.TimeInterval
{
    public class TimeIntervalComparer : ITimeIntervalComparer
    {
        public Models.Configuration.TimeInterval GetFailedTimerInterval(
            IEnumerable<Models.Configuration.TimeInterval> timeIntervals, 
            DeviceLog presenceRedocrd)
        {
            foreach (var interval in timeIntervals)
            {
                if (presenceRedocrd.DataValue != null && !string.IsNullOrEmpty(interval?.DataValue))
                {
                    var weekDays = PresenceInConfiguredWeekdays(interval, presenceRedocrd);
                    if (weekDays && interval.DataValue?.ToLower() != presenceRedocrd.DataValue.ToLower())
                        return interval;

                    var times = PresenceTimeInConfiguredTimes(interval, presenceRedocrd);
                    if (times && interval.DataValue?.ToLower() != presenceRedocrd.DataValue?.ToLower())
                        return interval;
                }
            }
            return null;
        }

        private bool PresenceInConfiguredWeekdays(
            Models.Configuration.TimeInterval timeInterval,
            DeviceLog presenceRedocrd)
        {
            if (timeInterval?.Weekdays == null) return true;
            if (!timeInterval.Weekdays.Any()) return true;

            return (timeInterval.Weekdays.Contains(presenceRedocrd.DateTime.DayOfWeek));
        }

        private bool PresenceTimeInConfiguredTimes(
            Models.Configuration.TimeInterval timeInterval,
            DeviceLog presenceRedocrd)
        {
            if (timeInterval?.Times == null) return true;
            if (!timeInterval.Times.Any()) return true;

            var res = timeInterval.Times.Any(x =>
                GetTimeNumber(presenceRedocrd.DateTime.Hour, presenceRedocrd.DateTime.Minute) >= x.From.GetNumber() &&
                GetTimeNumber(presenceRedocrd.DateTime.Hour, presenceRedocrd.DateTime.Minute) <= x.To.GetNumber());
            return res;
        }

        private long GetTimeNumber(int hour, int minute)
        {
            return (hour * 60) + minute;
        }

    }
}
