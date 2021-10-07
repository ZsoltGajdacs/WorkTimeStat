using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using UsageWatcher.Models;
using WorkTimeStat.Enums;
using WorkTimeStat.Events;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Services;

namespace WorkTimeStat.Controls
{
    public partial class StatisticsControl : UserControl
    {
        internal event CloseTheBallonEventHandler CloseBallon;
        public StatisticsControl(double dailyWorkHours)
        {
            InitializeComponent();

            OverviewTabData dto = CreateOverviewData(dailyWorkHours);
            AssignDataToWindowControls(ref dto);
            OverviewGrid.DataContext = this;

            UsageTabData usageTabData = CreateUsageTabData();
            UsageGrid.DataContext = usageTabData;
        }

        private static UsageTabData CreateUsageTabData()
        {
            return new UsageTabData();
        }

        private static OverviewTabData CreateOverviewData(double dailyWorkHours)
        {
            OverviewTabData dto = new OverviewTabData { dailyWorkHours = dailyWorkHours };

            List<WorkDayType> dayTypes = StatisticsService.GetOfficalWorkdayTypes();

            // Monthly
            int thisMonth = DateTime.Now.Month;
            dto.mWorkedHours = StatisticsService.CalcMonthlyWorkedHours(thisMonth, dayTypes);
            dto.mFullHours = StatisticsService.CalcMonthlyTotalHours(thisMonth);
            dto.mCalcHours = StatisticsService.GetUsageForMonth(thisMonth, dayTypes);
            dto.mLeftHours = AddPlusIfNeeded(StatisticsService.CalcMonthlyHoursDifference(thisMonth, dayTypes));

            // Last month
            int lastMonth = thisMonth - 1;
            dto.pmWorkedHours = StatisticsService.CalcMonthlyWorkedHours(lastMonth, dayTypes);
            dto.pmFullHours = StatisticsService.CalcMonthlyTotalHours(lastMonth);
            dto.pmCalcHours = StatisticsService.GetUsageForMonth(lastMonth, dayTypes);
            dto.pmLeftHours = AddPlusIfNeeded(StatisticsService.CalcMonthlyHoursDifference(lastMonth, dayTypes));

            // Daily
            WorkDay today = WorkDayService.GetCurrentDay();
            dto.dWorkedHours = StatisticsService.CalcDailyWorkedHours(today);
            dto.dFullHours = StatisticsService.CalcDailyTotalHours(today);
            dto.dCalcHours = StatisticsService.GetUsageForToday();
            dto.dLeftHours = AddPlusIfNeeded(StatisticsService.CalcDailyHoursDifference(today));

            // Yesterday
            WorkDay lastWorkday = WorkDayService.GetLastWorkDay();
            dto.ywdWorkedHours = StatisticsService.CalcDailyWorkedHours(lastWorkday);
            dto.ywdFullHours = StatisticsService.CalcDailyTotalHours(lastWorkday);
            dto.ywdCalcHours = StatisticsService.GetUsageForDay(lastWorkday);
            dto.ywdLeftHours = AddPlusIfNeeded(StatisticsService.CalcDailyHoursDifference(lastWorkday));

            return dto;
        }

        // TODO: rework this to happen in the dto and assign that as datasource
        private void AssignDataToWindowControls(ref OverviewTabData dto)
        {
            yesterworkdayWorkedHours.Content = NumberFormatter.FormatNum(dto.ywdWorkedHours);
            yesterworkdayFullHours.Content = NumberFormatter.FormatNum(dto.ywdFullHours);
            yesterworkdayCalcHours.Content = NumberFormatter.FormatNum(dto.ywdCalcHours);
            yesterworkdayLeftHours.Content = dto.ywdLeftHours;

            todayWorkedHours.Content = NumberFormatter.FormatNum(dto.dWorkedHours);
            todayFullHours.Content = NumberFormatter.FormatNum(dto.dFullHours);
            todayCalcHours.Content = NumberFormatter.FormatNum(dto.dCalcHours);
            todayLeftHours.Content = dto.dLeftHours;

            monthlyWorkedHours.Content = NumberFormatter.FormatNum(dto.mWorkedHours);
            monthlyFullHours.Content = NumberFormatter.FormatNum(dto.mFullHours);
            monthlyCalcHours.Content = NumberFormatter.FormatNum(dto.mCalcHours);
            monthlyLeftHours.Content = dto.mLeftHours;

            prevMonthlyWorkedHours.Content = NumberFormatter.FormatNum(dto.pmWorkedHours);
            prevMonthlyFullHours.Content = NumberFormatter.FormatNum(dto.pmFullHours);
            prevMonthlyCalcHours.Content = NumberFormatter.FormatNum(dto.pmCalcHours);
            prevMonthlyLeftHours.Content = dto.pmLeftHours;
        }

        private static string AddPlusIfNeeded(double num)
        {
            num = Math.Round(num, 2, MidpointRounding.ToEven);
            return num > 0 ? "+" + num : num.ToString(CultureInfo.CurrentCulture);
        }

        private void SaveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CloseBallon?.Invoke();
        }

        private class OverviewTabData
        {
            public double dailyWorkHours;

            // Monthly
            public double mWorkedHours, mFullHours, mCalcHours;
            public string mLeftHours;

            // Previous Month
            public double pmWorkedHours, pmFullHours, pmCalcHours;
            public string pmLeftHours;

            // Daily
            public double dWorkedHours, dFullHours, dCalcHours;
            public string dLeftHours;

            // Yesterday
            public double ywdWorkedHours, ywdFullHours, ywdCalcHours;
            public string ywdLeftHours;
        }
    
        private class UsageTabData
        {
            public string UsageFlowData { get; private set; }
            public string UsageBreakData { get; private set; }
            public string UsageFlowTotal { get; private set; }
            public string UsageBreakTotal { get; private set; }
            public List<DateTime> DatesWithUsageData { get; private set; }

            public UsageTabData()
            {
                List<UsageBlock> usageflows = StatisticsService.GetUsageFlowForDate(DateTime.Today);
                List<UsageBlock> usageBreaks = StatisticsService.GetUsageBreaksForDate(DateTime.Today);

                LocalizationHelper locHelper = LocalizationHelper.Instance;
                UsageFlowTotal = Math.Round(CalcUsageBlockTotals(usageflows, TimeSpan.FromMinutes(5)), MidpointRounding.ToEven)
                    .ToString(CultureInfo.CurrentCulture) + " " + locHelper.GetStringForKey("u_minute");
                UsageBreakTotal = Math.Round(CalcUsageBlockTotals(usageBreaks, TimeSpan.FromMinutes(10)), MidpointRounding.ToEven)
                    .ToString(CultureInfo.CurrentCulture) + " " + locHelper.GetStringForKey("u_minute");

                UsageFlowData = UsageBlocksToString(usageflows, TimeSpan.FromMinutes(5));
                UsageBreakData = UsageBlocksToString(usageBreaks, TimeSpan.FromMinutes(10));

                DatesWithUsageData = GetDatesWithUsageData();
            }

            private List<DateTime> GetDatesWithUsageData()
            {
                //TODO: Build list so the user can choose to view each day's usage data
                return new List<DateTime>();
            }

            private static double CalcUsageBlockTotals(List<UsageBlock> usages, TimeSpan minBlockLength)
            {
                TimeSpan total = TimeSpan.Zero;

                usages.ForEach((usage) => 
                {
                    if (usage.EndTime - usage.StartTime > minBlockLength)
                    {
                        total += usage.EndTime - usage.StartTime;
                    }
                });

                return total.TotalMinutes;
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

        }
    }
}
