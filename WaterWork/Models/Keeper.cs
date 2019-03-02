using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WaterWork.Services;

namespace WaterWork.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class Keeper : INotifyPropertyChanged
    {
        public Dictionary<int, WorkYear> WorkYears { get; set; }
        public Boolean IsLunchTimeWorkTimeDefault { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal Keeper()
        {
            WorkYears = new Dictionary<int, WorkYear>();
        }

        #region GETs
        internal WorkYear GetCurrentYear()
        {
            WorkYears.TryGetValue(GetCurrentYearNum(), out WorkYear thisYear);

            if (thisYear != null)
                return thisYear;
            else
            {
                WorkYear year = new WorkYear();
                SetCurrentYear(ref year);

                return year;
            }
        }

        internal WorkMonth GetCurrentMonth()
        {
            return GetCurrentYear().GetCurrentMonth();
        }

        internal WorkDay GetCurrentDay()
        {
            return GetCurrentMonth().GetCurrentDay(IsLunchTimeWorkTimeDefault);
        }
        #endregion

        #region SETs
        internal void SetCurrentYear(ref WorkYear thisYear)
        {
            WorkYears[GetCurrentYearNum()] = thisYear;
        }

        internal void SetCurrentMonth(ref WorkMonth thisMonth)
        {
            GetCurrentYear().SetCurrentMonth(ref thisMonth);
        }

        internal void SetCurrentDay(ref WorkDay today)
        {
            GetCurrentMonth().SetCurrentDay(ref today);
        }
        #endregion

        private int GetCurrentYearNum()
        {
            return StatisticsService.GetThisYearNum();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
