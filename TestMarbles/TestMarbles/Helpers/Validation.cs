using System;
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

        
    }
}
