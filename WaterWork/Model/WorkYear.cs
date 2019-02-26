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

        internal void AddDay(String date)
        {
            if (!WorkDays.ContainsKey(date))
            {
                WorkDays.Add(GetTodayDate(), new WorkDay());
            }
        }

        internal void AddDay(String date, WorkDay day)
        {
            WorkDays[date] = day;
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

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private String GetTodayDate()
        {
            return DateTime.Today.Date.ToString("yyyy-MM-dd");
        }
    }
}
