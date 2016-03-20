using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;
using Xunit.Abstractions;
using System.Reactive.Concurrency;

namespace TestMarbles.xUnit
{
    public class DebugUnitTests : ReactiveTest
    {
        private readonly ITestOutputHelper _output;

        public DebugUnitTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestScheduler()
        {
            var s = new TestScheduler();
            var obs = Observable.Return('a');
            s.ScheduleAbsolute(0, () => obs.Subscribe(_ => _output.WriteLine(s.Clock.ToString())));
            s.Start();
        }

        [Fact]
        public void Ensure()
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
        public void Hot()
        {
            var scheduler = new TestScheduler();
            var hot = scheduler.CreateHotObservable("---a---b---|");

            hot.Subscribe(
                o => _output.WriteLine($"{o}"),
                ex => _output.WriteLine(ex.Message),
                () => _output.WriteLine("End"));
            scheduler.Start();
            _output.WriteLine("Time " + scheduler.Now.Ticks);
            _output.WriteLine("Cnt " + hot.Messages.Count);
            // scheduler.Start()
        }

        [Fact]
        public void Cold()
        {
            var scheduler = new TestScheduler();
            var cold = scheduler.CreateColdObservable("---#");
            var n = Notification.CreateOnNext(cold.Messages);
            cold.Subscribe(
                o => _output.WriteLine($"{o}"),
                ex => _output.WriteLine(ex.Message),
                () => _output.WriteLine("End"));
            scheduler.Start();

            _output.WriteLine(scheduler.Now.Ticks.ToString());
        }
    }
}