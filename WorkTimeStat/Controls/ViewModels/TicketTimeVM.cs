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

        internal readonly TicketTime storedTicket;

        private string _ticketName = string.Empty;
        private string _startTimeText = string.Empty;
        private string _timeSpentText = string.Empty;
        private string _statusBtnText = string.Empty;

        private TicketStatus status = TicketStatus.STOPPED;

        public string TicketName { get => _ticketName; set => SetAndNotifyPropertyChanged(ref _ticketName, value); }
        public string StartTimeText { get => _startTimeText; set => SetAndNotifyPropertyChanged(ref _startTimeText, value); }
        public string TimeSpentText { get => _timeSpentText; set => SetAndNotifyPropertyChanged(ref _timeSpentText, value); }
        public string StatusBtnText { get => _statusBtnText; set => SetAndNotifyPropertyChanged(ref _statusBtnText, value); }

        public TicketTimeVM(TicketTime storedTicket)
        {
            if (storedTicket == null)
            {
                throw new ArgumentNullException(nameof(storedTicket));
            }

            this.storedTicket = storedTicket;

            TicketName = storedTicket.TicketName;
            
            status = storedTicket.CurrentTimePair.EndTime == default
                        ? TicketStatus.ONGOING
                        : TicketStatus.STOPPED;

            StatusBtnText = status == TicketStatus.ONGOING
                                ? STOP_TEXT
                                : START_TEXT;

            if (storedTicket.UsageTimes.Count > 0)
            {
                StartTimeText = storedTicket.UsageTimes[0].StartTime.ToShortTimeString();
                RefreshTimeSpent();
            }
        }

        internal void ChangeStatus()
        {
            if (status == TicketStatus.ONGOING)
            {
                status = TicketStatus.STOPPED;
                StatusBtnText = START_TEXT;
                storedTicket.PauseTicket();
            }
            else
            {
                status = TicketStatus.ONGOING;
                StatusBtnText = STOP_TEXT;
                storedTicket.StartTicket();
                StartTimeText = storedTicket.UsageTimes[0].StartTime.ToShortTimeString();
            }

            RefreshTimeSpent();
        }

        internal void StopIfOngoing()
        {
            if (status == TicketStatus.ONGOING)
            {
                status = TicketStatus.STOPPED;
                StatusBtnText = START_TEXT;
                storedTicket.PauseTicket();
            }
        }

        private void RefreshTimeSpent()
        {
            string overallTime = Rounder.RoundToClosestTime(storedTicket.GetOverallSpentTime(), TimeSpan.FromMinutes(5))
                                             .TotalMinutes
                                             .ToString(CultureInfo.CurrentCulture);

            int lastUsageTime = storedTicket.UsageTimes.Count - 1;
            DateTime ticketStart = storedTicket.UsageTimes[0].StartTime;
            DateTime ticketEnd = storedTicket.UsageTimes[lastUsageTime].EndTime != default
                                                ? storedTicket.UsageTimes[lastUsageTime].EndTime
                                                : DateTime.Now;

            string measuredTime = StatisticsService.GetUsageForDateTimeFrame(ticketStart, ticketEnd)
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
