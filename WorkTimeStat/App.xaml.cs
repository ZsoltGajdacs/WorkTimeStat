using System.Windows;

namespace WorkTimeStat
{
    public partial class App : Application
    {
        private App()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("hu-HU");
            
        }
    }
}
