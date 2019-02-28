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
    internal class WorkYear : INotifyPropertyChanged
    {
        public Dictionary<int, WorkMonth> WorkMonths { get; set; }
        public int OfficalWorkDayCount { get; set; }
        public int NoOfDaysWorked { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal WorkYear()
        {
            WorkMonths = new Dictionary<int, WorkMonth>();

            PropertyChanged += WorkYear_PropertyChanged;
        }

        private void WorkYear_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CountWorkedDays();
        }

        internal WorkMonth GetCurrentMonth()
        {
            WorkMonths.TryGetValue(GetThisMonthNum(), out WorkMonth thisMonth);

            return thisMonth ?? new WorkMonth();
        }

        internal void SetCurrentMonth(ref WorkMonth thisMonth)
        {
            WorkMonths[GetThisMonthNum()] = thisMonth;
            NotifyPropertyChanged();
        }

        private void CountWorkedDays()
        {
            int count = 0;
            foreach (WorkMonth workMonth in WorkMonths.Values)
            {
                count += workMonth.NoOfDaysWorked;
            }

            NoOfDaysWorked = count;
        }

        private int GetThisMonthNum()
        {
            return int.Parse(DateTime.Today.Date.ToString("MM"));
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
