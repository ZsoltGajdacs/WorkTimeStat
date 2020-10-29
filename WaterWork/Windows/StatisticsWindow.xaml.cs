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
            dto.mWorkedHours = StatisticsService.GetMonthlyWorkedHours(thisMonth);
            dto.mFullHours = StatisticsService.GetMonthlyTotalHours(thisMonth, dailyWorkHours);
            dto.mCalcHours = StatisticsService.GetUsageForMonth(thisMonth);

            // Daily
            WorkDay today = WorkDayService.GetCurrentDay();
            dto.dWorkedHours = StatisticsService.GetDailyWorkedHours(today);
            dto.dCalcHours = StatisticsService.GetUsageForToday();

            // Yesterday
            WorkDay yesterWorkday = WorkDayService.GetYesterWorkDay();
            dto.ywdWorkedHours = StatisticsService.GetDailyWorkedHours(yesterWorkday);
            dto.ywdCalcHours = StatisticsService.GetUsageForDay(yesterWorkday);

            AssignLabels(ref dto);
        }

        private void AssignLabels(ref StatisticsDto dto)
        {
            yesterworkdayWorkedHours.Content = dto.ywdWorkedHours;
            yesterworkdayFullHours.Content = dto.dailyWorkHours;
            yesterworkdayCalcHours.Content = dto.ywdCalcHours;
            yesterworkdayLeftHours.Content = AddPlusIfNeeded(dto.ywdWorkedHours - dto.dailyWorkHours);

            todayWorkedHours.Content = dto.dWorkedHours;
            todayFullHours.Content = dto.dailyWorkHours;
            todayCalcHours.Content = dto.dCalcHours;
            todayLeftHours.Content = AddPlusIfNeeded(dto.dWorkedHours - dto.dailyWorkHours);

            monthlyWorkedHours.Content = dto.mWorkedHours;
            monthlyFullHours.Content = dto.mFullHours;
            monthlyCalcHours.Content = dto.mCalcHours;
            monthlyLeftHours.Content = AddPlusIfNeeded(dto.mWorkedHours - dto.mFullHours);
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

            // Daily
            public double dWorkedHours, dCalcHours;

            // Yesterday
            public double ywdWorkedHours, ywdCalcHours;
        }
    }
}
