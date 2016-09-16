using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using TestMarbles.Utils;
using Xunit;

namespace TestMarbles.xUnit.MarbleSchedulerTests
{
    public class ExpectObservableTests
    {
        [Fact]
        public void ExpectObservable_should_work_for_observable_never()
        {
            using (var s = new MarbleScheduler())
            {
                s.ExpectObservable(Observable.Never<int>()).ToBe("-", Dict.Empty<int>());
            }
        }

        [Fact]
        public void ExpectObservable_should_work_for_observable_empty()
        {
            using (var s = new MarbleScheduler())
            {
                s.ExpectObservable(Observable.Empty<int>(s)).ToBe("|", Dict.Empty<int>());
            }
        }

        [Fact]
        public void ExpectObservable_should_work_for_observable_return()
        {
            using (var s = new MarbleScheduler())
            {
                s.ExpectObservable(Observable.Return('a', s)).ToBe("(a|)");
            }
        }

        [Fact]
        public void ExpectObservable_should_work_for_observable_throw()
        {
            using (var s = new MarbleScheduler())
            {
                s.ExpectObservable(Observable.Throw<char>(new Exception(), s)).ToBe("#");
            }
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
            using (var s = new MarbleScheduler())
            {
                var source = s.Hot("---^-a-b-|");
                var unsubscribe =     "---!";
                var expected =        "--a";
                s.ExpectObservable(source, unsubscribe).ToBe(expected);
            }
        }

        [Fact]
        public void ExpectObservable_should_support_testing_metastreams()
        {
            using (var s = new MarbleScheduler())
            {
                var x = s.Cold("-a-b|");
                var y = s.Cold("-c-d|");
                var myObservable = s.Hot("---x---y----|", Dict.Map('x', x, 'y', y));
                var expected = "---x---y----|";
                s.ExpectObservable(myObservable).ToBe(expected, Dict.Map('x', x, 'y', y));
            }
        }

        [Fact]
        public void EqualityComparer_can_be_provided_in_ToBe()
        {
            using (var s = new MarbleScheduler())
            {
                var veryCuriousEqualityComparer = 
                    new LambdaEqualityComparer<int>((a, b) => Math.Abs(a - b) == 1, x => 0);
                s.ExpectObservable(Observable.Return(1)).ToBe(
                    "(x|)",
                    Dict.Map('x', 2),
                    comparer: veryCuriousEqualityComparer);
            }
        }

        [Fact]
        public void ExpectObservable_should_handle_inner_observables()
        {
            using (var s = new MarbleScheduler())
            {
                var x = s.Cold(         "--a--b--c--d--e--|           ");
                var xsubs =    "         ^         !                  ";
                var y = s.Cold(                   "---f---g---h---i--|");
                var ysubs =    "                   ^                 !";
                var e1 = s.Hot("---------x---------y---------|        ");
                var e1subs =   "^                            !        ";
                var expected = "-----------a--b--c----f---g---h---i--|";
                var observableLookup = Dict.Map('x', x, 'y', y);
                var result = e1.Select(p => observableLookup[p]).Switch();
                s.ExpectObservable(result).ToBe(expected);
                s.ExpectSubscriptions(x.Subscriptions).ToBe(xsubs);
                s.ExpectSubscriptions(y.Subscriptions).ToBe(ysubs);
                s.ExpectSubscriptions(e1.Subscriptions).ToBe(e1subs);
            }
        }

        [Fact]
        public void ExpectObservable_should_materialize_inner_observables()
        {
            var s = new MarbleScheduler();
            
                var x1 = s.Cold("---a---b---|");
                var x2 = s.Cold("---a---b---|");
                var y = s.Hot("---x---", Dict.Map('x', x1));
                s.ExpectObservable(y).ToBe("---x---", Dict.Map('x', x2), comparer: new ObservableEqualityComparer<char>());
              
            s.Start();
        }

        private class LambdaEqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _comparer;
            private readonly Func<T, int> _hash;

            public LambdaEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hash)
            {
                _comparer = comparer;
                _hash = hash;
            }

            public bool Equals(T x, T y)
            {
                return _comparer(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _hash(obj);
            }
        }
    }
}
