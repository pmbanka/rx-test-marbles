using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;

namespace TestMarbles
{
    public static class TestSchedulerExtensions
    {
        public static ITestableObservable<T> CreateHotObservable<T>(
            this TestScheduler scheduler,
            string sequence,
            IDictionary<string, T> values = null,
            string error = null)
        {
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));
            if (string.IsNullOrWhiteSpace(sequence)) throw new ArgumentException("Cannot be either null, empty, nor whitespace only.", nameof(sequence));
            var input = sequence.Trim();
            var events = new List<Recorded<Notification<T>>>();
            return scheduler.CreateHotObservable(Enumerable.Empty<Recorded<Notification<T>>>().ToArray());
        }
    }
}
