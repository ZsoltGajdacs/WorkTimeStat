using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterWork.Helpers
{
    /// <summary>
    /// https://www.codeproject.com/questions/558094/howplustoplusgetplustheplussystemplusdpiplusplusse
    /// </summary>
    internal static class DPI
    {
        private const float BaseScaleNum = 96;
        private static readonly float BaseScaleUnit = BaseScaleNum / 100;

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
        /// 96 DPI = 100%
        /// </summary>
        public static int GetScaling()
        {
            float xDpi = GetDpiX();
            float scale = xDpi / BaseScaleUnit;
            return (int)Math.Round(scale);
        }
    }
}
