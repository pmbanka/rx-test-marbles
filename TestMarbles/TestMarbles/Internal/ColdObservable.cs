using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Microsoft.Reactive.Testing;
using TestMarbles.Utils;

namespace TestMarbles.Internal
{
    // Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.
    internal class ColdObservable<T> : ITestableObservable<T>
    {
        private readonly VirtualTimeScheduler<long, long> _scheduler;

        public ColdObservable(VirtualTimeScheduler<long, long> scheduler, IEnumerable<Recorded<Notification<T>>> messages)
        {
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(messages, nameof(messages));
            _scheduler = scheduler;
            Messages = messages.ToList();
            Subscriptions = new List<Subscription>();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Ensure.NotNull(observer, nameof(observer));
            Subscriptions.Add(new Subscription(_scheduler.Clock));
            var index = Subscriptions.Count - 1;

            var d = new CompositeDisposable();

            foreach (var message in Messages)
            {
                var notification = message.Value;
                d.Add(_scheduler.ScheduleRelative(
                    default(object), 
                    message.Time, 
                    (_, __) => { notification.Accept(observer); return Disposable.Empty; }));
            }

            return Disposable.Create(() =>
            {
                Subscriptions[index] = new Subscription(Subscriptions[index].Subscribe, _scheduler.Clock);
                d.Dispose();
            });
        }

        public IList<Subscription> Subscriptions { get; }

        public IList<Recorded<Notification<T>>> Messages { get; }
    }
}