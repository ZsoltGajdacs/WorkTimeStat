using System;
using System.Globalization;

namespace WaterWork.Helpers
{
    internal class NumberFormatter
    {
        public static string NO_DATA = "-";

        internal static string FormatNum(double num)
        {
            return num == 0 ? NO_DATA : num.ToString(CultureInfo.InvariantCulture);
        }

        internal static DateTime RoundUpTime(DateTime date, TimeSpan roundTime)
        {
            return new DateTime((date.Ticks + roundTime.Ticks - 1) / roundTime.Ticks * roundTime.Ticks, date.Kind);
        }
    }
}
