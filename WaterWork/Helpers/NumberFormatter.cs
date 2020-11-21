using System.Globalization;

namespace WaterWork.Helpers
{
    internal class NumberFormatter
    {
        public static string NO_DATA = "-";

        internal static string FormatNum(double num)
        {
            return num == 0 ? NO_DATA : num.ToString(CultureInfo.InvariantCulture);
        }
    }
}
