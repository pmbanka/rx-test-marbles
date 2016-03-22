using System.Collections.Generic;
using System.Reactive;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;

namespace TestMarbles
{
    // Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.
    internal class HotObservable<T> : ITestableObservable<T>
    {
        private readonly VirtualTimeScheduler<long, long> _scheduler;
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();

        public HotObservable(VirtualTimeScheduler<long, long> scheduler, IEnumerable<Recorded<Notification<T>>> messages)
        {
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(messages, nameof(messages));
            _scheduler = scheduler;
            Messages = messages.ToList();
            Subscriptions = new List<Subscription>();
            foreach (var message in Messages)
            {
                var notification = message.Value;
                scheduler.ScheduleAbsolute(default(object), message.Time, (_, __) =>
                {
                    var observers = _observers.ToArray();
                    foreach (var observer in observers)
                    {
                        notification.Accept(observer);
                    }
                    return Disposable.Empty;
                });
            }
        }

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            _observers.Add(observer);
            Subscriptions.Add(new Subscription(_scheduler.Clock));
            var index = Subscriptions.Count - 1;

            return Disposable.Create(() =>
            {
                _observers.Remove(observer);
                Subscriptions[index] = new Subscription(Subscriptions[index].Subscribe, _scheduler.Clock);
            });
        }

        public IList<Subscription> Subscriptions { get; }

        public IList<Recorded<Notification<T>>> Messages { get; }
    }
}