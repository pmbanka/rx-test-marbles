using System;
using TestMarbles.Utils;

namespace TestMarbles
{
    public static partial class Marbles
    {
        public static long ToCompletionTime(string marbles)
        {
            Ensure.NotNull(marbles, nameof(marbles));
            var indexOf = marbles.IndexOf(Marker.Completed);
            if (indexOf == -1)
            {
                throw new ArgumentException($"Marble diagram for time should have a completion marker \"{Marker.Completed}\"",
                    nameof(marbles));
            }
            return indexOf * MarbleScheduler.FrameTimeFactor;
        }
    }
}