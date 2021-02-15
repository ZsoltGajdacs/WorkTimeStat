using System;
using System.Globalization;
using UsageWatcher.Enums;
using System.Windows.Data;
using UsageWatcher.Enums.Utils;

namespace WorkTimeStat.Enums.Converters
{
    public class EnumNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum enumToString = (Enum)value;
            return enumToString.GetDisplayName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
