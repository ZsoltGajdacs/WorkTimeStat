using System;
using System.Globalization;

namespace WorkTimeStat.Helpers
{
    internal static class NumberFormatter
    {
        public const string NO_DATA = "-";

        internal static string FormatNum(double num)
        {
            return num == 0 ? NO_DATA : num.ToString(CultureInfo.InvariantCulture);
        }
    }
}
