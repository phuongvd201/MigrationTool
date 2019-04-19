using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MigrationTool.Services.Helpers
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, bool parallel, Action<T> action)
        {
            if (parallel)
            {
                var partitioner = Partitioner.Create(source, EnumerablePartitionerOptions.NoBuffering);
                Parallel.ForEach(partitioner, action);
            }
            else
            {
                foreach (var obj in source)
                {
                    action(obj);
                }
            }
        }

        public static string AsSeparatedString<T>(
            this IEnumerable<T> source,
            Func<T, string[]> getFields,
            string valuesSeparator,
            string objectsSeparator)
        {
            return source == null
                ? null
                : string.Join(
                    objectsSeparator,
                    source
                        .Select(x =>
                        {
                            var fields = getFields(x);

                            return fields == null
                                ? null
                                : string.Join(
                                    valuesSeparator,
                                    fields.Where(y => !string.IsNullOrWhiteSpace(y)));
                        })
                        .Where(y => !string.IsNullOrWhiteSpace(y)));
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}