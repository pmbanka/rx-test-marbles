using System;
using System.Collections.Generic;
using System.Linq;
using TestMarbles.Helpers;
using TestMarbles.Internal;

namespace TestMarbles
{
    public class ObservableToBe
    {
        private readonly ObservableExpectation<char> _expectation;

        internal ObservableToBe(ObservableExpectation<char> expectation)
        {
            _expectation = expectation;
        }

        public void ToBe(
            string marbles,
            Exception error = null)
        {
            Ensure.NotNull(marbles, nameof(marbles));
            _expectation.HandleToBe(marbles, Marbles.ToNotifications(marbles, error));
        }
    }

    public class ObservableToBe<T>
    {
        private readonly ObservableExpectation<T> _expectation;

        internal ObservableToBe(ObservableExpectation<T> expectation)
        {
            _expectation = expectation;
        }

        public void ToBe(
            string marbles,
            IReadOnlyDictionary<char, T> values,
            Exception error = null)
        {
            Ensure.NotNull(marbles, nameof(marbles));
            Ensure.NotNull(values, nameof(values));
            _expectation.HandleToBe(marbles, Marbles.ToNotifications(marbles, values, error), values);
        }
    }

    public class SubscriptionToBe
    {
        private readonly SubscriptionExpectation _expectation;

        internal SubscriptionToBe(SubscriptionExpectation expectation)
        {
            _expectation = expectation;
        }

        public void ToBe(params string[] marbles)
        {
            _expectation.HandleToBe(marbles
                .Select(Marbles.ToSubscription));
        }
    }
}