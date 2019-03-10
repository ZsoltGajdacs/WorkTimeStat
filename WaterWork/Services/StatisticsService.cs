﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterWork.Models;

namespace WaterWork.Services
{
    internal static class StatisticsService
    {
        private static readonly int WORKDAY_HOUR_LENGTH = 8;

        #region Year counters
        internal static double GetYearlyWorkedHours(ref WorkYear year)
        {
            double result = 0;
            foreach (WorkMonth workMonth in year.WorkMonths.Values)
            {
                result += GetMonthlyWorkedHours(workMonth);
            }

            return result;
        }

        internal static double GetYearlyTotalHours(ref WorkYear year)
        {
            return year.NoOfDaysWorked * WORKDAY_HOUR_LENGTH;
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

            return result;
        }

        internal static double GetMonthlyTotalHours(WorkMonth month)
        {
            return month.NoOfDaysWorked * WORKDAY_HOUR_LENGTH;
        }
        #endregion

        #region Day Counters
        internal static double GetDailyWorkedHours(WorkDay day)
        {
            double minutesWorked = (day.EndTime - day.StartTime).TotalMinutes;

            minutesWorked -= day.OtherBreakDuration;

            if (!day.IsLunchTimeWorkTime)
            {
                minutesWorked -= day.LunchBreakDuration;
            }

            return minutesWorked / 60;
        }

        internal static double GetDailyTotalHours()
        {
            return WORKDAY_HOUR_LENGTH;
        }
        #endregion

        #region Helpers
        internal static int GetTodayNum()
        {
            return int.Parse(DateTime.Today.Date.ToString("dd"));
        }

        internal static int GetThisMonthNum()
        {
            return int.Parse(DateTime.Today.Date.ToString("MM"));
        }

        internal static int GetThisYearNum()
        {
            return int.Parse(DateTime.Now.Year.ToString());
        }
        #endregion

    }
}
