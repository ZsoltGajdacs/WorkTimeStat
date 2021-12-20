using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTimeStat.Helpers
{
    internal static class Rounder
    {
        internal static DateTime RoundUpTime(DateTime date, TimeSpan roundTime)
        {
            return new DateTime((date.Ticks + roundTime.Ticks - 1) / roundTime.Ticks * roundTime.Ticks, date.Kind);
        }

        internal static DateTime RoundToClosestTime(DateTime date, TimeSpan roundTime)
        {
            return new DateTime((date.Ticks + (roundTime.Ticks / 2) + 1) / roundTime.Ticks * roundTime.Ticks, date.Kind);
        }

        internal static TimeSpan RoundToClosestTime(TimeSpan time, TimeSpan roundTime)
        {
            return new DateTime((time.Ticks + (roundTime.Ticks / 2) + 1) / roundTime.Ticks * roundTime.Ticks, DateTimeKind.Local).TimeOfDay;
        }

        internal static DateTime RoundDownTime(DateTime date, TimeSpan roundTime)
        {
            return new DateTime(date.Ticks / roundTime.Ticks * roundTime.Ticks, date.Kind);
        }

        /// <summary>
        /// Gives back the midpoint rounded number with two digits after zero for the given double
        /// </summary>
        internal static double RoundToMidWithTwoPrecision(double num)
        {
            return Math.Round(num, 2, MidpointRounding.ToEven);
        }
    }
}
