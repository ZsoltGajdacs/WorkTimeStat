using System;

namespace WorkTimeStat.Helpers
{
    internal static class Converter
    {
        public static TimeSpan ConvertHoursAndMinutesToTimeSpan(int hour, int minute)
        {
            return TimeSpan.FromHours(hour) + TimeSpan.FromMinutes(minute);
        }
    }
}
