using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;

namespace TestMarbles.Internal
{
    internal class ObservableExpectation<T> : TestExpectation
    {
        private IEqualityComparer<T> _comparer;

        private string _expectedMarbles;

        private readonly List<Recorded<Notification<T>>> _actual;

        private readonly List<Recorded<Notification<T>>> _expected;

        public ObservableExpectation()
        {
            _actual = new List<Recorded<Notification<T>>>();
            _expected = new List<Recorded<Notification<T>>>();
        }

        public IReadOnlyList<Recorded<Notification<T>>> Actual => _actual;

        public IReadOnlyList<Recorded<Notification<T>>> Expected => _expected;

        public IReadOnlyDictionary<char, T> Values { get; private set; }

        public void AddNotification(long time, Notification<T> notification)
        {
            _actual.Add(new Recorded<Notification<T>>(time, notification));
        }

        public override void Assert()
        {
            Debug.Assert(Ready);
            if (Expected.Count != Actual.Count)
            {
                throw new ExpectObservableToBeFailedException(
                    $"Recorded unexpected number of notifications. Expected {Expected.Count} but was {Actual.Count}",
                    _expectedMarbles,
                    ActualMarbles);
            }
            for (int i = 0; i < Expected.Count; i++)
            {
                Assert(Expected[i], Actual[i], i);
            }
        }

        private void Assert(Recorded<Notification<T>> expected, Recorded<Notification<T>> actual, int index)
        {
            var markerPosition = (int)(actual.Time/MarbleScheduler.FrameTimeFactor) + GetNumberOfGroupsBefore(actual) * 2 + 1;
            if (expected.Time != actual.Time)
            {
                throw new ExpectObservableToBeFailedException(
                    $"Times for elements at index {index} do not match. Expected {expected.Time} but was {actual.Time}",
                    _expectedMarbles,
                    ActualMarbles,
                    markerPosition);
            }
            var exVal = expected.Value;
            var acVal = actual.Value;
            var time = actual.Time;
            if (exVal.Kind != acVal.Kind)
            {
                throw new ExpectObservableToBeFailedException(
                    $"Types of elements at time {time} (index {index}) do not match. Expected {exVal.Kind} but was {acVal.Kind}",
                    _expectedMarbles,
                    ActualMarbles,
                    markerPosition);
            }
            if (exVal.Kind == NotificationKind.OnError && exVal.Exception.GetType() != acVal.Exception.GetType())
            {               
                throw new ExpectObservableToBeFailedException(
                    $"Errors at time {time} (index {index}) do not match. Expected {exVal.Exception.GetType()} but was {acVal.Exception.GetType()}",
                    _expectedMarbles,
                    ActualMarbles,
                    markerPosition);
            }
            if (exVal.Kind == NotificationKind.OnNext && !_comparer.Equals(exVal.Value, acVal.Value))
            {
                throw new ExpectObservableToBeFailedException(
                    $"Elements at time {time} (index {index}) do not match. Expected {exVal.Value} but was {acVal.Value}",
                    _expectedMarbles,
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

        public void HandleToBe(
            string expectedMarbles, 
            IEnumerable<Recorded<Notification<T>>> expectedNotifications,
            IReadOnlyDictionary<char, T> values = null,
            IEqualityComparer<T> comparer = null)
        {
            _expectedMarbles = expectedMarbles;
            _expected.AddRange(expectedNotifications);
            Values = values;
            _comparer = comparer ?? EqualityComparer<T>.Default;
            Ready = true;
        }
    }
}