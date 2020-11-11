using System;
using System.Collections.Generic;
using System.Linq;
using WaterWork.Models;
using WaterWork.Storage;

namespace WaterWork.Services
{
    internal static class StatisticsService
    {
        internal static void FullReCountWorkedDays()
        {
            for (int monthNum = 1; monthNum < 13; ++monthNum)
            {
                CountOfficialWorkedDaysInMonth(monthNum);
            }
        }

        #region Month counters
        internal static void CountOfficialWorkedDaysInMonth(int month)
        {
            IEnumerable<WorkDay> monthlyWorkdays = FilterOfficalWorkdaysInMonth(month);

            WorkKeeper keeper = WorkKeeper.Instance;
            keeper.DaysWorkedInMonth[month] = monthlyWorkdays.Count();
        }

        internal static double CalcMonthlyWorkedHours(int month)
        {
            IEnumerable<WorkDay> workDaysInMonth = FilterDaysWithWorkedHoursInMonth(month);

            double result = CalcWorkedHoursOnGivenDays(workDaysInMonth);

            return RoundToMidWithTwoPrecision(result);
        }

        /// <summary>
        /// Gives back the amount of hours based on the number of official workdays. 
        /// This excludes the sick and leave days on which worked hours were logged.
        /// </summary>
        internal static double CalcMonthlyTotalHours(int month)
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            return keeper.DaysWorkedInMonth[month] * keeper.Settings.DailyWorkHours;
        }

        /// <summary>
        /// Gives back the difference of the actual worked hours and the required ones. 
        /// Positive number: More hours than required
        /// Negative number: Less hours than needed
        /// </summary>
        internal static double CalcMonthlyHoursDifference(int month)
        {
            var daysWithWorkedHours = FilterDaysWithWorkedHoursInMonth(month);
            double mWorked = CalcWorkedHoursOnGivenDays(daysWithWorkedHours);

            double mTotal = CalcMonthlyTotalHours(month);

            return mWorked > mTotal ? mWorked - mTotal : mTotal - mWorked;
        }

        private static IEnumerable<WorkDay> FilterOfficalWorkdaysInMonth(int month)
        {
            WorkKeeper keeper = WorkKeeper.Instance;

            return keeper.WorkDays
                .Where(w => w.Key.Date.Month == month)
                .Where(d => !keeper.SickDays.Contains(d.Key))
                .Where(l => !keeper.LeaveDays.Contains(l.Key))
                .Select(d => d.Value);
        }

        private static IEnumerable<WorkDay> FilterDaysWithWorkedHoursInMonth(int month)
        {
            WorkKeeper keeper = WorkKeeper.Instance;

            return keeper.WorkDays
                .Where(w => w.Key.Date.Month == month)
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
            bool isSickday = IsDaySickDay(day);
            return isSickday ? 0 : WorkKeeper.Instance.Settings.DailyWorkHours;
        }

        /// <summary>
        /// Calculates the difference between the required and actually done worktime
        /// </summary>
        internal static double CalcDailyHoursDifference(WorkDay day)
        {
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

        private static bool IsDaySickDay(WorkDay day)
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            return keeper.SickDays.Exists(s => s.Date == day.DayDate);
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
            DateTime startDate = day.DayDate.Date + day.StartTime;
            DateTime endDate = day.DayDate.Date + day.EndTime;

            TimeSpan usageInTimeframe = UsageService.GetUsageForTimeframe(startDate, endDate);
            return RoundToMidWithTwoPrecision(usageInTimeframe.TotalHours);
        }

        internal static double GetUsageForMonth(int month)
        {
            IEnumerable<WorkDay> workDays = FilterDaysWithWorkedHoursInMonth(month);
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
