using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WorkTimeStat.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class TicketTime
    {
        public string TicketName { get; private set; }

        [JsonIgnore]
        public TimePair CurrentTimePair { get; private set; }

        public List<TimePair> UsageTimes { get; private set; }

        public TicketTime(string ticketName)
        {
            TicketName = ticketName;
            UsageTimes = new List<TimePair>();

            StartTicket();
        }

        [JsonConstructor]
        private TicketTime(string ticketName, List<TimePair> usageTimes)
        {
            TicketName = ticketName ?? throw new ArgumentNullException(nameof(ticketName));
            UsageTimes = usageTimes ?? throw new ArgumentNullException(nameof(usageTimes));

            CurrentTimePair = UsageTimes[UsageTimes.Count - 1];
        }

        public void StartTicket()
        {
            if (CurrentTimePair == null || CurrentTimePair.EndTime != default)
            {
                CurrentTimePair = new TimePair(DateTime.Now);
                UsageTimes.Add(CurrentTimePair);
            }

            UsageTimes.Sort((thisTime, thatTime) => DateTime.Compare(thisTime.StartTime, thatTime.StartTime));
        }

        public void PauseTicket()
        {
            EndTicket();
        }

        public void EndTicket()
        {
            CurrentTimePair.End();
        }

        public TimeSpan GetOverallSpentTime()
        {
            TimeSpan spentTime = TimeSpan.Zero;
            foreach (var usage in UsageTimes)
            {
                DateTime end = usage.EndTime != default
                                              ? usage.EndTime
                                              : DateTime.Now;

                spentTime += end - usage.StartTime;
            }

            return spentTime;
        }
    }
}
