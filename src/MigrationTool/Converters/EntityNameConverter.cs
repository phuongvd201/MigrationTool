using System;
using System.Globalization;
using System.Windows.Data;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.Converters
{
    internal class EntityNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((MigrationEntity)value).ToMigrationEntityTypeName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("No conversion.");
        }
    }
}