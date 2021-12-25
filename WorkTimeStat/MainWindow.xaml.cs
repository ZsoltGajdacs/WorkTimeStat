using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using UsageWatcher.Enums;
using WorkTimeStat.Controls;
using WorkTimeStat.Enums;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Services;
using WorkTimeStat.Storage;
using WorkTimeStat.Timers;

namespace WorkTimeStat
{
    public partial class MainWindow : Window, IDisposable
    {
        private WorkKeeper workKeeper;
        private SaveTimer saveTimer;
        private bool disposedValue;

        #region Startup
        public MainWindow()
        {
            InitializeWorkKeeper();
            SetLanguage();
            InitializeComponent();
            CheckSettingsSetup();
        }

        private void InitializeWorkKeeper()
        {
            workKeeper = WorkKeeper.Instance;
            workKeeper.Init();

            saveTimer = new SaveTimer(TimeSpan.FromMinutes(30));

            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            StatisticsService.FullReCountWorkedDays();
        }

        private void SetLanguage()
        {
            AvailableLanguages chosenLang = workKeeper.Settings.ChosenLanguage;
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(chosenLang.GetDescription());
        }

        private void CheckSettingsSetup()
        {
            WorkSettings settings = WorkKeeper.Instance.Settings;
            if (settings.DailyWorkHours == default || settings.YearlyLeaveNumber == default || settings.HolidayYearStart == default)
            {
                LocalizationHelper locHelp = LocalizationHelper.Instance;
                MessageBox.Show(locHelp.GetStringForKey("main_error_no_settings_content"),
                       locHelp.GetStringForKey("main_error_no_settings_header"),
                       MessageBoxButton.OK, MessageBoxImage.Error);

                SettingsItem_Click(null, null);
            }
        }
        #endregion

        #region Window Events
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "There is a crash when it tries to save at shutdown")]
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                SaveService.SaveData(SaveUsage.Yes);
            }
            catch (Exception)
            {
                // Log if something fails
            }

        }

        private void CloseTaskbarBalloon()
        {
            TaskbarIcon.CloseBalloon();
        }
        #endregion

        #region Tray Click Events
        private void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            WorkDayEditControl dayEdit = new WorkDayEditControl(WorkDayService.GetCurrentDay());
            dayEdit.CloseBallon += CloseTaskbarBalloon;

            TaskbarIcon.ShowCustomBalloon(dayEdit, PopupAnimation.Fade, null);
        }
        #endregion

        #region Menu Click Events
        private void TicketItem_Click(object sender, RoutedEventArgs e)
        {
            TicketTimeControl ticketTimeControl = new TicketTimeControl();
            ticketTimeControl.CloseBallon += CloseTaskbarBalloon;

            TaskbarIcon.ShowCustomBalloon(ticketTimeControl, PopupAnimation.Fade, null);
            WindowFocusHelper.ActivatePopup(TaskbarIcon.CustomBalloon);
        }

        private void CalendarItem_Click(object sender, RoutedEventArgs e)
        {
            CalendarControl calendarControl = new CalendarControl();
            calendarControl.CloseBallon += CloseTaskbarBalloon;

            TaskbarIcon.ShowCustomBalloon(calendarControl, PopupAnimation.Fade, null);
        }

        private void SettingsItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsControl settingsControl = new SettingsControl();
            settingsControl.CloseBallon += CloseTaskbarBalloon;

            TaskbarIcon.ShowCustomBalloon(settingsControl, PopupAnimation.Fade, null);
        }

        private void StatisticsItem_Click(object sender, RoutedEventArgs e)
        {
            StatisticsControl statisticsControl = new StatisticsControl();
            statisticsControl.CloseBallon += CloseTaskbarBalloon;

            TaskbarIcon.ShowCustomBalloon(statisticsControl, PopupAnimation.Fade, null);
        }

        private void UsageItem_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan usageSoFar = workKeeper.GetWatcher().UsageTimeForGivenTimeframe(DateTime.Today, DateTime.Now);
            TaskbarIcon.ShowBalloonTip("Eddigi használat", string.Format(CultureInfo.CurrentCulture, 
                "{0} óra : {1} perc", usageSoFar.Hours, usageSoFar.Minutes), BalloonIcon.Info);
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region System Events
        /// <summary>
        /// Happens when the user lockes the workstation, determines the timers to restart based on 
        /// lock time
        /// </summary>
        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                TaskService.PauseActiveTask();
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                TaskService.RestartActiveTask();
            }
        }
        #endregion

        #region Disposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    saveTimer.Dispose();
                }

                //  free unmanaged resources (unmanaged objects) and override finalizer
                //  set large fields to null
                workKeeper.Dispose();
                disposedValue = true;
            }
        }

        ~MainWindow()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
