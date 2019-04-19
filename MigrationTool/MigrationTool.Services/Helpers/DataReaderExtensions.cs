using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;

namespace MigrationTool.Services.Helpers
{
    internal static class DataReaderExtensions
    {
        internal static DateTime? GetNullableDateTime(this DbDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? null : (DateTime?)reader.GetDateTime(column);
        }

        internal static int? GetNullableInt32(this DbDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? null : (int?)reader.GetInt32(column);
        }

        internal static string GetTrimString(this DbDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? null : reader.GetString(column).Trim();
        }

        internal static string SafeGetString(this DbDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? null : reader.GetString(column);
        }

        internal static bool? GetNullableBoolean(this DbDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? null : (bool?)reader.GetBoolean(column);
        }

        internal static int? GetNullableInt16(this DbDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? null : (int?)reader.GetInt16(column);
        }

        internal static TimeSpan GetTimeSpan(this DbDataReader reader, int column)
        {
            return (TimeSpan)reader.GetValue(column);
        }

        internal static TimeSpan? GetNullableTimeSpan(this DbDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? null : (TimeSpan?)reader.GetValue(column);
        }

        internal static byte[] GetBlob(this DbDataReader reader, int column)
        {
            using (var stream = reader.GetStream(column))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        internal static int GetCount(this DbDataReader reader, Dictionary<string, int> columns)
        {
            return reader.IsDBNull(0) ? 0 : int.Parse(reader.GetValue(0).ToString());
        }
    }
}