using System;
using WaterWork.Models;
using WaterWork.Storage;

namespace WaterWork.Services
{
    internal static class StatisticsService
    {
        #region Year counters
        internal static double GetYearlyWorkedHours(ref WorkYear year)
        {
            double result = 0;
            foreach (WorkMonth workMonth in year.WorkMonths.Values)
            {
                result += GetMonthlyWorkedHours(workMonth);
            }

            return RoundToMidWithTwoPrecision(result);
        }

        internal static double GetYearlyTotalHours(ref WorkYear year, double workdayHourLength)
        {
            return year.NoOfDaysWorked * workdayHourLength;
        }
        #endregion

        #region Month counters
        internal static double GetMonthlyWorkedHours(WorkMonth month)
        {
            double result = 0;
            foreach (WorkDay workDay in month.WorkDays.Values)
            {

                result += GetDailyWorkedHours(workDay);
            }

            return RoundToMidWithTwoPrecision(result);
        }

        internal static double GetMonthlyTotalHours(WorkMonth month, double workdayHourLength)
        {
            return month.NoOfDaysWorked * workdayHourLength;
        }
        #endregion

        #region Day Counters
        internal static double GetDailyWorkedHours(WorkDay day)
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
        #endregion

        #region UsageCounter
        /// <summary>
        /// Gives back the usage data for the given day
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        internal static double GetUsageForDay(WorkDay day)
        {
            if (day != null)
            {
                UsageKeeper usage = UsageKeeper.Instance;
                DateTime startDate = day.DayDate.Date + day.StartTime;
                DateTime endDate = day.DayDate.Date + day.EndTime;

                TimeSpan usageInTimeframe = usage.GetUsageForTimeframe(startDate, endDate);
                double roundedHours = RoundToMidWithTwoPrecision(usageInTimeframe.TotalHours);
                return roundedHours;
            }
            else
            {
                return 0.0;
            }
        }

        internal static double GetUsageForMonth(WorkMonth month)
        {
            throw new NotImplementedException();
        }

        internal static double GetUsageForYear(ref WorkYear year)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Helpers
        internal static int GetTodayNum()
        {
            return GetDayForDate(DateTime.Today);
        }

        internal static int GetThisMonthNum()
        {
            return GetMonthForDate(DateTime.Today);
        }

        internal static int GetThisYearNum()
        {
            return GetYearForDate(DateTime.Today);
        }

        internal static int GetYearForDate(DateTime date)
        {
            return int.Parse(date.ToString("yyyy"));
        }

        internal static int GetMonthForDate(DateTime date)
        {
            return int.Parse(date.ToString("MM"));
        }

        internal static int GetDayForDate(DateTime date)
        {
            return int.Parse(date.ToString("dd"));
        }

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
