using System;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace WorkTimeStat.Helpers
{
    public class LocalizationHelper
    {
        private readonly ResourceManager resManager;

        #region Singleton
        public static LocalizationHelper Instance { get { return lazy.Value; } }

        private LocalizationHelper()
        {
            resManager = new ResourceManager("WorkTimeStat.Properties.Resources", Assembly.GetExecutingAssembly());
        }

        private static readonly Lazy<LocalizationHelper> lazy = new Lazy<LocalizationHelper>(() =>
        {
            return new LocalizationHelper();
        });

        #endregion

        public string GetStringForKey(string resKey)
        {
            return resManager.GetString(resKey, Thread.CurrentThread.CurrentUICulture);
        }
    }
}
