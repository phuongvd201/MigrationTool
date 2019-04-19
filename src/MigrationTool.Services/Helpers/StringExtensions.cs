using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using log4net;

using Newtonsoft.Json;

namespace MigrationTool.Services.Helpers
{
    public static class StringExtensions
    {
        private const string IllegalFileNameCharacters = @"/?<>\:*|""";
        private static readonly Regex RtfTagRegex = new Regex(@"\{\*?\\[^{}]+}|[{}]|\\\n?[A-Za-z]+\n?(?:-?\d+)?[ ]?", RegexOptions.Compiled);
        private static readonly Regex HtmlTagRegex = new Regex(@"<[^>]*>", RegexOptions.Compiled);

        public static bool TryParseJson<T>(this string json, ILog log, out T result)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                log.Error(new ArgumentNullException("json"));
                result = default(T);
                return false;
            }
            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch (Exception e)
            {
                log.Error(e);
                result = default(T);
                return false;
            }
        }

        public static bool IsValidFileName(this string fileName)
        {
            return !(string.IsNullOrWhiteSpace(fileName) || fileName.Intersect(IllegalFileNameCharacters).Any());
        }

        public static string WithFieldName(this string source, string fieldName)
        {
            return string.IsNullOrWhiteSpace(source)
                ? null
                : fieldName + ": " + source;
        }

        public static string NullIfEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public static bool EqualsIgnoreCase(this string firstValue, string secondValue)
        {
            return string.Equals(firstValue, secondValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string ReplaceRtfTagByHtmlBreakTag(this string rtfText)
        {
            return RtfTagRegex.Replace(rtfText, "<br />");
        }

        public static string RemoveHtmlTag(this string htmlText)
        {
            return HtmlTagRegex.Replace(htmlText, string.Empty).Trim();
        }

        public static bool IsRtfTextFormat(this string text)
        {
            return !string.IsNullOrEmpty(text) && text.TrimStart().StartsWith(@"{\rtf", StringComparison.InvariantCultureIgnoreCase);
        }

        public static float? GetNullableFloat(this string value)
        {
            float result;
            if (!string.IsNullOrWhiteSpace(value) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return null;
        }

        public static int? GetNullableInt(this string value)
        {
            int result;
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return null;
        }
    }
}