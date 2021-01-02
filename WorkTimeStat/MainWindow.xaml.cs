using System;
using System.Threading;
using System.Windows;
using WorkTimeStat.Controls;
using WorkTimeStat.Models;
using WorkTimeStat.Services;
using WorkTimeStat.Storage;
using WorkTimeStat.Timers;
using WorkTimeStat.Windows;

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
            InitializeComponent();
            InitializeWorkKeeper();
        }

        private void InitializeWorkKeeper()
        {
            workKeeper = WorkKeeper.Instance;
            workKeeper.InitWatcher();

            saveTimer = new SaveTimer(TimeSpan.FromMinutes(30));

            StatisticsService.FullReCountWorkedDays();
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

        private void DayEdit_CloseBallon()
        {
            taskbarIcon.CloseBalloon();
        }
        #endregion

        #region Tray Click Events
        private void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            WorkDayEditControl dayEdit = new WorkDayEditControl(WorkDayService.GetCurrentDay());
            dayEdit.CloseBallon += DayEdit_CloseBallon;

            taskbarIcon.ShowCustomBalloon(dayEdit, System.Windows.Controls.Primitives.PopupAnimation.Fade, null);
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
                new StatisticsWindow(workKeeper.Settings.DailyWorkHours);
            statisticsWindow.Show();
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
