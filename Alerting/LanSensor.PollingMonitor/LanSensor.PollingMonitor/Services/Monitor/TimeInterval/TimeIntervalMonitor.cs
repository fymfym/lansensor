using System.Collections.Generic;
using System.Linq;
using LanSensor.Models.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Monitor.TimeInterval
{
    public class TimeIntervalComparer : ITimeIntervalMonitor
    {
        public Models.Configuration.TimeInterval GetFailedTimerInterval(
            IEnumerable<Models.Configuration.TimeInterval> timeIntervals, 
            DeviceLogEntity presenceRecord)
        {
            if (timeIntervals == null) return null;
            if (presenceRecord == null) return timeIntervals.FirstOrDefault();

            foreach (var interval in timeIntervals)
            {
                if (presenceRecord.DataValue != null && !string.IsNullOrEmpty(interval?.DataValue))
                {
                    var weekDays = PresenceInConfiguredWeekdays(interval, presenceRecord);
                    if (weekDays && interval.DataValue?.ToLower() != presenceRecord.DataValue.ToLower())
                        return interval;

                    var times = PresenceTimeInConfiguredTimes(interval, presenceRecord);
                    if (times && interval.DataValue?.ToLower() != presenceRecord.DataValue?.ToLower())
                        return interval;
                }
            }
            
            return  null;
        }

        private bool PresenceInConfiguredWeekdays(
            Models.Configuration.TimeInterval timeInterval,
            DeviceLogEntity presenceRedocrd)
        {
            if (timeInterval?.Weekdays == null) return true;
            if (!timeInterval.Weekdays.Any()) return true;

            return (timeInterval.Weekdays.Contains(presenceRedocrd.DateTime.DayOfWeek));
        }

        private bool PresenceTimeInConfiguredTimes(
            Models.Configuration.TimeInterval timeInterval,
            DeviceLogEntity presenceRedocrd)
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
