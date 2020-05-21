using System;
using System.Windows;
using WaterWork.Models;
using WaterWork.Services;

namespace WaterWork.Windows
{
    public partial class StatisticsWindow : Window
    {

        internal StatisticsWindow(WorkYear thisYear, bool isLunchTimeWorkTime, double dailyWorkHours)
        {
            InitializeComponent();
            mainGrid.DataContext = this;

            // Get data
            // Yearly
            double yWorkedHours = StatisticsService.GetYearlyWorkedHours(ref thisYear);
            double yFullHours = StatisticsService.GetYearlyTotalHours(ref thisYear, dailyWorkHours);
            //double yCalcHours = StatisticsService.GetUsageForYear(ref thisYear);

            // Monthly
            WorkMonth thisMonth = thisYear.GetCurrentMonth();
            double mWorkedHours = StatisticsService.GetMonthlyWorkedHours(thisMonth);
            double mFullHours = StatisticsService.GetMonthlyTotalHours(thisMonth, dailyWorkHours);
            //double mCalcHours = StatisticsService.GetUsageForMonth(thisMonth);

            // Daily
            WorkDay today = thisYear.GetCurrentMonth().GetCurrentDay(isLunchTimeWorkTime);
            double dWorkedHours = StatisticsService.GetDailyWorkedHours(today);
            double dCalcHours = StatisticsService.GetUsageForDay(today);

            // Yesterday
            WorkDay yesterWorkday = thisYear.GetCurrentMonth().GetYesterWorkDay();
            double ywdWorkedHours = StatisticsService.GetDailyWorkedHours(yesterWorkday);
            double ywdCalcHours = StatisticsService.GetUsageForDay(yesterWorkday);

            // Assign to Labels
            yesterworkdayWorkedHours.Content = ywdWorkedHours;
            yesterworkdayFullHours.Content = dailyWorkHours;
            yesterworkdayCalcHours.Content = ywdCalcHours;
            yesterworkdayLeftHours.Content = AddPlusIfNeeded(ywdWorkedHours - dailyWorkHours);

            todayWorkedHours.Content = dWorkedHours;
            todayFullHours.Content = dailyWorkHours;
            todayCalcHours.Content = dCalcHours;
            todayLeftHours.Content = AddPlusIfNeeded(dWorkedHours - dailyWorkHours);

            yearlyWorkedHours.Content = yWorkedHours;
            yearlyFullHours.Content = yFullHours;
            yearlyLeftHours.Content = AddPlusIfNeeded(yWorkedHours - yFullHours);

            monthlyWorkedHours.Content = mWorkedHours;
            monthlyFullHours.Content = mFullHours;
            monthlyLeftHours.Content = AddPlusIfNeeded(mWorkedHours - mFullHours);
        }

        private string AddPlusIfNeeded(double num)
        {
            num = Math.Round(num, 2, MidpointRounding.ToEven);
            return num > 0 ? "+" + num : num.ToString();
        }
    }
}
