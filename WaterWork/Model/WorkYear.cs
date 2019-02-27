using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WaterWork.Model
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class WorkYear : INotifyPropertyChanged
    {
        public Dictionary<String, WorkDay> WorkDays { get; set; }
        public uint OfficalWorkDayCount { get; set; }
        public uint NoOfDaysWorked { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal WorkYear()
        {
            WorkDays = new Dictionary<String, WorkDay>();
        }

        internal WorkDay GetCurrentDay()
        {
            WorkDays.TryGetValue(GetTodayDate(), out WorkDay day);

            return day ?? new WorkDay();
        }

        internal void SetCurrentDay(WorkDay today)
        {
            WorkDays[GetTodayDate()] = today;
        }

        private String GetTodayDate()
        {
            return DateTime.Today.Date.ToString("yyyy-MM-dd");
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
