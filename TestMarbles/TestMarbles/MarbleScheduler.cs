using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;

namespace TestMarbles
{
    public class MarbleScheduler : VirtualTimeScheduler<long, long>, IDisposable
    {
        public const long FrameTimeFactor = 10;

        protected override long Add(long absolute, long relative) => absolute + relative;

        protected override DateTimeOffset ToDateTimeOffset(long absolute) => new DateTimeOffset(absolute, TimeSpan.Zero);

        protected override long ToRelative(TimeSpan timeSpan) => timeSpan.Ticks;

        private readonly List<TestExpectation> _expectations;

        private bool _expectationsChecked;

        public MarbleScheduler()
        {
            _expectations = new List<TestExpectation>();
        }

        public ITestableObservable<char> Cold(
            string marbles,
            Exception error = null)
        {
            Ensure.NotNull(marbles, nameof(marbles));
            Ensure.ValidColdObservable(marbles, nameof(marbles));
            var messages = Marbles.ToNotifications(marbles, error);
            return new ColdObservable<char>(this, messages);
        }

        public ITestableObservable<T> Cold<T>(
            string marbles,
            IReadOnlyDictionary<char, T> values,
            Exception error = null)
        {
            Ensure.NotNull(marbles, nameof(marbles));
            Ensure.NotNull(values, nameof(values));
            Ensure.ValidColdObservable(marbles, nameof(marbles));
            Ensure.NotContainsMarkers(values, nameof(values));
            var messages = Marbles.ToNotifications(marbles, values, error);
            return new ColdObservable<T>(this, messages);
        }

        public ITestableObservable<char> Hot(
            string marbles,
            Exception error = null)
        {
            Ensure.NotNull(marbles, nameof(marbles));
            var messages = Marbles.ToNotifications(marbles, error);
            return new HotObservable<char>(this, messages);
        }

        public ITestableObservable<T> Hot<T>(
            string marbles,
            IReadOnlyDictionary<char, T> values,
            Exception error = null)
        {
            Ensure.NotNull(marbles, nameof(marbles));
            Ensure.NotNull(values, nameof(values));
            Ensure.NotContainsMarkers(values, nameof(values));
            var messages = Marbles.ToNotifications(marbles, values, error);
            return new HotObservable<T>(this, messages);
        }

        public new void Start()
        {
            if (_expectationsChecked)
            {
                throw new InvalidOperationException("Cannot check expectations more than once in a unit test.");
            }
            base.Start();
            foreach (var expectation in _expectations.Where(e => e.Ready))
            {
                expectation.Assert();
            }
            _expectationsChecked = true;
        }

        public ObservableToBe ExpectObservable(
            IObservable<char> observable,
            string unsubscriptionMarbles = null)
        {
            Ensure.NotNull(observable, nameof(observable));
            var expectation = CreateExpectation(observable, unsubscriptionMarbles);
            _expectations.Add(expectation);
            return new ObservableToBe(expectation);
        }

        public ObservableToBe<T> ExpectObservable<T>(
            IObservable<T> observable,
            string unsubscriptionMarbles = null)
        {
            Ensure.NotNull(observable, nameof(observable));
            var expectation = CreateExpectation(observable, unsubscriptionMarbles);
            _expectations.Add(expectation);
            return new ObservableToBe<T>(expectation);
        }

        private ObservableExpectation<T> CreateExpectation<T>(IObservable<T> observable, string unsubscriptionMarbles)
        {
            var expectation = new ObservableExpectation<T>();
            var unsubscriptionFrame = unsubscriptionMarbles != null
                ? Marbles.ToSubscription(unsubscriptionMarbles).Unsubscribe
                : Subscription.Infinite;
            IDisposable disposable = null;
            this.ScheduleAbsolute(0, () =>
            {
                disposable = observable.Subscribe(
                    x =>
                    {
                        // TODO handle observable-of-observable
                        expectation.Actual.Add(new Recorded<Notification<T>>(Clock, Notification.CreateOnNext(x)));
                    },
                    err => expectation.Actual.Add(new Recorded<Notification<T>>(Clock, Notification.CreateOnError<T>(err))),
                    () => expectation.Actual.Add(new Recorded<Notification<T>>(Clock, Notification.CreateOnCompleted<T>())));
            });
            if (unsubscriptionFrame != Subscription.Infinite)
            {
                this.ScheduleAbsolute(unsubscriptionFrame, () => disposable.Dispose());
            }
            return expectation;
        }

        public SubscriptionToBe ExpectSubscription(Subscription subscription)
        {
            Ensure.NotNull(subscription, nameof(subscription));
            var test = new SubscriptionExpectation
            {
                Actual = subscription
            };
            _expectations.Add(test);
            return new SubscriptionToBe(test);
        }

        void IDisposable.Dispose()
        {
            if (!_expectationsChecked)
            {
                Start();
            }
        }
    }
}