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

        #region Startup
        public MainWindow()
        {
            InitializeComponent();
            InitializeWorkKeeper();
        }
        
        private void InitializeWorkKeeper()
        {
            workKeeper = WorkKeeper.Instance;
            StatisticsService.FullReCountWorkedDays();
        }
        #endregion

        // TODO: App start and exit times must be saved, and checked against the daily start/end times
        // so that the app know if it was shut down during the day, and also whether it was a crash!
        #region Window Events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveService.SaveData(true);
        }

        private void DayEdit_Closed(object sender, EventArgs e)
        {
            SaveService.SaveData(false);
        }
        #endregion

        #region Tray Click Events
        private void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            WorkdayEdit dayEdit = new WorkdayEdit(workKeeper.Settings, WorkDayService.GetCurrentDay());
            dayEdit.Closed += DayEdit_Closed;
            WorkDay today = dayEdit.ShowDialog();

            WorkDayService.SetCurrentDay(ref today);
        }

        private void TaskbarIcon_TrayRightMouseUp(object sender, RoutedEventArgs e)
        {
            WorkDay today = WorkDayService.GetCurrentDay();
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
                new StatisticsWindow(workKeeper.Settings.IsLunchTimeWorkTimeDefault,
                                        workKeeper.Settings.DailyWorkHours);
            statisticsWindow.Show();
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

    }
}
