using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;

namespace TestMarbles.Internal
{
    internal class SubscriptionExpectation : TestExpectation
    {
        private readonly IList<Subscription> _actual; 

        public SubscriptionExpectation(IList<Subscription> actualSubscriptions)
        {
            _actual = actualSubscriptions;
        }

        public IReadOnlyList<Subscription> Actual => _actual.AsReadOnly();

        public IReadOnlyList<Subscription> Expected { get; private set; }

        public void HandleToBe(IEnumerable<Subscription> expected)
        {
            Expected = expected.ToList();
            Ready = true;
        }

        public override void Assert()
        {
            Debug.Assert(Ready);
            if (Expected.Count != Actual.Count)
            {
                throw new ExpectSubscriptionToBeFailedException(
                    $"Different number of subscriptions were provided. Expected {Expected.Count} but was {Actual.Count}");
            }
            for (int i = 0; i < Actual.Count; i++)
            {
                Assert(Expected[i], Actual[i], i);
            }
        }

        private static void Assert(Subscription expected, Subscription actual, int index)
        {
            if (expected.Subscribe != actual.Subscribe)
            {
                throw new ExpectSubscriptionToBeFailedException(
                    $"Subscription time at index {index} do not match. Expected {expected.Subscribe} but was {actual.Subscribe}");
            }
            if (expected.Unsubscribe != actual.Unsubscribe)
            {
                throw new ExpectSubscriptionToBeFailedException(
                    $"Unsubscription time at index {index} do not match. Expected {expected.Unsubscribe} but was {actual.Unsubscribe}");
            }
        }
    }
}