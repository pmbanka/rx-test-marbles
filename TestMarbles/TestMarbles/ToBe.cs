using System;
using System.Collections.Generic;
using System.Linq;

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
            _expectation.Expected.AddRange(Marbles.ToNotifications(marbles, error));
            _expectation.Ready = true;
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
            _expectation.Expected.AddRange(Marbles.ToNotifications(marbles, values, error));
            _expectation.Values = values;
            _expectation.Ready = true;
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
            _expectation.Expected = marbles
                .Select(Marbles.ToSubscription)
                .ToList();
            _expectation.Ready = true;
        }
    }
}