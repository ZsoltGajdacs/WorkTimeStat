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
    internal class WorkMonth : INotifyPropertyChanged
    {
        public Dictionary<int, WorkDay> WorkDays { get; set; }
        public int NoOfDaysWorked { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public WorkMonth()
        {
            WorkDays = new Dictionary<int, WorkDay>();

            PropertyChanged += WorkMonth_PropertyChanged;
        }

        internal WorkDay GetCurrentDay(bool isLunchTimeWorkTime)
        {
            WorkDays.TryGetValue(GetTodayNum(), out WorkDay today);

            if (today != null)
                return today;
            else
            {
                WorkDay day = new WorkDay(isLunchTimeWorkTime);
                SetCurrentDay(ref day);

                return day;
            }
        }

        internal void SetCurrentDay(ref WorkDay today)
        {
            WorkDays[GetTodayNum()] = today;
            NotifyPropertyChanged();
        }

        // This is a risky method as it ASSUMES that the Dictionary is in ORDER.
        // TAKE CARE!!!!!
        internal WorkDay GetYesterWorkDay()
        {
            bool todayFlag = false;
            int todayNum = GetTodayNum();
            List<int> keyList = WorkDays.Keys.ToList();

            WorkDay yesterWorkDay = null;

            for (int i = keyList.Count - 1; i >= 0; --i)
            {
                if (todayFlag)
                {
                    yesterWorkDay = WorkDays[keyList[i]];
                    break;
                }

                if (keyList[i] == todayNum)
                {
                    todayFlag = true;
                }
            }

            return yesterWorkDay;
        }

        private void CountWorkedDays()
        {
            NoOfDaysWorked = WorkDays.Count;
        }

        private int GetTodayNum()
        {
            return StatisticsService.GetTodayNum();
        }

        #region Event Handling
        private void WorkMonth_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
