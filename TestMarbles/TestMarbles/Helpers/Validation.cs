﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarbles.Helpers
{
    internal static partial class Helpers
    {
        public static void CheckIfValidColdObservable(this string marbles, string argumentName)
        {
            if (marbles.Contains(Marker.Subscription))
            {
                throw new ArgumentException($"Cold observable cannot have subscription marker '{Marker.Subscription}'", argumentName);
            }
            if (marbles.Contains(Marker.Unsubscription))
            {
                throw new ArgumentException($"Cold observable cannot have unsubscription marker '{Marker.Unsubscription}'", argumentName);
            }
        }

        public static void CheckIfContainsMarkers<T>(this IEnumerable<KeyValuePair<char, T>> dict, string argumentName)
        {
            var marker = dict.Select(p => p.Key).FirstOrDefault(p => Marker.All.Contains(p));
            if (marker != default(char))
            {
                throw new ArgumentException($"Dictionary cannot contain mapping for a special \"{marker}\" marker.", argumentName);
            }
        }

        public static void CheckIfContainsMarkers<T>(this IEnumerable<KeyValuePair<T, char>> dict, string argumentName)
        {
            var marker = dict.Select(p => p.Value).FirstOrDefault(p => Marker.All.Contains(p));
            if (marker != default(char))
            {
                throw new ArgumentException($"Dictionary cannot contain mapping for a special \"{marker}\" marker.", argumentName);
            }
        }
    }
}
