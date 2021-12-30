using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WorkTimeStat.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class MeasuredTask
    {
        public string TaskName { get; private set; }

        [JsonIgnore]
        public TimePair CurrentTimePair { get; private set; }

        public List<TimePair> UsageTimes { get; private set; }

        #region CTOR
        public MeasuredTask(string ticketName)
        {
            TaskName = ticketName;
            UsageTimes = new List<TimePair>();

            StartTask();
        }

        [JsonConstructor]
        private MeasuredTask(string taskName, List<TimePair> usageTimes)
        {
            TaskName = taskName ?? throw new ArgumentNullException(nameof(taskName));
            UsageTimes = usageTimes ?? throw new ArgumentNullException(nameof(usageTimes));

            CurrentTimePair = UsageTimes[UsageTimes.Count - 1];
        }
        #endregion

        public void StartTask()
        {
            if (CurrentTimePair == null || CurrentTimePair.EndTime != default)
            {
                CurrentTimePair = new TimePair(DateTime.Now);
                UsageTimes.Add(CurrentTimePair);
            }

            UsageTimes.Sort((thisTime, thatTime) => DateTime.Compare(thisTime.StartTime, thatTime.StartTime));
        }

        public void PauseTask()
        {
            EndTask();
        }

        public void EndTask()
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

        public DateTime GetStartTime()
        {
            return UsageTimes[0].StartTime;
        }

        public DateTime GetEndTime()
        {
            int lastUsageTime = UsageTimes.Count - 1;
            return UsageTimes[lastUsageTime].EndTime != default
                                            ? UsageTimes[lastUsageTime].EndTime
                                            : DateTime.Now;
        }
    }
}
