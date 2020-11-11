using System;
using System.Globalization;
using System.Windows;
using WaterWork.Models;
using WaterWork.Services;

namespace WaterWork.Windows
{
    public partial class StatisticsWindow : Window
    {
        internal StatisticsWindow(double dailyWorkHours)
        {
            InitializeComponent();
            mainGrid.DataContext = this;

            StatisticsDto dto = new StatisticsDto
            {
                dailyWorkHours = dailyWorkHours
            };

            // Monthly
            int thisMonth = DateTime.Now.Month;
            dto.mWorkedHours = StatisticsService.CalcMonthlyWorkedHours(thisMonth);
            dto.mFullHours = StatisticsService.CalcMonthlyTotalHours(thisMonth);
            dto.mCalcHours = StatisticsService.GetUsageForMonth(thisMonth);
            dto.mLeftHours = AddPlusIfNeeded(StatisticsService.CalcMonthlyHoursDifference(thisMonth));

            // Daily
            WorkDay today = WorkDayService.GetCurrentDay();
            dto.dWorkedHours = StatisticsService.CalcDailyWorkedHours(today);
            dto.dFullHours = StatisticsService.CalcFullHoursForDay(today);
            dto.dCalcHours = StatisticsService.GetUsageForToday();
            dto.dLeftHours = AddPlusIfNeeded(StatisticsService.CalcDailyHoursDifference(today));

            // Yesterday
            WorkDay yesterWorkday = WorkDayService.GetYesterWorkDay();
            dto.ywdWorkedHours = StatisticsService.CalcDailyWorkedHours(yesterWorkday);
            dto.ywdFullHours = StatisticsService.CalcFullHoursForDay(yesterWorkday);
            dto.ywdCalcHours = StatisticsService.GetUsageForDay(yesterWorkday);
            dto.ywdLeftHours = AddPlusIfNeeded(StatisticsService.CalcDailyHoursDifference(yesterWorkday));

            AssignLabels(ref dto);
        }

        private void AssignLabels(ref StatisticsDto dto)
        {
            yesterworkdayWorkedHours.Content = dto.ywdWorkedHours;
            yesterworkdayFullHours.Content = dto.ywdFullHours;
            yesterworkdayCalcHours.Content = dto.ywdCalcHours;
            yesterworkdayLeftHours.Content =dto.ywdLeftHours;

            todayWorkedHours.Content = dto.dWorkedHours;
            todayFullHours.Content = dto.dFullHours;
            todayCalcHours.Content = dto.dCalcHours;
            todayLeftHours.Content = dto.dLeftHours;

            monthlyWorkedHours.Content = dto.mWorkedHours;
            monthlyFullHours.Content = dto.mFullHours;
            monthlyCalcHours.Content = dto.mCalcHours;
            monthlyLeftHours.Content = dto.mLeftHours;
        }

        private static string AddPlusIfNeeded(double num)
        {
            num = Math.Round(num, 2, MidpointRounding.ToEven);
            return num > 0 ? "+" + num : num.ToString(CultureInfo.InvariantCulture);
        }

        private class StatisticsDto
        {
            public double dailyWorkHours;

            // Monthly
            public double mWorkedHours, mFullHours, mCalcHours;
            public string mLeftHours;

            // Daily
            public double dWorkedHours, dFullHours, dCalcHours;
            public string dLeftHours;

            // Yesterday
            public double ywdWorkedHours, ywdFullHours, ywdCalcHours;
            public string ywdLeftHours;
        }
    }
}
