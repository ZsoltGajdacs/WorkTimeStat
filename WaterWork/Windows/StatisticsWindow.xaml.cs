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
            double yWorkedHours = Math.Round(StatisticsService.GetYearlyWorkedHours(ref thisYear), 
                                            2, MidpointRounding.ToEven);
            double yFullHours = StatisticsService.GetYearlyTotalHours(ref thisYear, dailyWorkHours);

            // Monthly
            double mWorkedHours = Math.Round(StatisticsService.GetMonthlyWorkedHours(thisYear.GetCurrentMonth()), 
                                            2, MidpointRounding.ToEven);
            double mFullHours = StatisticsService.GetMonthlyTotalHours(thisYear.GetCurrentMonth(), 
                                                                        dailyWorkHours);

            // Daily
            double dWorkedHours = StatisticsService.GetDailyWorkedHours(
                                            thisYear.GetCurrentMonth().GetCurrentDay(isLunchTimeWorkTime));
            double dFullHours = (double)dailyWorkHours;
            double dCalcHours = Math.Round(StatisticsService.GetWatchedUsage().TotalHours, 
                                            2, MidpointRounding.ToEven);

            // Yesterday
            double ywdWorkedHours = StatisticsService.GetDailyWorkedHours(thisYear.GetCurrentMonth().GetYesterWorkDay());

            // Assign to Labels
            yesterworkdayWorkedHours.Content = ywdWorkedHours;
            yesterworkdayFullHours.Content = dFullHours;
            yesterworkdayLeftHours.Content = AddPlusIfNeeded(ywdWorkedHours - dFullHours);

            todayWorkedHours.Content = dWorkedHours;
            todayFullHours.Content = dFullHours;
            todayCalcHours.Content = dCalcHours;
            todayLeftHours.Content = AddPlusIfNeeded(dWorkedHours - dFullHours);

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
