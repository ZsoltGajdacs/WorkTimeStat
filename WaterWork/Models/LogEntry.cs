using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WaterWork.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class LogEntry : INotifyPropertyChanged
    {
        private static readonly string DATETIME_FORMAT = "MM-dd hh\\:mm";
        private static readonly string TIMESPAN_FORMAT = "hh\\:mm";

        public LogEntry(string logName)
        {
            LogName = logName ?? throw new ArgumentNullException(nameof(logName));
            StartDate = DateTime.Now;

            StartDateText = StartDate.ToString(DATETIME_FORMAT);
            TimeSpentText = TimeSpent.ToString(TIMESPAN_FORMAT);
        }

        public LogEntry()
        {
            // For serialization support
        }

        public string LogName { get; set; }
        public bool IsFinished { get; set; }
        public DateTime EndDate { get; set; }

        private DateTime _startDate;
        private TimeSpan _timeSpent;

        [JsonIgnore]
        internal string StartDateText { get; set; }
        [JsonIgnore]
        internal string TimeSpentText { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                StartDateText = StartDate.ToString(DATETIME_FORMAT);
            }
        }
        public TimeSpan TimeSpent
        {
            get
            {
                return _timeSpent;
            }
            set
            {
                _timeSpent = value;
                TimeSpentText = TimeSpent.ToString(TIMESPAN_FORMAT);
            }
        }

        public override string ToString()
        {
            string doneState = IsFinished ? "Befejezett" : "Befejezetlen";
            return LogName + ": " + TimeSpent.ToString() + "; " + doneState;
        }

        #region Event handler
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
