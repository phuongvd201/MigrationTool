using System;
using System.Globalization;
using System.Windows.Data;

namespace MigrationTool.Converters
{
    internal class AppNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "Clinic to Cloud - Migration Tool " + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("No conversion.");
        }
    }
}