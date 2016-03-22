using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestMarbles.Helpers
{
    internal static partial class Helpers
    {
        // Modified from http://stackoverflow.com/a/22595707/1108916
        public static IReadOnlyDictionary<T, char> ReverseKeyValue<T>(this IEnumerable<KeyValuePair<char, T>> source)
        {
            var dictionary = new Dictionary<T, char>();
            foreach (var entry in source)
            {
                if (!dictionary.ContainsKey(entry.Value))
                {
                    dictionary.Add(entry.Value, entry.Key);
                }
                else
                {
                    dictionary[entry.Value] = '?';
                }
            }
            return dictionary;
        }

        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> collection)
        {
            Ensure.NotNull(collection, nameof(collection));
            var list = collection as IReadOnlyList<T>;
            return list ?? new ReadOnlyCollection<T>(collection);
        }

        public static IEnumerable<MaterializedEnumerable<T>> Materialize<T>(this IEnumerable<T> self)
        {
            using (var enumerator = self.GetEnumerator())
            {
                bool isFirst = true;
                if (enumerator.MoveNext())
                {
                    bool isLast;
                    do
                    {
                        var current = enumerator.Current;
                        isLast = !enumerator.MoveNext();
                        yield return new MaterializedEnumerable<T>(current, isFirst, isLast);
                        isFirst = false;
                    } while (!isLast);
                }
            }
        }
    }

    internal class MaterializedEnumerable<T>
    {
        public MaterializedEnumerable(T value, bool isFirst, bool isLast)
        {
            IsFirst = isFirst;
            IsLast = isLast;
            Value = value;
        }

        public bool IsFirst { get; }

        public bool IsLast { get; }

        public T Value { get; }
    }
}