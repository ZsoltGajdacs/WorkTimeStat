using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorkTimeStat.Controls.ViewModels;
using WorkTimeStat.Events;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Controls
{
    public partial class TicketTimeControl : UserControl
    {
        private static readonly string INPUT_PLACEHOLDER = LocalizationHelper.Instance.GetStringForKey("tt_input_default_text");

        public BindingList<TicketTimeVM> TicketTimeList { get; private set; }

        internal event CloseTheBallonEventHandler CloseBallon;

        public TicketTimeControl()
        {
            InitializeComponent();

            TicketTimeList = InitializeTicketList(WorkKeeper.Instance.GetTodaysTickets());
            TicketTimeItems.ItemsSource = TicketTimeList;
        }

        private BindingList<TicketTimeVM> InitializeTicketList(List<TicketTime> storedTickets)
        {
            BindingList<TicketTimeVM> viewModelList = new BindingList<TicketTimeVM>();

            if (storedTickets != null)
            {
                storedTickets.ForEach(ticket => viewModelList.Add(new TicketTimeVM(ticket)));
            }

            return viewModelList;
        }

        #region Events handlers
        private void TaskNameInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!TaskNameInput.Text.Equals(INPUT_PLACEHOLDER) && !string.IsNullOrWhiteSpace(TaskNameInput.Text))
                {
                    AddNewTicket(TaskNameInput.Text);
                    ChangeColor(ref TaskNameInput, Brushes.DimGray);
                    TaskNameInput.Text = INPUT_PLACEHOLDER;
                }
            }

            AddDefaultToInputIfEmpty();
        }

        private void TaskNameInput_KeyDown(object sender, KeyEventArgs e)
        {
            ClearPlaceholder(ref TaskNameInput, e);
            ChangeColor(ref TaskNameInput, Brushes.Black);
        }

        private void TaskNameInput_MouseEnter(object sender, MouseEventArgs e)
        {
            ClearPlaceholder(ref TaskNameInput, null);
            ChangeColor(ref TaskNameInput, Brushes.Black);

            TaskNameInput.Focus();
        }

        private void TaskNameInput_MouseLeave(object sender, MouseEventArgs e)
        {
            AddDefaultToInputIfEmpty();
        }

        private void okButtonS_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveTicketData();
            CloseBallon?.Invoke();
        }
        #endregion

        #region Event helpers
        private void SaveTicketData()
        {
            List<TicketTime> tickets = TicketTimeList.Select(vm => vm.storedTicket).ToList();
            WorkKeeper.Instance.UpdateTicketList(tickets);
        }

        private void AddNewTicket(string name)
        {
            if (!TicketTimeList.Any(ticket => ticket.TicketName == name))
            {
                TicketTimeList.Add(new TicketTimeVM(new TicketTime(name)));
            }
        }

        private void AddDefaultToInputIfEmpty()
        {
            if (string.IsNullOrWhiteSpace(TaskNameInput.Text))
            {
                ChangeColor(ref TaskNameInput, Brushes.DimGray);
                TaskNameInput.Text = INPUT_PLACEHOLDER;
            }
        }

        /// <summary>
        /// Clears the placeholder for the passed textbox if the placeholder text is shown
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="e"></param>
        private static void ClearPlaceholder(ref TextBox textBox, KeyEventArgs e)
        {
            if (textBox.Text.Equals(INPUT_PLACEHOLDER) && (e == null || Key.Enter != e.Key))
            {
                textBox.Text = string.Empty;
            }
        }

        private static void ChangeColor(ref TextBox textBox, Brush color)
        {
            if (textBox.Foreground != color)
            {
                textBox.Foreground = color;
            }
        }
        #endregion

        #region Template handlers
        private void StatusBtn_Click(object sender, RoutedEventArgs e)
        {
            Button startBtn = sender as Button;
            string ticketName = startBtn.Tag as string;

            TicketTimeVM ticketToWorkOn = TicketTimeList.FirstOrDefault(ticket => ticket.TicketName == ticketName);
            ticketToWorkOn.ChangeStatus();

            TicketTimeList.Where(ticket => ticket.TicketName != ticketToWorkOn.TicketName)
                .ToList()
                .ForEach(ticketToStop => ticketToStop.StopIfOngoing());
        }
        #endregion
    }
}
