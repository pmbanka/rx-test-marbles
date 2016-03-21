using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;

namespace TestMarbles
{
    public class MarbleScheduler : VirtualTimeScheduler<long, long>, IDisposable
    {
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
            return Cold<char>(marbles, null, error);
        }

        public ITestableObservable<T> Cold<T>(
            string marbles,
            IReadOnlyDictionary<char, T> values = null,
            Exception error = null)
        {
            // TODO input validation
            var messages = Marbles.ToNotifications(marbles, values, error);
            return new ColdObservable<T>(this, messages);
        }

        public ITestableObservable<char> Hot(
            string marbles,
            Exception error = null)
        {
            return Hot<char>(marbles, null, error);
        }

        public ITestableObservable<T> Hot<T>(
            string marbles,
            IReadOnlyDictionary<char, T> values = null,
            Exception error = null)
        {
            // TODO input validation
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

        public ObservableToBe<T> ExpectObservable<T>(
            IObservable<T> observable,
            string unsubscriptionMarbles = null)
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
            _expectations.Add(expectation);
            return new ObservableToBe<T>(expectation);
        }

        public SubscriptionToBe ExpectSubscriptions(params Subscription[] subscriptions)
        {
            var test = new SubscriptionExpectation
            {
                Actual = subscriptions.ToList()
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