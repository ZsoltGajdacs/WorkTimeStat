using System;
using System.Linq;
using System.Windows;
using UsageWatcher;
using UsageWatcher.Enums;
using WorkTimeStat.Enums;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Windows
{
    public partial class SettingsWindow : Window
    {
        private readonly WorkKeeper keeper;

        internal SettingsWindow(ref WorkKeeper keeper)
        {
            InitializeComponent();
            this.keeper = keeper;

            ResCombo.ItemsSource = Enum.GetValues(typeof(Resolution)).Cast<Resolution>();
            WorkPlaceCombo.ItemsSource = Enum.GetValues(typeof(WorkPlaceType)).Cast<WorkPlaceType>();

            mainGrid.DataContext = this.keeper.Settings;
        }
    }
}
