using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Globalization;
using System.Windows;
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
            workKeeper.InitWatcher();

            saveTimer = new SaveTimer(TimeSpan.FromMinutes(30));

            StatisticsService.FullReCountWorkedDays();
        }

        private void SetLanguage()
        {
            AvailableLanguages chosenLang = workKeeper.Settings.ChosenLanguage;
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(chosenLang.GetDescription());
        }

        private void CheckSettingsSetup()
        {
            WorkSettings settings = WorkKeeper.Instance.Settings;
            if (settings.DailyWorkHours == default || settings.YearlyLeaveNumber == default)
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
            taskbarIcon.CloseBalloon();
        }
        #endregion

        #region Tray Click Events
        private void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            WorkDayEditControl dayEdit = new WorkDayEditControl(WorkDayService.GetCurrentDay());
            dayEdit.CloseBallon += CloseTaskbarBalloon;

            taskbarIcon.ShowCustomBalloon(dayEdit, System.Windows.Controls.Primitives.PopupAnimation.Fade, null);
        }
        #endregion

        #region Menu Click Events
        private void CalendarItem_Click(object sender, RoutedEventArgs e)
        {
            CalendarControl calendarControl = new CalendarControl();
            calendarControl.CloseBallon += CloseTaskbarBalloon;

            taskbarIcon.ShowCustomBalloon(calendarControl, System.Windows.Controls.Primitives.PopupAnimation.Fade, null);
        }

        private void SettingsItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsControl settingsControl = new SettingsControl();
            settingsControl.CloseBallon += CloseTaskbarBalloon;

            taskbarIcon.ShowCustomBalloon(settingsControl, System.Windows.Controls.Primitives.PopupAnimation.Fade, null);
        }

        private void StatisticsItem_Click(object sender, RoutedEventArgs e)
        {
            StatisticsControl statisticsControl = new StatisticsControl(workKeeper.Settings.DailyWorkHours);
            statisticsControl.CloseBallon += CloseTaskbarBalloon;

            taskbarIcon.ShowCustomBalloon(statisticsControl, System.Windows.Controls.Primitives.PopupAnimation.Fade, null);
        }

        private void UsageItem_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan usageSoFar = workKeeper.GetWatcher().UsageForGivenTimeframe(DateTime.Today, DateTime.Now);
            taskbarIcon.ShowBalloonTip("Eddigi használat", string.Format(CultureInfo.InvariantCulture, 
                "{0} óra : {1} perc", usageSoFar.Hours, usageSoFar.Minutes), BalloonIcon.Info);
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
