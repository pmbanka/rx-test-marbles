using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reactive.Testing;

namespace TestMarbles.Utils
{
    public class ObservableEqualityComparer<T> : IEqualityComparer<ITestableObservable<T>>
    {
        public bool Equals(ITestableObservable<T> x, ITestableObservable<T> y)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }
            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }
            return Enumerable.SequenceEqual(x.Messages, y.Messages)
                && Enumerable.SequenceEqual(x.Subscriptions, y.Subscriptions);
        }

        public int GetHashCode(ITestableObservable<T> obj)
        {
            return 0; // ¯\_(ツ)_/¯
        }
    }
}