using System;
using System.Globalization;
using System.Windows.Data;

namespace FileWatcher.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // 你可以在这里指定你想要的日期格式，例如 "yyyy-MM-dd"
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss", culture);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dateString &&
                DateTime.TryParse(dateString, culture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }

            return value;
        }
    }
}