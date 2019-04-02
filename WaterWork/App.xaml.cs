using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WaterWork.Models;

namespace WaterWork
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        LogKeeper logKeeper;

        internal App()
        {
            logKeeper = LogKeeper.Instance;
        }

        private void Done_MouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LogEntry logEntry = FindLogEntry(sender);

            if (logEntry != null)
            {
                logKeeper.ActiveWorkLogs.Remove(logEntry);
                logKeeper.WorkLogs[logEntry.LogName].IsFinished = true;
            }
        }

        private void Start_MouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LogEntry logEntry = FindLogEntry(sender);

            if (logEntry != null)
            {
                logEntry.IsPaused = false;
                logKeeper.WorkLogs[logEntry.LogName].IsPaused = false;
            }
        }

        private void Pause_MouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LogEntry logEntry = FindLogEntry(sender);

            if (logEntry != null)
            {
                logEntry.IsPaused = true;
                logKeeper.WorkLogs[logEntry.LogName].IsPaused = true;
            }
        }

        private LogEntry FindLogEntry(object sender)
        {
            if (sender is Image image)
            {
                string logName = image.Tag as string;
                LogEntry logEntry = logKeeper.ActiveWorkLogs.Where(a => a.LogName == logName).SingleOrDefault();

                return logEntry;
            }

            return null;
        }
    }
}
