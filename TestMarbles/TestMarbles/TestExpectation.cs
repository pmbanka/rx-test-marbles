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
        public ObservableExpectation()
        {
            Actual = new List<Recorded<Notification<T>>>();
            Expected = new List<Recorded<Notification<T>>>();
        }

        public List<Recorded<Notification<T>>> Actual { get; }

        public List<Recorded<Notification<T>>> Expected { get; }

        public override void Assert()
        {
            CollectionAssert.AreEqual(Expected, Actual, "TODO message");
        }
    }

    internal class SubscriptionExpectation : TestExpectation
    {
        public SubscriptionExpectation()
        {
            Actual = new List<Subscription>();
            Expected = new List<Subscription>();
        }

        public List<Subscription> Actual { get; set; }

        public List<Subscription> Expected { get; set; }

        public override void Assert()
        {
            CollectionAssert.AreEqual(Expected, Actual, "TODO message");
        }
    }
}