using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Services;
using ZsGUtils.UIHelpers;

namespace WorkTimeStat.Controls.ViewModels
{
    public class TicketTimeVM : BindableClass
    {
        private static readonly string START_TEXT = LocalizationHelper.Instance.GetStringForKey("tt_ticket_start");
        private static readonly string STOP_TEXT = LocalizationHelper.Instance.GetStringForKey("tt_ticket_stop");
        private static readonly string MINUTE_TEXT = LocalizationHelper.Instance.GetStringForKey("u_minute");

        internal readonly MeasuredTask storedTask;

        private string _taskName = string.Empty;
        private string _startTimeText = string.Empty;
        private string _timeSpentText = string.Empty;
        private string _statusBtnText = string.Empty;

        private TicketStatus status = TicketStatus.STOPPED;

        public string TaskName { get => _taskName; set => SetAndNotifyPropertyChanged(ref _taskName, value); }
        public string StartTimeText { get => _startTimeText; set => SetAndNotifyPropertyChanged(ref _startTimeText, value); }
        public string TimeSpentText { get => _timeSpentText; set => SetAndNotifyPropertyChanged(ref _timeSpentText, value); }
        public string StatusBtnText { get => _statusBtnText; set => SetAndNotifyPropertyChanged(ref _statusBtnText, value); }

        public TicketTimeVM(MeasuredTask storedTask)
        {
            if (storedTask == null)
            {
                throw new ArgumentNullException(nameof(storedTask));
            }

            this.storedTask = storedTask;
            TaskName = storedTask.TaskName;
            
            status = storedTask.CurrentTimePair.EndTime == default
                        ? TicketStatus.ONGOING
                        : TicketStatus.STOPPED;

            StatusBtnText = status == TicketStatus.ONGOING
                                ? STOP_TEXT
                                : START_TEXT;

            if (storedTask.UsageTimes.Count > 0)
            {
                StartTimeText = storedTask.UsageTimes[0].StartTime.ToShortTimeString();
                RefreshTimeSpent();
            }
        }

        internal void ChangeStatus()
        {
            if (status == TicketStatus.ONGOING)
            {
                status = TicketStatus.STOPPED;
                StatusBtnText = START_TEXT;
                storedTask.PauseTask();
            }
            else
            {
                status = TicketStatus.ONGOING;
                StatusBtnText = STOP_TEXT;
                storedTask.StartTask();
                StartTimeText = storedTask.UsageTimes[0].StartTime.ToShortTimeString();
            }

            RefreshTimeSpent();
        }

        internal void StopIfOngoing()
        {
            if (status == TicketStatus.ONGOING)
            {
                status = TicketStatus.STOPPED;
                StatusBtnText = START_TEXT;
                storedTask.PauseTask();
            }
        }

        private void RefreshTimeSpent()
        {
            string overallTime = Rounder.RoundToClosestTime(storedTask.GetOverallSpentTime(), TimeSpan.FromMinutes(5))
                                             .TotalMinutes
                                             .ToString(CultureInfo.CurrentCulture);

            DateTime ticketStart = storedTask.GetStartTime();
            DateTime ticketEnd = storedTask.GetEndTime();

            string measuredTime = StatisticsService.GetUsageForDateTimeFrame(ticketStart, ticketEnd, TimeSpan.FromMinutes(5))
                                                            .ToString(CultureInfo.CurrentCulture);

            TimeSpentText = string.Format(CultureInfo.CurrentCulture, "{0} {1} / {2} {3}", measuredTime, MINUTE_TEXT,
                                                                                        overallTime, MINUTE_TEXT);
        }

        private enum TicketStatus
        {
            ONGOING, STOPPED
        }
    }
}
