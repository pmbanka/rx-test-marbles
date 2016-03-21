using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reactive.Testing;

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
            if (values == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }
            var events = Marbles.ToNotifications(marbles, values, error).ToArray();
            return scheduler.CreateColdObservable(events);
        }
    }
}