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

            // Get data
            // Monthly
            int thisMonth = DateTime.Now.Month;
            double mWorkedHours = StatisticsService.GetMonthlyWorkedHours(thisMonth);
            double mFullHours = StatisticsService.GetMonthlyTotalHours(thisMonth, dailyWorkHours);
            double mCalcHours = StatisticsService.GetUsageForMonth(thisMonth);

            // Daily
            WorkDay today = WorkDayService.GetCurrentDay();
            double dWorkedHours = StatisticsService.GetDailyWorkedHours(today);
            double dCalcHours = StatisticsService.GetUsageForToday();

            // Yesterday
            WorkDay yesterWorkday = WorkDayService.GetYesterWorkDay();
            double ywdWorkedHours = StatisticsService.GetDailyWorkedHours(yesterWorkday);
            double ywdCalcHours = StatisticsService.GetUsageForDay(ref yesterWorkday);

            // Assign to Labels
            yesterworkdayWorkedHours.Content = ywdWorkedHours;
            yesterworkdayFullHours.Content = dailyWorkHours;
            yesterworkdayCalcHours.Content = ywdCalcHours;
            yesterworkdayLeftHours.Content = AddPlusIfNeeded(ywdWorkedHours - dailyWorkHours);

            todayWorkedHours.Content = dWorkedHours;
            todayFullHours.Content = dailyWorkHours;
            todayCalcHours.Content = dCalcHours;
            todayLeftHours.Content = AddPlusIfNeeded(dWorkedHours - dailyWorkHours);

            monthlyWorkedHours.Content = mWorkedHours;
            monthlyFullHours.Content = mFullHours;
            monthlyCalcHours.Content = mCalcHours;
            monthlyLeftHours.Content = AddPlusIfNeeded(mWorkedHours - mFullHours);
        }

        private static string AddPlusIfNeeded(double num)
        {
            num = Math.Round(num, 2, MidpointRounding.ToEven);
            return num > 0 ? "+" + num : num.ToString(CultureInfo.InvariantCulture);
        }
    }
}
