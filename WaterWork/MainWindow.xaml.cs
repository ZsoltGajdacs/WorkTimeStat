using System;
using System.Threading;
using System.Windows;
using WaterWork.Dialogs;
using WaterWork.Helpers;
using WaterWork.Models;
using WaterWork.Services;
using WaterWork.Storage;
using WaterWork.Windows;

namespace WaterWork
{
    public partial class MainWindow : Window
    {
        private WorkKeeper workKeeper;
        private UsageKeeper usageKeeper;

        public MainWindow()
        {
            InitializeComponent();
            InitializeWorkKeeper();
            InitializeUsageWatcher();
        }

        #region Startup
        private void InitializeWorkKeeper()
        {
            string filePath = FilesLocation.GetSaveDirPath() + FilesLocation.GetWaterWorkFileName();
            workKeeper = Serializer.JsonObjectDeserialize<WorkKeeper>(filePath);

            if (workKeeper == null)
                workKeeper = new WorkKeeper();
            else
                workKeeper.GetCurrentYear().CountWorkedDays();
        }

        private void InitializeUsageWatcher()
        {
            usageKeeper = UsageKeeper.Instance;
        }
        #endregion

        // TODO: App start and exit times must be saved, and checked against the daily start/end times
        // so that the app know if it was shut down during the day, and also whether it was a crash!
        #region Window Events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Usage needs to be saved here, becuase this is the only place
            // where I can be certain that I have the complete time
            usageKeeper.AddUsage(DateTime.Now.Date, GetTodaysUsageForSave());
            
            // Serialization stuff
            string saveDirPath = FilesLocation.GetSaveDirPath();
            string waterWorkFileName = FilesLocation.GetWaterWorkFileName();
            string usageFileName = FilesLocation.GetUsageLogName();

            Serializer.JsonObjectSerialize<WorkKeeper>(saveDirPath + waterWorkFileName, ref workKeeper);
            Serializer.JsonObjectSerialize<UsageKeeper>(saveDirPath + usageFileName, ref usageKeeper);
        }
        #endregion

        #region Tray Click Events
        private void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            WorkdayEdit dayEdit = new WorkdayEdit(workKeeper.GetCurrentDay());
            WorkDay today = dayEdit.ShowDialog();

            workKeeper.SetCurrentDay(ref today);
        }

        private void TaskbarIcon_TrayRightMouseUp(object sender, RoutedEventArgs e)
        {
            WorkDay today = workKeeper.GetCurrentDay();
            today.IncreaseWaterConsumption();

            decimal waterAmount = today.WaterConsumptionCount * today.AmountOfLitreInOneUnit;

            taskbarIcon.ShowBalloonTip("Vízfogyasztás", "Már " + waterAmount + "l vizet ittál ma!",
                                            Hardcodet.Wpf.TaskbarNotification.BalloonIcon.None);
            Thread.Sleep(3000);
            taskbarIcon.HideBalloonTip();
        }
        #endregion

        #region Menu Click Events
        private void CalendarItem_Click(object sender, RoutedEventArgs e)
        {
            CalendarWindow calendarWindow = new CalendarWindow(ref workKeeper);
            calendarWindow.Show();
        }

        private void SettingsItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(ref workKeeper);
            settingsWindow.Show();
        }

        private void StatisticsItem_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWindow statisticsWindow =
                new StatisticsWindow(workKeeper.GetCurrentYear(),
                                        workKeeper.IsLunchTimeWorkTimeDefault,
                                        workKeeper.DailyWorkHours);
            statisticsWindow.Show();
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region Helpers
        private TimeSpan GetTodaysUsageForSave()
        {
            WorkDay today = workKeeper.GetCurrentDay();
            DateTime start = today.DayDate.Date + today.StartTime;
            DateTime end = today.DayDate.Date + today.EndTime;

            return usageKeeper.GetUsageForTimeframe(start, end, true);
        }
        #endregion

    }
}
