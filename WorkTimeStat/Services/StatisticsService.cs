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

            return GetOfficialWorkdaysInMonth(month).Count();
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
        /// Calculates the amount of worked hours per passed month, only official workdays are counted
        /// </summary>
        internal static double CalcMonthlyTotalHours(int month)
        {
            if (month == 0)
            {
                return 0;
            }
            
            double workedHours = 0;
            foreach (WorkDay day in GetOfficialWorkdaysInMonth(month))
            {
                workedHours += CalcDailyTotalHours(day);
            }

            return workedHours;
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

            double mTotal = CalcMonthlyTotalHours(month);

            return mWorked - mTotal;
        }

        private static IEnumerable<WorkDay> GetOfficialWorkdaysInMonth(int month)
        {
            List<WorkDayType> dayTypes = GetOfficalWorkdayTypes();
            return FilterOfficalWorkdaysInMonth(month, dayTypes);
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
        /// Calculates the daily needed work amount based on the DayType.
        /// </summary>
        internal static double CalcDailyTotalHours(WorkDay day)
        {
            if (day == null)
            {
                return 0;
            }

            Dictionary<DayType, double> workHoursPerDayType = GetWorkHoursPerDayType();
            return workHoursPerDayType[DecideDayType(day)];
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
                double requiredHours = CalcDailyTotalHours(day);
                diff = workedHours - requiredHours;
            }

            return diff;
        }

        private static DayType DecideDayType(WorkDay day)
        {
            DayType dayType;
            if (IsDayOverworkOnlyDay(day))
            {
                dayType = DayType.OVERWORKDAY;
            }
            else if (IsDaySickDay(day))
            {
                dayType = DayType.SICKDAY;
            }
            else if (IsDayLeaveDay(day))
            {
                dayType = DayType.LEAVEDAY;
            }
            else if (IsDayHalfDay(day))
            {
                dayType = DayType.HALFDAY;
            }
            else
            {
                dayType = DayType.WORKDAY;
            }

            return dayType;
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

        private static bool IsDayHalfDay(WorkDay day)
        {
            return day.WorkDayType == WorkDayType.HALF_DAY;
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

        internal static List<UsageBlock> GetUsageFlowForDate(DateTime date)
        {
            WorkDay day = WorkDayService.GetDayAtDate(date);
            if (day == null)
            {
                return new List<UsageBlock>();
            }

            DateTime startDate = day.DayDate.Date + day.StartTime;
            DateTime endDate = day.DayDate.Date + day.EndTime;

            return UsageService.GetUsageListForTimeFrame(startDate, endDate);
        }

        internal static List<UsageBlock> GetUsageBreaksForDate(DateTime date)
        {
            WorkDay day = WorkDayService.GetDayAtDate(date);
            if (day == null)
            {
                return new List<UsageBlock>();
            }

            DateTime startDate = day.DayDate.Date + day.StartTime;
            DateTime endDate = day.DayDate.Date + day.EndTime;

            return UsageService.GetBreaksInUsageListForTimeFrame(startDate, endDate);
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
        public static List<WorkDayType> GetOfficalWorkdayTypes()
        {
            return new List<WorkDayType> { WorkDayType.WEEKDAY, WorkDayType.HALF_DAY };
        }

        /// <summary>
        /// Gives back the midpoint rounded number with two digits after zero for the given double
        /// </summary>
        private static double RoundToMidWithTwoPrecision(double num)
        {
            return Math.Round(num, 2, MidpointRounding.ToEven);
        }

        private static Dictionary<DayType, double> GetWorkHoursPerDayType()
        {
            return new Dictionary<DayType, double>
            {
                { DayType.WORKDAY, WorkKeeper.Instance.Settings.DailyWorkHours },
                { DayType.SICKDAY, 0 },
                { DayType.LEAVEDAY, 0 },
                { DayType.OVERWORKDAY, 0 },
                { DayType.HALFDAY, WorkKeeper.Instance.Settings.DailyWorkHours / 2d }
            };
        }
        #endregion

    }
}
