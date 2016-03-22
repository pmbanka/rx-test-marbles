using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Xunit;

namespace TestMarbles.xUnit.MarbleSchedulerTests
{
    public class ExpectObservableTests
    {
        [Fact]
        public void ExpectObservable_should_work_for_observable_never()
        {
            var s = new MarbleScheduler();
            s.ExpectObservable(Observable.Never<int>()).ToBe("-", new Dictionary<char, int>());
            s.Start();
        }

        [Fact]
        public void ExpectObservable_should_work_for_observable_empty()
        {
            var s = new MarbleScheduler();
            s.ExpectObservable(Observable.Empty<int>(s)).ToBe("|", new Dictionary<char, int>());
            s.Start();
        }

        [Fact]
        public void ExpectObservable_should_work_for_observable_return()
        {
            var s = new MarbleScheduler();
            s.ExpectObservable(Observable.Return('a', s)).ToBe("(a|)");
            s.Start();
        }

        [Fact]
        public void ExpectObservable_should_work_for_observable_throw()
        {
            var s = new MarbleScheduler();
            s.ExpectObservable(Observable.Throw<char>(new Exception(), s)).ToBe("#");
            s.Start();
        }
    }
}
