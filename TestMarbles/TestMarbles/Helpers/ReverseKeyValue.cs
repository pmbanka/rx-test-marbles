using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarbles.Helpers
{
    internal static partial class Helpers
    {
        // Modified from http://stackoverflow.com/a/22595707/1108916
        public static IReadOnlyDictionary<TValue, TKey> ReverseKeyValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            var dictionary = new Dictionary<TValue, TKey>();
            foreach (var entry in source)
            {
                if (!dictionary.ContainsKey(entry.Value))
                {
                    dictionary.Add(entry.Value, entry.Key);
                }
            }
            return dictionary;
        }
    }
}
