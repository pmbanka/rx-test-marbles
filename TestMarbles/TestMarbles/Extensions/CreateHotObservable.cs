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
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }
            if (string.IsNullOrWhiteSpace(marbles))
            {
                throw new ArgumentException("Cannot be either null, empty, nor whitespace.", nameof(marbles));
            }
            var events = Marbles.ToNotifications(marbles, error).ToArray();
            return scheduler.CreateHotObservable(events);
        }

        public static ITestableObservable<T> CreateHotObservable<T>(
            this TestScheduler scheduler,
            string marbles,
            IReadOnlyDictionary<char, T> values,
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
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            var events = Marbles.ToNotifications(marbles, values, error).ToArray();
            return scheduler.CreateHotObservable(events);
        }
    }
}