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

        private readonly List<Recorded<Notification<T>>> _actual;

        private readonly List<Recorded<Notification<T>>> _expected;
        private string _expectedMarbles;

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
                throw CreateError(
$@"Recorded unexpected number of notifications. 
Expected {Expected.Count} but was {Actual.Count}");
            }
            for (int i = 0; i < Expected.Count; i++)
            {
                Assert(Expected[i], Actual[i], i);
            }
        }

        private void Assert(Recorded<Notification<T>> expected, Recorded<Notification<T>> actual, int index)
        {
            if (expected.Time != actual.Time)
            {
                throw CreateError(
$@"Times for elements at index {index} do not match.
Expected event at {expected.Time} but something happened at {actual.Time}",
                        expected.Time, actual.Time);
            }
            var exVal = expected.Value;
            var acVal = actual.Value;
            var time = actual.Time;
            if (exVal.Kind != acVal.Kind)
            {
                throw CreateError(
$@"Types of elements at time {time} (index {index}) do not match.
Expected {exVal.Kind} but was {acVal.Kind}",
                        time);
            }
            if (exVal.Kind == NotificationKind.OnError && exVal.Exception.GetType() != acVal.Exception.GetType())
            {
                throw CreateError(
$@"Errors at time {time} (index {index}) do not match. 
Expected {exVal.Exception.GetType()} but was {acVal.Exception.GetType()}",
                        time);
            }
            if (exVal.Kind == NotificationKind.OnNext && !_comparer.Equals(exVal.Value, acVal.Value))
            {
                throw CreateError(
$@"Elements at time {time} (index {index}) do not match. 
Expected {exVal.Value} but was {acVal.Value}",
                        time);
            }
        }

        private ExpectObservableToBeFailedException CreateError(string message, long? expectedTime = null, long? actualTime = null)
        {
            var msg = $"ExpectObservable.ToBe failed.\n{message}.\n{GetDrawingMessage(expectedTime, actualTime)}";
            return new ExpectObservableToBeFailedException(msg);
        }

        private string GetDrawingMessage(long? expectedTime, long? actualTime)
        {
            var actualMarbles = TryGetActualMarbles();
            if (actualMarbles == null)
            {
                return "";
            }
            var drawing = $"Expected: \"{_expectedMarbles}\"\nActual:   \"{actualMarbles}\"";
            if (!expectedTime.HasValue)
            {
                return drawing;
            }
            if (!actualTime.HasValue)
            {
                actualTime = expectedTime;
            }
            var above = GetTimeMarker(expectedTime.Value, "↓");
            var below = GetTimeMarker(actualTime.Value, "↑");
            return string.Join("\n", above, drawing, below);
        }

        private string GetTimeMarker(long time, string arrow)
        {
            var position = (int)(time / MarbleScheduler.FrameTimeFactor);
            var spaces = new string(' ', 11 + position);
            return $"{spaces}{arrow} (time {time})";
        }

        private string TryGetActualMarbles()
        {
            return Marbles.TryGetMarblesAcceptingNullDict(Actual, Values?.ReverseKeyValue());
        }

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