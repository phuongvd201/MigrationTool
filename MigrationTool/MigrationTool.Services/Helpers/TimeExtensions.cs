using System;
using System.Globalization;

namespace MigrationTool.Services.Helpers
{
    public static class TimeExtensions
    {
        public static string AsInvariantString(this DateTime? date, TimeSpan? time)
        {
            return date.HasValue
                ? time.HasValue
                    ? (date.Value + time.Value).AsInvariantString()
                    : date.Value.AsInvariantString()
                : null;
        }

        public static string AsInvariantString(this DateTime dateTime)
        {
            return ((DateTime?)dateTime).AsInvariantString();
        }

        public static string AsInvariantString(this TimeSpan timeSpan)
        {
            return ((TimeSpan?)timeSpan).AsInvariantString();
        }

        public static string AsInvariantString(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToString(CultureInfo.InvariantCulture) : null;
        }

        public static string AsInvariantString(this TimeSpan? timeSpan)
        {
            return timeSpan.HasValue ? timeSpan.Value.ToString("c", CultureInfo.InvariantCulture) : null;
        }

        public static string AsDisplayDateString(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
        }

        public static string AsDisplayTimeString(this TimeSpan? date)
        {
            return date.HasValue ? date.Value.ToString("hh\\:mm\\:ss", CultureInfo.InvariantCulture) : null;
        }
    }
}