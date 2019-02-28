using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterWork.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class Keeper
    {
        public Dictionary<int, WorkYear> WorkYears { get; set; }

        internal Keeper()
        {
            WorkYears = new Dictionary<int, WorkYear>();
        }

        internal WorkYear GetCurrentYear()
        {
            WorkYears.TryGetValue(GetCurrentYearNum(), out WorkYear thisYear);

            return thisYear ?? new WorkYear();
        }

        internal WorkMonth GetCurrentMonth()
        {
            return GetCurrentYear().GetCurrentMonth();
        }

        internal WorkDay GetCurrentDay()
        {
            return GetCurrentMonth().GetCurrentDay();
        }

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

        private int GetCurrentYearNum()
        {
            return int.Parse(DateTime.Now.Year.ToString());
        }
    }
}
