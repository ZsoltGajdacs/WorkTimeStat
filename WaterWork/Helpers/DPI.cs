using System;
using System.Drawing;

namespace WorkTimeStat.Helpers
{
    /// <summary>
    /// https://www.codeproject.com/questions/558094/howplustoplusgetplustheplussystemplusdpiplusplusse
    /// </summary>
    internal static class Dpi
    {
        private const float BaseScaleNum = 96;
        private const float BaseScaleUnit = BaseScaleNum / 100;

        /// <summary>
        /// It gives back the currentDPI of the screen. You need this one!
        /// </summary>
        public static float GetDpiX()
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                return graphics.DpiX;
            }
        }

        public static float GetDpiY()
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                return graphics.DpiY;
            }
        }

        /// <summary>
        /// Returns a number between 1 and 2.5
        /// </summary>
        public static double GetScaling()
        {
            float xDpi = GetDpiX();
            float scale = xDpi / BaseScaleUnit;

            return Math.Round(scale) / 100;
        }
    }
}
