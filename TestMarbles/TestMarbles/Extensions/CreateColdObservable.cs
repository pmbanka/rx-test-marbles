using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;
using TestMarbles.Utils;

namespace TestMarbles.Extensions
{
    public static partial class TestSchedulerExtensions
    {
        public static ITestableObservable<char> CreateColdObservable(
            this TestScheduler scheduler,
            string marbles,
            Exception error = null)
        {
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(marbles, nameof(marbles));
            Ensure.ValidColdObservable(marbles, nameof(marbles));
            var events = Marbles.ToNotifications(marbles, error).ToArray();
            return scheduler.CreateColdObservable(events);
        }

        public static ITestableObservable<T> CreateColdObservable<T>(
            this TestScheduler scheduler,
            string marbles,
            IReadOnlyDictionary<char, T> values,
            Exception error = null)
        {
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(marbles, nameof(marbles));
            Ensure.NotNull(values, nameof(values));
            Ensure.ValidColdObservable(marbles, nameof(marbles));
            Ensure.NotContainsMarkers(values, nameof(values));
            var events = Marbles.ToNotifications(marbles, values, error).ToArray();
            return scheduler.CreateColdObservable(events);
        }
    }
}