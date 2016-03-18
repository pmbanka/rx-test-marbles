using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarbles
{
    public class MarbleScheduler : IDisposable
    {
        private readonly TestScheduler _scheduler;
        private readonly List<TestExpectation> _expectations;
        private bool _expectationsChecked;

        public IScheduler Scheduler => _scheduler;

        public MarbleScheduler(TestScheduler scheduler)
        {
            _scheduler = scheduler;
            _expectations = new List<TestExpectation>();
        }

        public MarbleScheduler() : this(new TestScheduler()) { }

        public ITestableObservable<char> Cold(
            string marbles,
            Exception error = null)
        {
            return _scheduler.CreateColdObservable(marbles, error);
        }

        public ITestableObservable<T> Cold<T>(
            string marbles,
            IReadOnlyDictionary<char, T> values = null,
            Exception error = null)
        {
            return _scheduler.CreateColdObservable(marbles, values, error);
        }

        public ITestableObservable<char> Hot(
            string marbles,
            Exception error = null)
        {
            return _scheduler.CreateHotObservable(marbles, error);
        }

        public ITestableObservable<T> Hot<T>(
            string marbles,
            IReadOnlyDictionary<char, T> values = null,
            Exception error = null)
        {
            return _scheduler.CreateHotObservable(marbles, values, error);
        }

        public void Start()
        {
            if (_expectationsChecked)
            {
                throw new InvalidOperationException("Cannot check expectations more than once in a unit test.");
            }
            _scheduler.Start();
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
                ? TestSchedulerEx.ParseMarblesAsSubscriptions(unsubscriptionMarbles).Unsubscribe
                : Subscription.Infinite;
            IDisposable disposable = null;
            _scheduler.ScheduleAbsolute(0, () =>
            {
                disposable = observable.Subscribe(
                    x =>
                    {
                        // TODO handle observable-of-observable
                        expectation.Actual.Add(new Recorded<Notification<T>>(_scheduler.Clock, Notification.CreateOnNext(x)));
                    },
                    err => expectation.Actual.Add(new Recorded<Notification<T>>(_scheduler.Clock, Notification.CreateOnError<T>(err))),
                    () => expectation.Actual.Add(new Recorded<Notification<T>>(_scheduler.Clock, Notification.CreateOnCompleted<T>())));
            });
            if (unsubscriptionFrame != Subscription.Infinite)
            {
                _scheduler.ScheduleAbsolute(unsubscriptionFrame, () => disposable.Dispose());
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