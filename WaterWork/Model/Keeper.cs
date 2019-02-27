using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterWork.Model
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

        internal WorkDay GetToday()
        {
            WorkYears.TryGetValue(GetCurrentYear(), out WorkYear year);

            if (year != null)
            {
                return year.GetCurrentDay();
            }
            else
            {
                year = new WorkYear();
                WorkYears[GetCurrentYear()] = year;

                return year.GetCurrentDay();
            }
        }

        internal void SetToday(WorkDay today)
        {
            WorkYears[GetCurrentYear()].SetCurrentDay(today);
        }

        private int GetCurrentYear()
        {
            return int.Parse(DateTime.Now.Year.ToString());
        }
    }
}
