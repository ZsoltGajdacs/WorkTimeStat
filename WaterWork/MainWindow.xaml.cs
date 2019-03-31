using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using WaterWork.Dialogs;
using WaterWork.Helpers;
using WaterWork.Models;
using WaterWork.Windows;

namespace WaterWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WorkKeeper workKeeper;
        private LogKeeper logKeeper;

        private Serializer serializer;

        private readonly string saveDirPath;
        private readonly string waterWorkFileName;
        private readonly string workLogFileName;

        public MainWindow()
        {
            InitializeComponent();

            serializer = new Serializer();

            FileInfo file = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            saveDirPath = file.DirectoryName;

            waterWorkFileName = "\\waterwork" + DateTime.Now.Year.ToString() + ".json";
            workLogFileName = "\\worklog" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";

            InitializeWorkKeeper();
            InitializeLogKeeper();
        }

        #region Startup
        private void InitializeWorkKeeper()
        {
            workKeeper = serializer.JsonObjectDeserialize<WorkKeeper>(saveDirPath + waterWorkFileName);

            if (workKeeper == null)
                workKeeper = new WorkKeeper();
            else
                workKeeper.GetCurrentYear().CountWorkedDays();
        }

        private void InitializeLogKeeper()
        {
            logKeeper = serializer.JsonObjectDeserialize<LogKeeper>(saveDirPath + workLogFileName);

            if (logKeeper == null)
                logKeeper = new LogKeeper();
        }

        #endregion

        #region Window Events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            serializer.JsonObjectSerialize<WorkKeeper>(saveDirPath + waterWorkFileName, ref workKeeper);
            serializer.JsonObjectSerialize<LogKeeper>(saveDirPath + workLogFileName, ref logKeeper);
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
        private void WorkLogItem_Click(object sender, RoutedEventArgs e)
        {
            WorkLogWindow workLogWindow = new WorkLogWindow(ref logKeeper);
            workLogWindow.Show();
        }

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
            StatisticsWindow statisticsWindow = new StatisticsWindow(workKeeper.GetCurrentYear(), workKeeper.IsLunchTimeWorkTimeDefault);
            statisticsWindow.Show();
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

    }
}
