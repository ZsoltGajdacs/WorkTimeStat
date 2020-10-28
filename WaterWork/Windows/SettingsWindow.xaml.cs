using System;
using System.Linq;
using System.Windows;
using UsageWatcher;
using WaterWork.Storage;

namespace WaterWork.Windows
{
    public partial class SettingsWindow : Window
    {
        private readonly WorkKeeper keeper;

        internal SettingsWindow(ref WorkKeeper keeper)
        {
            InitializeComponent();
            this.keeper = keeper;

            ResCombo.ItemsSource = Enum.GetValues(typeof(Resolution)).Cast<Resolution>();
            mainGrid.DataContext = this.keeper.Settings;
        }
    }
}
