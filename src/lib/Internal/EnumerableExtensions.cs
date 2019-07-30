using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ockham.RdbInternal
{
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Force iteration to end, invoking <paramref name="action"/> on each item in <paramref name="source"/>
        /// </summary>
        public static void InvokeAll<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
                action(item);
        }

        public static TValue ItemOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            if (source.TryGetValue(key, out TValue value)) return value;
            return default;
        }

        public static TValue ItemOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, bool create)
        {
            if (source.TryGetValue(key, out TValue value)) return value;
            if (create) return source[key] = default;
            return default;
        }

        public static TValue ItemOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue @default)
        {
            if (source.TryGetValue(key, out TValue value)) return value;
            return @default;
        }

        public static TValue ItemOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue @default, bool create)
        {
            if (source.TryGetValue(key, out TValue value)) return value;
            if (create) return source[key] = @default;
            return @default;
        }

        public static TValue ItemOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TValue> @default)
        {
            if (source.TryGetValue(key, out TValue value)) return value;
            return @default();
        }

        public static TValue ItemOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TValue> @default, bool create)
        {
            if (source.TryGetValue(key, out TValue value)) return value;
            if (create) return source[key] = @default();
            return @default();
        }

        public static IEnumerable<T> Iterate<T>(this IEnumerable<T> source) => source.Select(x => x);

    }
}
