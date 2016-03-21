using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reactive.Testing;

namespace TestMarbles.Extensions
{
    public static partial class TestSchedulerExtensions
    {
        public static ITestableObservable<char> CreateHotObservable(
            this TestScheduler scheduler,
            string marbles,
            Exception error = null)
        {
            return scheduler.CreateHotObservable<char>(marbles, null, error);
        }

        public static ITestableObservable<T> CreateHotObservable<T>(
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
            if (values == null && typeof(T) != typeof(char))
            {
                throw new ArgumentNullException(nameof(values),
                    "If observable type is not char, values dictionary has to be provided");
            }
            var events = TestSchedulerEx.ParseMarbles(marbles, values, error).ToArray();
            return scheduler.CreateHotObservable(events);
        }
    }
}