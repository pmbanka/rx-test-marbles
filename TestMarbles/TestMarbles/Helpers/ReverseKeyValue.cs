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
    }
}
