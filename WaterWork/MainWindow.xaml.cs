using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WaterWork.Dialogs;
using WaterWork.Models;
using WaterWork.Windows;

namespace WaterWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Keeper keeper;
        private readonly string savePath;

        private WorkYear year;
        private WorkMonth month;
        private WorkDay today;

        public MainWindow()
        {
            InitializeComponent();
            FileInfo file = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            savePath = file.DirectoryName + "\\waterwork - " + DateTime.Now.Year.ToString() + ".json";

            keeper = Deserialize();

            if (keeper == null)
            {
                keeper = new Keeper();
            }
        }

        #region Window Events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Serialize();
        }
        #endregion

        #region Tray Click Events
        private void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            GetWorkVars();

            WorkdayEdit dayEdit = new WorkdayEdit(ref today);
            today = dayEdit.ShowDialog();

            SetWorkVars();
        }

        private void TaskbarIcon_TrayRightMouseUp(object sender, RoutedEventArgs e)
        {
            GetWorkVars();

            today.IncreaseWaterConsumption();

            decimal waterAmount = today.WaterConsumptionCount * today.AmountOfLitreInOneUnit;

            taskbarIcon.ShowBalloonTip("Vízfogyasztás", "Már " + waterAmount + "l vizet ittál ma!", 
                                            Hardcodet.Wpf.TaskbarNotification.BalloonIcon.None);
            Thread.Sleep(3000);
            taskbarIcon.HideBalloonTip();

            SetWorkVars();
        }
        #endregion

        #region Menu Click Events
        private void SettingsItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void StatisticsItem_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWindow statisticsWindow = new StatisticsWindow(keeper.GetCurrentYear());
            statisticsWindow.Show();
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region Helpers
        private void GetWorkVars()
        {
            year = keeper.GetCurrentYear();
            month = year.GetCurrentMonth();
            today = month.GetCurrentDay();
        }

        private void SetWorkVars()
        {
            month.SetCurrentDay(ref today);
            year.SetCurrentMonth(ref month);
            keeper.SetCurrentYear(ref year);
        }
        #endregion

        #region JSON serialization
        private void Serialize()
        {
            TextWriter writer = null;
            try
            {
                string output = JsonConvert.SerializeObject(keeper);
                writer = new StreamWriter(savePath, false);
                writer.Write(output);
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }

        private Keeper Deserialize()
        {
            if (new FileInfo(savePath).Exists)
            {
                TextReader reader = null;
                try
                {
                    reader = new StreamReader(savePath);
                    var fileContents = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<Keeper>(fileContents);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
