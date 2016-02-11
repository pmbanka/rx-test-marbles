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
        public static ITestableObservable<char> CreateColdObservable(
            this TestScheduler scheduler,
            string marbles,
            Exception error = null)
        {
            return scheduler.CreateColdObservable<char>(marbles, null, error);
        }

        public static ITestableObservable<T> CreateColdObservable<T>(
            this TestScheduler scheduler,
            string marbles,
            IReadOnlyDictionary<char, T> values = null,
            Exception error = null)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }
            if (string.IsNullOrWhiteSpace(marbles))
            {
                throw new ArgumentException("Cannot be either null, empty, nor whitespace.", nameof(marbles));
            }
            if (marbles.IndexOf('^') != -1)
            {
                throw new ArgumentException("Cold observable cannot have subscription offset '^'", nameof(marbles));
            }
            if (marbles.IndexOf('!') != -1)
            {
                throw new ArgumentException("Cold observable cannot have unsubscription marker '!'", nameof(marbles));
            }
            if (values == null && typeof(T) != typeof(char))
            {
                throw new ArgumentNullException(nameof(values),
                    "If observable type is not char, values dictionary has to be provided");
            }
            var events = TestSchedulerEx.ParseMarbles(marbles, values, error).ToArray();
            return scheduler.CreateColdObservable(events);
        }
    }
}