using System;
using System.Reactive;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;
using Xunit.Abstractions;
using System.Reactive.Concurrency;
using TestMarbles.Extensions;

namespace TestMarbles.xUnit
{
    public class Scratchpad : ReactiveTest
    {
        private readonly ITestOutputHelper _output;

        public Scratchpad(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestScheduler()
        {
            var s = new TestScheduler();
            var obs = Observable.Return('a');
            long actualClock = -1;
            s.ScheduleAbsolute(0, () => obs.Subscribe(_ => actualClock = s.Clock));
            s.Start();
            actualClock.Should().Be(1);
        }

        [Fact]
        public void TestScheduler2()
        {
            var s = new TestScheduler();
            var obs = s.CreateHotObservable(OnNext(0, 'a'), OnCompleted<char>(90));
            obs.Subscribe();
            s.Start();
            obs.Subscriptions.Should().HaveCount(1);
            obs.Subscriptions[0].Subscribe.Should().Be(0);
            obs.Subscriptions[0].Unsubscribe.Should().Be(Subscription.Infinite);
        }

        [Fact]
        public void Markers()
        {
            using (var s = new MarbleScheduler())
            {
                var e1 = s.Hot("-a");
                var e2 = s.Hot("--b");
                var expected = "-ab";
                s.ExpectObservable(e1.Merge(e2)).ToBe(expected);
            }
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