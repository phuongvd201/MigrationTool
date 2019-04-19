using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

using Log4net = log4net;

namespace MigrationTool.Services.Helpers
{
    internal static class XmlExtensions
    {
        private static readonly Log4net.ILog logger = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string DateFormat = "yyyy-MM-dd";

        private delegate bool TryParseHandler<T>(string value, out T result);

        private static TValue GetValue<TValue>(this XElement element, XName name, Func<string, TValue> transform)
        {
            var valueElement = element.Element(name);
            return transform(valueElement == null ? string.Empty : valueElement.Value);
        }

        private static Func<string, TValue> TransformedOrDefault<TValue>(this TryParseHandler<TValue> tryTransform, TValue defaultValue)
        {
            return value =>
            {
                TValue result;
                if (string.IsNullOrWhiteSpace(value) || !tryTransform(value, out result))
                {
                    logger.Warn("Unable to transform XML value '" + (value ?? "null") + "'");
                    return @defaultValue;
                }
                return result;
            };
        }

        private static TryParseHandler<DateTime?> TryParseNullableDateTimeExact(string dateFormat)
        {
            return (string value, out DateTime? result) =>
            {
                DateTime localResult;
                if (DateTime.TryParseExact(value, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out localResult))
                {
                    result = localResult;
                    return true;
                }

                result = null;
                return false;
            };
        }

        public static int GetInt(this XElement element, XName name)
        {
            return GetValue(element, name, TransformedOrDefault(int.TryParse, default(int)));
        }

        public static bool GetBoolean(this XElement element, XName name)
        {
            return GetValue(element, name, TransformedOrDefault(bool.TryParse, default(bool)));
        }

        public static DateTime GetDateTime(this XElement element, XName name)
        {
            return GetValue(element, name, TransformedOrDefault(TryParseNullableDateTimeExact(DateFormat), null)).GetValueOrDefault(new DateTime(1900, 1, 1));
        }

        public static DateTime? GetNullableDateTime(this XElement element, XName name)
        {
            return GetValue(element, name, TransformedOrDefault(TryParseNullableDateTimeExact(DateFormat), null));
        }

        public static string GetString(this XElement element, XName name)
        {
            return GetValue(element, name, x => x);
        }

        public static string GetBase64String(this XElement element, XName name)
        {
            return GetValue(element, name, x => Encoding.UTF8.GetString(Convert.FromBase64String(x)));
        }
    }
}