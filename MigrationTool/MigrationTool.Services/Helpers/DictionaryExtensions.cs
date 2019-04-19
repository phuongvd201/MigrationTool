using System.Collections.Generic;

namespace MigrationTool.Services.Helpers
{
    internal static class DictionaryExtensions
    {
        internal static TEntity GetValueOrNull<TKey, TEntity>(this Dictionary<TKey, TEntity> dictionary, TKey key) where TEntity : class
        {
            return dictionary.GetValueOrDefault(key, null);
        }

        internal static TEntity GetValueOrNull<TKey, TEntity>(this Dictionary<TKey, TEntity> dictionary, TKey? key)
            where TKey : struct
            where TEntity : class
        {
            return key.HasValue ? dictionary.GetValueOrDefault(key.Value, null) : null;
        }

        internal static TEntity GetValueOrDefault<TKey, TEntity>(this Dictionary<TKey, TEntity> dictionary, TKey key, TEntity @default) where TEntity : class
        {
            TEntity value;
            return (key != null && dictionary.TryGetValue(key, out value)) ? value : @default;
        }
    }
}