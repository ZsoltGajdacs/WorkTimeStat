using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UsageWatcher.Models;
using WorkTimeStat.Enums;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Services
{
    internal static class StatisticsService
    {
        internal static void FullReCountWorkedDays()
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            for (int monthNum = 1; monthNum < 13; ++monthNum)
            {
                int count = CountOfficialWorkedDaysInMonth(monthNum);
                keeper.DaysWorkedInMonth[monthNum] = count;
            }
        }

        #region Month counters
        /// <summary>
        /// Only counts Normal workdays
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        internal static int CountOfficialWorkedDaysInMonth(int month)
        {
            if (month == 0)
            {
                return 0;
            }

            List<WorkDayType> dayTypes = new List<WorkDayType> { WorkDayType.WEEKDAY };

            IEnumerable<WorkDay> monthlyWorkdays = FilterOfficalWorkdaysInMonth(month, dayTypes);
            return monthlyWorkdays.Count();
        }

        internal static double CalcMonthlyWorkedHours(int month, List<WorkDayType> types)
        {
            if (month == 0)
            {
                return 0;
            }

            IEnumerable<WorkDay> workDaysInMonth = FilterWorkedDayInMonthByType(month, types);

            double result = CalcWorkedHoursOnGivenDays(workDaysInMonth);

            return RoundToMidWithTwoPrecision(result);
        }

        /// <summary>
        /// Gives back the previously calculated amount of hours based on the number of official workdays. 
        /// This excludes the sick and leave days on which worked hours were logged. 
        /// Only Normal days are counted.
        /// </summary>
        internal static double ReturnMonthlyTotalHours(int month)
        {
            if (month == 0)
            {
                return 0;
            }

            WorkKeeper keeper = WorkKeeper.Instance;
            return keeper.DaysWorkedInMonth[month] * keeper.Settings.DailyWorkHours;
        }

        /// <summary>
        /// Gives back the difference of the actual worked hours and the required ones. Only checks for normal workdays.
        /// Positive number: More hours than required
        /// Negative number: Less hours than needed
        /// </summary>
        internal static double CalcMonthlyHoursDifference(int month, List<WorkDayType> types)
        {
            if (month == 0)
            {
                return 0;
            }

            IEnumerable<WorkDay> normalWorkdays = FilterWorkedDayInMonthByType(month, types);

            double mWorked = CalcWorkedHoursOnGivenDays(normalWorkdays);

            double mTotal = ReturnMonthlyTotalHours(month);

            return mWorked > mTotal ? mTotal - mWorked : mWorked - mTotal;
        }

        private static IEnumerable<WorkDay> FilterOfficalWorkdaysInMonth(int month, List<WorkDayType> dayTypes)
        {
            WorkKeeper keeper = WorkKeeper.Instance;

            return keeper.WorkDays
                .Where(w => w.Key.Date.Month == month)
                .Where(d => !keeper.SickDays.Contains(d.Key))
                .Where(l => !keeper.LeaveDays.Contains(l.Key))
                .Where(o => !IsDayOverworkOnlyDay(o.Value))
                .Where(wdt => dayTypes.Contains(wdt.Value.WorkDayType))
                .Select(d => d.Value);
        }

        private static IEnumerable<WorkDay> FilterWorkedDayInMonthByType(int month, List<WorkDayType> dayType)
        {
            WorkKeeper keeper = WorkKeeper.Instance;

            return keeper.WorkDays
                .Where(w => w.Key.Date.Month == month)
                .Where(wdt => dayType.Contains(wdt.Value.WorkDayType))
                .Select(d => d.Value);
        }

        private static double CalcWorkedHoursOnGivenDays(IEnumerable<WorkDay> workDays)
        {
            double workedHours = 0;
            foreach (WorkDay day in workDays)
            {
                workedHours += CalcDailyWorkedHours(day);
            }

            return workedHours;
        }
        #endregion

        #region Day Counters
        internal static double CalcDailyWorkedHours(WorkDay day)
        {
            if (day == null)
            {
                return 0;
            }

            double minutesWorked = (day.EndTime - day.StartTime).TotalMinutes;

            minutesWorked -= day.OtherBreakDuration;
            minutesWorked -= day.OverWorkDuration;

            if (!day.IsLunchTimeWorkTime)
            {
                minutesWorked -= day.LunchBreakDuration;
            }

            return minutesWorked / 60;
        }

        /// <summary>
        /// Calculates the daily needed work amount based on whether the day was a sickday or not.
        /// </summary>
        internal static double CalcFullHoursForDay(WorkDay day)
        {
            if (day == null)
            {
                return 0;
            }

            bool isSickday = IsDaySickDay(day);
            bool isLeaveDay = IsDayLeaveDay(day);
            bool isOverworkDay = IsDayOverworkOnlyDay(day);
            return isSickday || isLeaveDay || isOverworkDay ? 0 : WorkKeeper.Instance.Settings.DailyWorkHours;
        }

        /// <summary>
        /// Calculates the difference between the required and actually done worktime
        /// </summary>
        internal static double CalcDailyHoursDifference(WorkDay day)
        {
            if (day == null)
            {
                return 0;
            }

            double workedHours = CalcDailyWorkedHours(day);
            
            double diff;
            if (IsDaySickDay(day))
            {
                diff = workedHours;
            }
            else
            {
                double requiredHours = CalcFullHoursForDay(day);
                diff = workedHours - requiredHours;
            }

            return diff;
        }

        internal static bool IsDayOverworkOnlyDay(WorkDay day)
        {
            return CalcDailyWorkedHours(day) == 0 && day.OverWorkDuration > 0;
        }

        private static bool IsDaySickDay(WorkDay day)
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            return keeper.SickDays.Exists(s => s.Date == day.DayDate);
        }

        private static bool IsDayLeaveDay(WorkDay day)
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            return keeper.LeaveDays.Exists(s => s.Date == day.DayDate);
        }
        #endregion

        #region UsageCounter
        /// <summary>
        /// Gives back the usage data for the given day
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        internal static double GetUsageForDay(WorkDay day)
        {
            if (day == null)
            {
                return 0;
            }

            if (day.DayDate != DateTime.Today)
            {
                return RoundToMidWithTwoPrecision(day.UsageTime.TotalHours);
            }
            else
            {
                return GetUsageForToday();
            }
        }

        internal static double GetUsageForToday()
        {
            WorkDay day = WorkDayService.GetCurrentDay();
            if (day == null)
            {
                return 0;
            }

            DateTime startDate = day.DayDate.Date + day.StartTime;
            DateTime endDate = day.DayDate.Date + day.EndTime;

            TimeSpan usageInTimeframe = UsageService.GetUsageForTimeframe(startDate, endDate);
            return RoundToMidWithTwoPrecision(usageInTimeframe.TotalHours);
        }

        internal static string GetUsageFlowForToday(TimeSpan minBlockLength)
        {
            WorkDay day = WorkDayService.GetCurrentDay();
            if (day == null)
            {
                return string.Empty;
            }

            DateTime startDate = day.DayDate.Date + day.StartTime;
            DateTime endDate = day.DayDate.Date + day.EndTime;

            List<UsageBlock> usageFlow = UsageService.GetUsageListForTimeFrame(startDate, endDate);

            return UsageBlocksToString(usageFlow, minBlockLength);
        }

        internal static string GetUsageBreaksForToday(TimeSpan minBlockLength)
        {
            WorkDay day = WorkDayService.GetCurrentDay();
            if (day == null)
            {
                return string.Empty;
            }

            DateTime startDate = day.DayDate.Date + day.StartTime;
            DateTime endDate = day.DayDate.Date + day.EndTime;

            List<UsageBlock> usageBreaks = UsageService.GetBreaksInUsageListForTimeFrame(startDate, endDate);

            return UsageBlocksToString(usageBreaks, minBlockLength);
        }

        private static string UsageBlocksToString(List<UsageBlock> blocks, TimeSpan minBlockLength)
        {
            LocalizationHelper locHelper = LocalizationHelper.Instance;
            StringBuilder sb = new StringBuilder();
            foreach (UsageBlock block in blocks)
            {
                if ((block.EndTime - block.StartTime) > minBlockLength)
                {
                    sb.Append(block.StartTime.ToShortTimeString());
                    sb.Append(" - ");
                    sb.Append(block.EndTime.ToShortTimeString());
                    sb.Append(": ");
                    sb.Append(Math.Round((block.EndTime - block.StartTime).TotalMinutes, MidpointRounding.ToEven)
                                                    .ToString(CultureInfo.CurrentCulture));
                    sb.Append(' ');
                    sb.AppendLine(locHelper.GetStringForKey("u_minute"));
                }
            }

            return sb.ToString();
        }

        internal static double GetUsageForMonth(int month, List<WorkDayType> types)
        {
            IEnumerable<WorkDay> workDays = FilterWorkedDayInMonthByType(month, types);
            IEnumerable<TimeSpan> usagesInMonth = workDays.Select(d => d.UsageTime);

            double result = 0;
            foreach (TimeSpan usage in usagesInMonth)
            {
                if (usage != default)
                {
                    result += usage.TotalHours;
                }
            }

            return RoundToMidWithTwoPrecision(result);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Gives back the midpoint rounded number with two digits after zero for the given double
        /// </summary>
        private static double RoundToMidWithTwoPrecision(double num)
        {
            return Math.Round(num, 2, MidpointRounding.ToEven);
        }
        #endregion

    }
}
