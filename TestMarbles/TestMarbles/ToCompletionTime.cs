using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;

namespace TestMarbles
{
    public static partial class Marbles
    {
        public static long ToCompletionTime(string marbles)
        {
            var indexOf = marbles.IndexOf(Marker.Completed);
            if (indexOf == -1)
            {
                throw new ArgumentException($"Marble diagram for time should have a completion marker \"{Marker.Completed}\"",
                    nameof(marbles));
            }
            return indexOf * Constants.FrameTimeFactor;
        }
    }
}