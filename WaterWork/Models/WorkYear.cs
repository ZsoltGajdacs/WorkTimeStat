using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WaterWork.Services;

namespace WaterWork.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class WorkYear : INotifyPropertyChanged
    {
        public Dictionary<int, WorkMonth> WorkMonths { get; set; }
        public int NoOfDaysWorked { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal WorkYear()
        {
            WorkMonths = new Dictionary<int, WorkMonth>();

            PropertyChanged += WorkYear_PropertyChanged;
        }

        internal WorkMonth GetCurrentMonth()
        {
            WorkMonths.TryGetValue(GetThisMonthNum(), out WorkMonth thisMonth);

            if (thisMonth != null)
                return thisMonth;
            else
            {
                WorkMonth month = new WorkMonth();
                month.PropertyChanged += Month_PropertyChanged;
                SetCurrentMonth(ref month);

                return month;
            }
        }

        internal void SetCurrentMonth(ref WorkMonth thisMonth)
        {
            WorkMonths[GetThisMonthNum()] = thisMonth;
            NotifyPropertyChanged();
        }

        internal void CountWorkedDays()
        {
            int count = 0;
            foreach (WorkMonth workMonth in WorkMonths.Values)
            {
                count += workMonth.NoOfDaysWorked;
            }

            NoOfDaysWorked = count;
        }

        internal WorkMonth GetMonth(int month)
        {
            WorkMonths.TryGetValue(month, out WorkMonth monthResult);
            return monthResult;
        }

        private int GetThisMonthNum()
        {
            return StatisticsService.GetThisMonthNum();
        }

        #region Event Handling
        private void WorkYear_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CountWorkedDays();
        }

        private void Month_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CountWorkedDays();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
