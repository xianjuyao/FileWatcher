using LogLib;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FileWatcher.Converters
{
    public class LogInfoToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string item)) return Brushes.Green;
            if (item == LogLevel.Error)
            {
                return Brushes.Red;
            }

            if (item == LogLevel.Warning)
            {
                return Brushes.Orange;
            }
            return item == LogLevel.Debug ? Brushes.Blue : Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}