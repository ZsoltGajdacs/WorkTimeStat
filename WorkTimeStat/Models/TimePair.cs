using Newtonsoft.Json;
using System;

namespace WorkTimeStat.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class TimePair
    {
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public TimePair(DateTime startTime)
        {
            StartTime = startTime;
        }

        [JsonConstructor]
        private TimePair(DateTime startTime, DateTime endTime) : this(startTime)
        {
            EndTime = endTime;
        }

        public void End()
        {
            EndTime = DateTime.Now;
        }
    }
}
