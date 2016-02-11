using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;

namespace TestMarbles
{
    public static partial class TestSchedulerExtensions
    {
        public static long CreateTime(this TestScheduler scheduler, string marbles)
        {
            var indexOf = marbles.IndexOf('|');
            if (indexOf == -1)
            {
                throw new ArgumentException(@"Marble diagram for time should have a completion marker ""|""",
                    nameof(marbles));
            }
            return indexOf * Utils.FrameTimeFactor;
        }
    }
}