using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;

namespace TestMarbles.Extensions
{
    public static partial class TestSchedulerExtensions
    {
        public static ITestableObservable<char> CreateColdObservable(
            this TestScheduler scheduler,
            string marbles,
            Exception error = null)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }
            if (marbles == null)
            {
                throw new ArgumentNullException(nameof(marbles));
            }
            marbles.CheckIfValidColdObservable(nameof(marbles));
            var events = Marbles.ToNotifications(marbles, error).ToArray();
            return scheduler.CreateColdObservable(events);
        }

        public static ITestableObservable<T> CreateColdObservable<T>(
            this TestScheduler scheduler,
            string marbles,
            IReadOnlyDictionary<char, T> values,
            Exception error = null)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }
            if (marbles == null)
            {
                throw new ArgumentNullException(nameof(marbles));
            }
            if (values == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }
            marbles.CheckIfValidColdObservable(nameof(marbles));
            values.CheckIfContainsMarkers(nameof(values));
            var events = Marbles.ToNotifications(marbles, values, error).ToArray();
            return scheduler.CreateColdObservable(events);
        }
    }
}