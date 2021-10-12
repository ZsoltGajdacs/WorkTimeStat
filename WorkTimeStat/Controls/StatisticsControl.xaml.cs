using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Linq;
using UsageWatcher.Models;
using WorkTimeStat.Enums;
using WorkTimeStat.Events;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Services;
using ZsGUtils.UIHelpers;

namespace WorkTimeStat.Controls
{
    public partial class StatisticsControl : UserControl
    {
        internal event CloseTheBallonEventHandler CloseBallon;

        private readonly UsageTabData usageTabDataVM;
        private readonly OverviewTabData overviewTabDataVM;

        public StatisticsControl()
        {
            InitializeComponent();

            overviewTabDataVM = CreateOverviewTabData();
            OverviewGrid.DataContext = overviewTabDataVM;

            usageTabDataVM = CreateUsageTabData();
            UsageGrid.DataContext = usageTabDataVM;
        }

        private static UsageTabData CreateUsageTabData()
        {
            return new UsageTabData();
        }

        private static OverviewTabData CreateOverviewTabData()
        {
            return new OverviewTabData();
        }

        private void SaveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CloseBallon?.Invoke();
        }

        private void UsageDateCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime) e.AddedItems[0];
            usageTabDataVM.LoadDataForDay(selectedDate);
        }

        #region Overview Tab
        private class OverviewTabData
        {
            // Current month
            public string MWorkedHours { get; set; }
            public string MFullHours { get; set; }
            public string MCalcHours { get; set; }
            public string MLeftHours { get; set; }

            // Previous month
            public string PmWorkedHours { get; set; }
            public string PmFullHours { get; set; }
            public string PmCalcHours { get; set; }
            public string PmLeftHours { get; set; }

            // Today
            public string DWorkedHours { get; set; }
            public string DFullHours { get; set; }
            public string DCalcHours { get; set; }
            public string DLeftHours { get; set; }

            // Yesterday
            public string YwdWorkedHours { get; set; }
            public string YwdFullHours { get; set; }
            public string YwdCalcHours { get; set; }
            public string YwdLeftHours { get; set; }

            public OverviewTabData()
            {
                LoadStatisticsData();
            }

            private void LoadStatisticsData()
            {
                List<WorkDayType> dayTypes = StatisticsService.GetOfficalWorkdayTypes();

                // Monthly
                int thisMonth = DateTime.Now.Month;
                MWorkedHours = NumberFormatter.FormatNum(StatisticsService.CalcMonthlyWorkedHours(thisMonth, dayTypes));
                MFullHours = NumberFormatter.FormatNum(StatisticsService.CalcMonthlyTotalHours(thisMonth));
                MCalcHours = NumberFormatter.FormatNum(StatisticsService.GetUsageForMonth(thisMonth, dayTypes));
                MLeftHours = AddPlusIfNeeded(StatisticsService.CalcMonthlyHoursDifference(thisMonth, dayTypes));

                // Last month
                int lastMonth = thisMonth - 1;
                PmWorkedHours = NumberFormatter.FormatNum(StatisticsService.CalcMonthlyWorkedHours(lastMonth, dayTypes));
                PmFullHours = NumberFormatter.FormatNum(StatisticsService.CalcMonthlyTotalHours(lastMonth));
                PmCalcHours = NumberFormatter.FormatNum(StatisticsService.GetUsageForMonth(lastMonth, dayTypes));
                PmLeftHours = AddPlusIfNeeded(StatisticsService.CalcMonthlyHoursDifference(lastMonth, dayTypes));

                // Daily
                WorkDay today = WorkDayService.GetCurrentDay();
                DWorkedHours = NumberFormatter.FormatNum(StatisticsService.CalcDailyWorkedHours(today));
                DFullHours = NumberFormatter.FormatNum(StatisticsService.CalcDailyTotalHours(today));
                DCalcHours = NumberFormatter.FormatNum(StatisticsService.GetUsageForToday());
                DLeftHours = AddPlusIfNeeded(StatisticsService.CalcDailyHoursDifference(today));

                // Yesterday
                WorkDay lastWorkday = WorkDayService.GetLastWorkDay();
                YwdWorkedHours = NumberFormatter.FormatNum(StatisticsService.CalcDailyWorkedHours(lastWorkday));
                YwdFullHours = NumberFormatter.FormatNum(StatisticsService.CalcDailyTotalHours(lastWorkday));
                YwdCalcHours = NumberFormatter.FormatNum(StatisticsService.GetUsageForDay(lastWorkday));
                YwdLeftHours = AddPlusIfNeeded(StatisticsService.CalcDailyHoursDifference(lastWorkday));
            }

            private static string AddPlusIfNeeded(double num)
            {
                num = Math.Round(num, 2, MidpointRounding.ToEven);
                return num > 0 ? "+" + num : num.ToString(CultureInfo.CurrentCulture);
            }
        }
        #endregion

        #region Usage Tab
        private class UsageTabData : BindableClass
        {
            private string usageFlowData;
            private string usageBreakData;
            private string usageFlowTotal;
            private string usageBreakTotal;

            public string UsageFlowData { get => usageFlowData; set => SetAndNotifyPropertyChanged(ref usageFlowData, value); }
            public string UsageBreakData { get => usageBreakData; set => SetAndNotifyPropertyChanged(ref usageBreakData, value); }
            public string UsageFlowTotal { get => usageFlowTotal; set => SetAndNotifyPropertyChanged(ref usageFlowTotal, value); }
            public string UsageBreakTotal { get => usageBreakTotal; set => SetAndNotifyPropertyChanged(ref usageBreakTotal, value); }
            public List<DateTime> DatesWithUsageData { get; set; }

            public UsageTabData()
            {
                Init();
                LoadDataForDay(DateTime.Today);
                DatesWithUsageData = GetDatesWithUsageData();
            }

            private void Init()
            {
                usageFlowData = "";
                usageBreakData = string.Empty;
                usageFlowTotal = string.Empty;
                usageBreakTotal = string.Empty;
            }

            public void LoadDataForDay(DateTime day)
            {
                List<UsageBlock> usageflows = StatisticsService.GetUsageFlowForDate(day);
                List<UsageBlock> usageBreaks = StatisticsService.GetUsageBreaksForDate(day);

                LocalizationHelper locHelper = LocalizationHelper.Instance;
                UsageFlowTotal = Math.Round(CalcUsageBlockTotals(usageflows, TimeSpan.FromMinutes(5)), MidpointRounding.ToEven)
                    .ToString(CultureInfo.CurrentCulture) + " " + locHelper.GetStringForKey("u_minute");
                UsageBreakTotal = Math.Round(CalcUsageBlockTotals(usageBreaks, TimeSpan.FromMinutes(10)), MidpointRounding.ToEven)
                    .ToString(CultureInfo.CurrentCulture) + " " + locHelper.GetStringForKey("u_minute");

                UsageFlowData = UsageBlocksToString(usageflows, TimeSpan.FromMinutes(5));
                UsageBreakData = UsageBlocksToString(usageBreaks, TimeSpan.FromMinutes(10));
            }

            private static List<DateTime> GetDatesWithUsageData()
            {
                return StatisticsService.GetDatesWithUsageData().OrderByDescending(u => u.Date).ToList();
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
        #endregion
    }
}
