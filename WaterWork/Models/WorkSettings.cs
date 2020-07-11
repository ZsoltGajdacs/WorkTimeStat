using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterWork.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class WorkSettings : INotifyPropertyChanged
    {
        public Boolean IsLunchTimeWorkTimeDefault { get; set; }
        public decimal AmountOfLitreInOneUnit { get; set; }
        public int YearlyLeaveNumber { get; set; }
        public double DailyWorkHours { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
