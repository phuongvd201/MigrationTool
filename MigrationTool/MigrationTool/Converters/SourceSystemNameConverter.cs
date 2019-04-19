using System;
using System.Globalization;
using System.Windows.Data;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.Converters
{
    internal class SourceSystemNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((MigrationSourceSystem)value)
            {
                case MigrationSourceSystem.C2cXml:
                    return "C2C XML";

                case MigrationSourceSystem.MedicalDirector:
                    return "MEDICAL DIRECTOR";

                default:
                    return value.ToString().ToUpper();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("No conversion.");
        }
    }
}