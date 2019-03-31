using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WaterWork.Models;

namespace WaterWork.Windows
{
    /// <summary>
    /// Interaction logic for WorkLogWindow.xaml
    /// </summary>
    public partial class WorkLogWindow : Window
    {
        private static string INPUT_PLACEHOLDER = "Új feladat hozzáadása";

        private LogKeeper logKeeper;

        internal WorkLogWindow(ref LogKeeper logKeeper)
        {
            InitializeComponent();
            this.logKeeper = logKeeper;

            workLogGrid.DataContext = logKeeper;
            workLogItems.ItemsSource = logKeeper.WorkLogs;
        }

        private void WorkLogInput_KeyUp(object sender, KeyEventArgs e)
        {
            // If ENTER is pressed add to list
            if (e.Key == Key.Enter)
            {
                if (!workLogInput.Text.Equals(INPUT_PLACEHOLDER))
                {
                    AddNewLogItem(logKeeper.WorkLogs, workLogInput.Text);
                    workLogInput.Text = INPUT_PLACEHOLDER;
                }
            }

            // If the input is empty add placeholder
            if (workLogInput.Text.Equals(String.Empty))
            {
                workLogInput.Text = INPUT_PLACEHOLDER;
            }
        }

        private void WorkLogInput_KeyDown(object sender, KeyEventArgs e)
        {
            ClearPlaceholder(ref workLogInput, ref e);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        /// <summary>
        /// Adds new item to the list of workLogs
        /// </summary>
        /// <param name="itemList"></param>
        /// <param name="itemName"></param>
        private void AddNewLogItem(BindingList<LogEntry> itemList, string itemName)
        {
            LogEntry logEntry = itemList.Where(q => q.LogName.Equals(itemName)).SingleOrDefault();

            if (logEntry == null)
            {
                itemList.Add(new LogEntry(itemName));
            }
            else
            {
                MessageBox.Show("Ez a feladat már hozzá van adva!", "Duplikáció", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        /// <summary>
        /// Clears the placeholder for the passed textbox if the placeholder text is shown
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="e"></param>
        private void ClearPlaceholder(ref TextBox textBox, ref KeyEventArgs e)
        {
            if (textBox.Text.Equals(INPUT_PLACEHOLDER) && e.Key != Key.Enter)
            {
                textBox.Text = String.Empty;
            }
        }
    }
}
