using System;
using System.Collections.Generic;

namespace WorkTimeStat.Models
{
    internal class KeeperDto
    {
        public Dictionary<DateTime, WorkDay> WorkDays { get; set; }
        public List<DateTime> LeaveDays { get; set; }
        public List<DateTime> SickDays { get; set; }
    }
}
