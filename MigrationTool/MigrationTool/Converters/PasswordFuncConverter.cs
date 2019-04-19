using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MigrationTool.Converters
{
    internal class PasswordFuncConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Func<string>(() => ((PasswordBox)value).Password);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("No conversion.");
        }
    }
}