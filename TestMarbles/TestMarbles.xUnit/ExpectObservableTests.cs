using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestMarbles.xUnit
{
    public class ExpectObservableTests
    {
        [Fact]
        public void ExpectObservable_should_work_for_observable_never()
        {
            var s = new MarbleScheduler();
            s.ExpectObservable(Observable.Never<int>()).ToBe("-");
            s.Start();
        }

        [Fact]
        public void ExpectObservable_should_work_for_observable_empty()
        {
            var s = new MarbleScheduler();
            s.ExpectObservable(Observable.Empty<int>(s)).ToBe("|");
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
            var ex = new Exception();
            s.ExpectObservable(Observable.Throw<char>(ex, s)).ToBe("#", error: ex);
            s.Start();
        }
    }
}
