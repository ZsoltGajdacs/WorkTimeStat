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
        private readonly TaskTabData tasksTabDataVM;

        public StatisticsControl()
        {
            InitializeComponent();

            overviewTabDataVM = CreateOverviewTabData();
            OverviewGrid.DataContext = overviewTabDataVM;

            usageTabDataVM = CreateUsageTabData();
            UsageGrid.DataContext = usageTabDataVM;

            tasksTabDataVM = CreateTasksTabData();
            TasksGrid.DataContext = tasksTabDataVM;
        }

        #region Init
        private static UsageTabData CreateUsageTabData()
        {
            return new UsageTabData();
        }

        private static OverviewTabData CreateOverviewTabData()
        {
            return new OverviewTabData();
        }
        
        private static TaskTabData CreateTasksTabData()
        {
            return new TaskTabData();
        }
        #endregion

        #region Event handlers
        private void SaveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CloseBallon?.Invoke();
        }

        private void UsageDateCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)e.AddedItems[0];
            usageTabDataVM.LoadDataForDay(selectedDate);
        }

        private void TasksDateCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)e.AddedItems[0];
            tasksTabDataVM.LoadDataForDay(selectedDate);
        }
        #endregion

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
                List<WorkDayType> dayTypes = new List<WorkDayType>(StatisticsService.OFFICIAL_WORK_DAYS);
                List<WorkDayType> monthlyDiffDayTypes = new List<WorkDayType>(dayTypes);
                monthlyDiffDayTypes.Add(WorkDayType.OVERWORK_DAY);
                
                // Monthly
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;
                MWorkedHours = NumberFormatter.FormatNum(StatisticsService.CalcMonthlyWorkedHours(thisYear, thisMonth, dayTypes));
                MFullHours = NumberFormatter.FormatNum(StatisticsService.CalcMonthlyTotalHours(thisYear, thisMonth));
                MCalcHours = NumberFormatter.FormatNum(StatisticsService.GetUsageForMonth(thisYear, thisMonth, dayTypes));
                MLeftHours = AddPlusIfNeeded(StatisticsService.CalcMonthlyHoursDifference(thisYear, thisMonth, monthlyDiffDayTypes));

                // Last month
                int lastMonth = thisMonth - 1;
                int lastYear = thisYear - 1;
                PmWorkedHours = NumberFormatter.FormatNum(StatisticsService.CalcMonthlyWorkedHours(lastYear, lastMonth, dayTypes));
                PmFullHours = NumberFormatter.FormatNum(StatisticsService.CalcMonthlyTotalHours(lastYear, lastMonth));
                PmCalcHours = NumberFormatter.FormatNum(StatisticsService.GetUsageForMonth(lastYear, lastMonth, dayTypes));
                PmLeftHours = AddPlusIfNeeded(StatisticsService.CalcMonthlyHoursDifference(lastYear, lastMonth, monthlyDiffDayTypes));

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
                usageFlowData = string.Empty;
                usageBreakData = string.Empty;
                usageFlowTotal = string.Empty;
                usageBreakTotal = string.Empty;
            }

            public void LoadDataForDay(DateTime day)
            {
                List<UsageBlock> usageflows = StatisticsService.GetUsageFlowForDate(day, TimeSpan.FromMinutes(5));
                List<UsageBlock> usageBreaks = StatisticsService.GetUsageBreaksForDate(day, TimeSpan.FromMinutes(10));

                LocalizationHelper locHelper = LocalizationHelper.Instance;
                UsageFlowTotal = string.Format(CultureInfo.CurrentCulture, "{0} {1}",
                                        StatisticsService.CalcUsageBlockTotals(ref usageflows), locHelper.GetStringForKey("u_minute"));
                UsageBreakTotal = string.Format(CultureInfo.CurrentCulture, "{0} {1}",
                                        StatisticsService.CalcUsageBlockTotals(ref usageBreaks), locHelper.GetStringForKey("u_minute"));

                UsageFlowData = UsageBlocksToString(usageflows, TimeSpan.FromMinutes(5));
                UsageBreakData = UsageBlocksToString(usageBreaks, TimeSpan.FromMinutes(10));
            }

            private static List<DateTime> GetDatesWithUsageData()
            {
                return StatisticsService.GetDatesWithUsageData().OrderByDescending(u => u.Date).ToList();
            }

            private static string UsageBlocksToString(List<UsageBlock> blocks, TimeSpan minBlockLength)
            {
                LocalizationHelper locHelper = LocalizationHelper.Instance;
                StringBuilder sb = new StringBuilder();
                foreach (UsageBlock block in blocks)
                {
                    if ((block.EndTime - block.StartTime) > minBlockLength)
                    {
                        string start = block.StartTime.ToShortTimeString();
                        string end = block.EndTime.ToShortTimeString();
                        string length = Math.Round((block.EndTime - block.StartTime).TotalMinutes, MidpointRounding.ToEven)
                                                        .ToString(CultureInfo.CurrentCulture);
                        string minute = locHelper.GetStringForKey("u_minute");

                        sb.AppendFormat(CultureInfo.CurrentCulture, "{0} - {1}: {2} {3}", start, end, length, minute);
                        sb.AppendLine();
                    }
                }

                return sb.ToString();
            }

        }
        #endregion

        #region Tasks Tab
        private class TaskTabData : BindableClass
        {
            private string tasksData;
            private string tasksTotal;

            public string TasksData { get => tasksData; set => SetAndNotifyPropertyChanged(ref tasksData, value); }
            public string TasksTotal { get => tasksTotal; set => SetAndNotifyPropertyChanged(ref tasksTotal, value); }

            public List<DateTime> DatesWithTasksData { get; set; }

            public TaskTabData()
            {
                Init();
                LoadDataForDay(DateTime.Today);
                DatesWithTasksData = GetDatesWithTasks();
            }

            private void Init()
            {
                tasksData = string.Empty;
                tasksTotal = string.Empty;
            }

            private static List<DateTime> GetDatesWithTasks()
            {
                return TaskService.GetDatesWithTasksRecorded().OrderByDescending(u => u.Date).ToList();
            }

            public void LoadDataForDay(DateTime day)
            {
                List<MeasuredTask> tasks = TaskService.GetDaysTasks(day);

                LocalizationHelper locHelper = LocalizationHelper.Instance;
                TasksTotal = string.Format(CultureInfo.CurrentCulture, "{0} {1}",
                                        StatisticsService.CalcTaskUsagesForDay(day), locHelper.GetStringForKey("u_minute"));

                TasksData = TasksToString(ref tasks);
            }

            private static string TasksToString(ref List<MeasuredTask> tasks)
            {
                LocalizationHelper locHelper = LocalizationHelper.Instance;
                StringBuilder sb = new StringBuilder();

                if (tasks != null)
                {
                    tasks.ForEach(task =>
                    {
                        string name = task.TaskName;
                        string start = task.GetStartTime().ToShortTimeString();
                        string end = task.GetEndTime().ToShortTimeString();
                        string measuredTime =
                            StatisticsService.GetUsageForDateTimeFrame(task.GetStartTime(), task.GetEndTime(), TimeSpan.FromMinutes(5))
                                                                    .ToString(CultureInfo.CurrentCulture);
                        string overallTime = Rounder.RoundToClosestTime(task.GetOverallSpentTime(), TimeSpan.FromMinutes(5))
                                                                    .TotalMinutes
                                                                    .ToString(CultureInfo.CurrentCulture);
                        string minute = locHelper.GetStringForKey("u_minute");

                        sb.AppendFormat(CultureInfo.CurrentCulture, "{0}: {1} - {2}; {3}{4}/{5}{6}", name, start, end, measuredTime, 
                                                                                                     minute, overallTime, minute);
                        sb.AppendLine();
                    });
                } 

                return sb.ToString();
            }
        }
        #endregion
    }
}
