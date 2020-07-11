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
    internal class WorkDay : INotifyPropertyChanged
    {
        public DateTime DayDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public UsageTime UsageTime { get; set; }
        public int LunchBreakDuration { get; set; }
        public int OtherBreakDuration { get; set; }
        public int OverWorkDuration { get; set; }
        public decimal WaterConsumptionCount { get; set; }
        public decimal AmountOfLitreInOneUnit { get; set; }
        public Boolean IsLunchTimeWorkTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #region CTORS
        public WorkDay(bool isLunchTimeWorkTime, decimal amountOfLiterInOneUnit)
        {
            AmountOfLitreInOneUnit = amountOfLiterInOneUnit;
            IsLunchTimeWorkTime = isLunchTimeWorkTime;
            DayDate = DateTime.Now.Date;
        }
        #endregion

        internal void IncreaseWaterConsumption()
        {
            WaterConsumptionCount = Decimal.Add(WaterConsumptionCount, AmountOfLitreInOneUnit);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
