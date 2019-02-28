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
    internal class WorkMonth : INotifyPropertyChanged
    {
        public Dictionary<int, WorkDay> WorkDays { get; set; }
        public int OfficalWorkDayCount { get; set; }
        public int NoOfDaysWorked { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public WorkMonth()
        {
            WorkDays = new Dictionary<int, WorkDay>();

            PropertyChanged += WorkMonth_PropertyChanged;
        }

        private void WorkMonth_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CountWorkedDays();
        }

        internal WorkDay GetCurrentDay()
        {
            WorkDays.TryGetValue(GetTodayNum(), out WorkDay day);

            return day ?? new WorkDay();
        }

        internal void SetCurrentDay(ref WorkDay today)
        {
            WorkDays[GetTodayNum()] = today;
            NotifyPropertyChanged();
        }

        private void CountWorkedDays()
        {
            NoOfDaysWorked = WorkDays.Count;
        }

        private int GetTodayNum()
        {
            return int.Parse(DateTime.Today.Date.ToString("dd"));
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
