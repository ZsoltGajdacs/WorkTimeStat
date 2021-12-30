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
using WorkTimeStat.Services;

namespace WorkTimeStat.Controls
{
    public partial class TicketTimeControl : UserControl
    {
        private static readonly string INPUT_PLACEHOLDER = LocalizationHelper.Instance.GetStringForKey("tt_input_default_text");

        public BindingList<TicketTimeVM> TaskTimeList { get; private set; }

        internal event CloseTheBallonEventHandler CloseBallon;

        public TicketTimeControl()
        {
            InitializeComponent();

            TaskTimeList = InitializeTicketList(TaskService.GetTodaysTasks());
            TaskTimeItems.ItemsSource = TaskTimeList;
        }

        private BindingList<TicketTimeVM> InitializeTicketList(List<MeasuredTask> storedTasks)
        {
            BindingList<TicketTimeVM> viewModelList = new BindingList<TicketTimeVM>();

            if (storedTasks != null)
            {
                storedTasks.ForEach(ticket => viewModelList.Add(new TicketTimeVM(ticket)));
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
            List<MeasuredTask> tickets = TaskTimeList.Select(vm => vm.storedTask).ToList();
            TaskService.UpdateTaskList(tickets);
            TaskService.UpdateTaskbarTooltipWithActiveTask();
        }

        private void AddNewTicket(string name)
        {
            if (!TaskTimeList.Any(ticket => ticket.TaskName == name))
            {
                TaskTimeList.Add(new TicketTimeVM(new MeasuredTask(name)));
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

            TicketTimeVM ticketToWorkOn = TaskTimeList.FirstOrDefault(ticket => ticket.TaskName == ticketName);
            ticketToWorkOn.ChangeStatus();

            TaskTimeList.Where(ticket => ticket.TaskName != ticketToWorkOn.TaskName)
                .ToList()
                .ForEach(ticketToStop => ticketToStop.StopIfOngoing());
        }
        #endregion
    }
}
