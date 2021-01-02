using System;
using System.Timers;
using WorkTimeStat.Services;

namespace WorkTimeStat.Timers
{
    internal class SaveTimer : IDisposable
    {
        private readonly Timer timer;

        public SaveTimer(TimeSpan interval)
        {
            double timeout = interval.TotalMilliseconds;
            timer = new Timer(timeout)
            {
                AutoReset = true
            };
            timer.Elapsed += Timer_Elapsed;

            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SaveService.SaveData(SaveUsage.Yes);
        }

        public void Dispose()
        {
            ((IDisposable)timer).Dispose();
        }
    }
}
