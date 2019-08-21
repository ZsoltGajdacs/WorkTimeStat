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
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int LunchBreakDuration { get; set; }
        public int OtherBreakDuration { get; set; }
        public int OverWorkDuration { get; set; }
        public decimal WaterConsumptionCount { get; set; }
        public decimal AmountOfLitreInOneUnit { get; set; }
        public Boolean IsLunchTimeWorkTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public WorkDay(bool isLunchTimeWorkTime)
        {
            AmountOfLitreInOneUnit = 1;
            IsLunchTimeWorkTime = isLunchTimeWorkTime;
        }

        internal void IncreaseWaterConsumption()
        {
            WaterConsumptionCount = Decimal.Add(WaterConsumptionCount, 0.5m);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
