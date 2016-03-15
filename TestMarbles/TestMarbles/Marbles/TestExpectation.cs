using System.Collections.Generic;
using System.Reactive;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestMarbles
{
    internal abstract class TestExpectation
    {
        public bool Ready { get; set; }

        public abstract void Assert();
    }

    internal class ObservableExpectation<T> : TestExpectation
    {
        public List<Recorded<Notification<T>>> Actual { get; set; }

        public List<Recorded<Notification<T>>> Expected { get; set; }

        public override void Assert()
        {
            CollectionAssert.AreEqual(Expected, Actual, "TODO message");
        }
    }

    internal class SubscriptionExpectation : TestExpectation
    {
        public List<Subscription> Actual { get; set; }

        public List<Subscription> Expected { get; set; }

        public override void Assert()
        {
            CollectionAssert.AreEqual(Expected, Actual, "TODO message");
        }
    }
}