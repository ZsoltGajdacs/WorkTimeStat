using System;
using System.Globalization;
using System.Windows.Data;
using UsageWatcher.Enums;
using WorkTimeStat.Helpers;

namespace WorkTimeStat.Enums.Converters
{
    public class EnumNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum enumToString = (Enum)value;
            string dispName = enumToString.GetDisplayName();
            string localizedName = LocalizationHelper.Instance.GetStringForKey(dispName);
            return localizedName ?? dispName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
