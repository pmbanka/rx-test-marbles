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

namespace TestMarbles.xUnit
{
    public class TestMarblesUnitTests
    {
        private readonly ITestOutputHelper _output;

        public TestMarblesUnitTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CrateTime_should_parse_simple_marbles_to_time()
        {
            var scheduler = new TestScheduler();
            var actual = scheduler.CreateTime("-----|");
            actual.Should().Be(50);
        }

        [Fact]
        public void CreteTime_should_throw_if_input_without_end_marker()
        {
            var scheduler = new TestScheduler();
            Action action = () => scheduler.CreateTime("--a--b-#");
            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void CreateColdObservable_should_create_cold_observable()
        {
            var expected = new List<char> {'A', 'B'};
            var scheduler = new TestScheduler();
            var source = scheduler.CreateColdObservable(
                "--a---b--|",
                new Dictionary<char, char> {{'a', 'A'}, {'b', 'B'}});
            source.Subscribe(p =>
            {
                p.Should().Be(expected[0]);
                expected.RemoveAt(0);
            });
            scheduler.Start();
            expected.Should().BeEmpty();
        }

        [Fact]
        public void CreateHotObservable_should_create_hot_observable()
        {
            var expected = new List<char> {'A', 'B'};
            var scheduler = new TestScheduler();
            var source = scheduler.CreateHotObservable(
                "--a---b--|",
                new Dictionary<char, char> {{'a', 'A'}, {'b', 'B'}});
            source.Subscribe(p =>
            {
                p.Should().Be(expected[0]);
                expected.RemoveAt(0);
            });
            scheduler.Start();
            expected.Should().BeEmpty();
        }

        // DEBUG
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