using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;

namespace TestMarbles
{
    internal abstract class TestExpectation
    {
        public bool Ready { get; set; }

        public abstract void Assert();
    }

    internal class ObservableExpectation<T> : TestExpectation
    {
        public ObservableExpectation()
        {
            Actual = new List<Recorded<Notification<T>>>();
            Expected = new List<Recorded<Notification<T>>>();
        }

        public List<Recorded<Notification<T>>> Actual { get; }

        public List<Recorded<Notification<T>>> Expected { get; }

        public IReadOnlyDictionary<char, T> Values { get; set; }

        public override void Assert()
        {
            if (Expected.Count != Actual.Count)
            {
                throw new ExpectObservableToBeFailedException(
                    $"Recorded unexpected number of notifications. Expected {Expected.Count} but was {Actual.Count}",
                    ExpectedMarbles,
                    ActualMarbles);
            }
            for (int i = 0; i < Expected.Count; i++)
            {
                Assert(Expected[i], Actual[i], i);
            }
        }

        private void Assert(Recorded<Notification<T>> expected, Recorded<Notification<T>> actual, int index)
        {
            var markerPosition = (int)(actual.Time/Constants.FrameTimeFactor) + GetNumberOfGroupsBefore(actual) * 2 + 1;
            if (expected.Time != actual.Time)
            {
                throw new ExpectObservableToBeFailedException(
                    $"Times for elements at index {index} do not match. Expected {expected.Time} but was {actual.Time}",
                    ExpectedMarbles,
                    ActualMarbles,
                    markerPosition);
            }
            var exVal = expected.Value;
            var acVal = actual.Value;
            if (exVal.Kind != acVal.Kind)
            {
                throw new ExpectObservableToBeFailedException(
                    $"Types of elements at index {index} do not match. Expected {exVal.Kind} but was {acVal.Kind}",
                    ExpectedMarbles,
                    ActualMarbles,
                    markerPosition);
            }
            if (exVal.Kind == NotificationKind.OnError && exVal.Exception.Message != acVal.Exception.Message)
            {
                throw new ExpectObservableToBeFailedException(
                    $"Errors of elements at index {index} do not match. Expected {exVal.Exception.Message} but was {acVal.Exception.Message}",
                    ExpectedMarbles,
                    ActualMarbles,
                    markerPosition);
            }
            if (exVal.Kind == NotificationKind.OnNext && !exVal.Value.Equals(acVal.Value))
            {
                throw new ExpectObservableToBeFailedException(
                    $"Elements at index {index} do not match. Expected {exVal.Value} but was {acVal.Value}",
                    ExpectedMarbles,
                    ActualMarbles,
                    markerPosition);
            }
        }

        private int GetNumberOfGroupsBefore(Recorded<Notification<T>> actual)
        {
            return Actual
                .TakeWhile(p => p != actual)
                .GroupBy(p => p.Time)
                .Count(g => g.Count() > 1);
        }

        private string ActualMarbles => 
            Marbles.GetMarblesOrErrorMessage(Actual, Values?.ReverseKeyValue());

        private string ExpectedMarbles => 
            Marbles.GetMarblesOrErrorMessage(Expected, Values?.ReverseKeyValue());
    }

    internal class SubscriptionExpectation : TestExpectation
    {
        public SubscriptionExpectation()
        {
            Actual = new List<Subscription>();
            Expected = new List<Subscription>();
        }

        public List<Subscription> Actual { get; set; }

        public List<Subscription> Expected { get; set; }

        public override void Assert()
        {
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

        private void Assert(Subscription expected, Subscription actual, int index)
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