using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
