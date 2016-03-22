using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public void ExpectObservable_should_work_for_merged_observable()
        {
            using (var s = new MarbleScheduler())
            {
                var e1 = s.Hot("----a--^--b-------c--|");
                var e2 = s.Hot(  "---d-^--e---------f-----|");
                var expected =        "---(be)----c-f-----|";
                s.ExpectObservable(e1.Merge(e2)).ToBe(expected);
            }
        }

        [Fact]
        public void ExpectObservable_should_work_for_zipped_observable()
        {
            using (var s = new MarbleScheduler())
            {
                var dict = Enumerable.Range(0, 10)
                    .Zip(Enumerable.Range('0' + 0, 10).Select(p => (char) p), Tuple.Create)
                    .ToDictionary(k => k.Item2, v => v.Item1);
                var e1 = s.Hot("----1--^--2--3-----0--|", dict);
                var e2 = s.Hot(  "---2-^--1---2--1----9--|", dict);
                var expected =        "---3---5----1--|";
                s.ExpectObservable(e1.Zip(e2, (a, b) => a + b)).ToBe(expected, dict);
            }
        }

        [Fact]
        public void ExpectObservable_should_work_for_combine_latest_observable()
        {
            using (var s = new MarbleScheduler())
            {
                var e1 = s.Cold("---a--b----|");
                var e2 = s.Cold("--c----d----|");
                var expected =  "---x--yz----|";
                var dict = new Dictionary<char, string>
                {
                    {'x', "ac"},
                    {'y', "bc"},
                    {'z', "bd"}
                };
                s.ExpectObservable(e1.CombineLatest(e2, (a, b) => a.ToString() + b.ToString()))
                    .ToBe(expected, dict);
            }
        }

        [Fact]
        public void ExpectObservable_should_accept_unsubscription_marble_diagram()
        {
            var s = new MarbleScheduler();
            var source = s.Hot("---^-a-b-|");
            var unsubscribe =     "---!";
            var expected =        "--a";
            s.ExpectObservable(source, unsubscribe).ToBe(expected);
        }
    }
    
}
