using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;

namespace TestMarbles.Extensions
{
    public static partial class TestSchedulerExtensions
    {
        public static ITestableObservable<char> CreateHotObservable(
            this TestScheduler scheduler,
            string marbles,
            Exception error = null)
        {
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(marbles, nameof(marbles));
            var events = Marbles.ToNotifications(marbles, error).ToArray();
            return scheduler.CreateHotObservable(events);
        }

        public static ITestableObservable<T> CreateHotObservable<T>(
            this TestScheduler scheduler,
            string marbles,
            IReadOnlyDictionary<char, T> values,
            Exception error = null)
        {
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(marbles, nameof(marbles));
            Ensure.NotNull(values, nameof(values));
            Ensure.NotContainsMarkers(values, nameof(values));
            var events = Marbles.ToNotifications(marbles, values, error).ToArray();
            return scheduler.CreateHotObservable(events);
        }
    }
}