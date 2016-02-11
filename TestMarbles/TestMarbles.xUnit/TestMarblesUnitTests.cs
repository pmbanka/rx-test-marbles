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
        public void ParseMarbles_should_parse_marble_string_into_series_of_notifications()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                new Recorded<Notification<char>>(70, Notification.CreateOnNext('A')),
                new Recorded<Notification<char>>(110, Notification.CreateOnNext('B')),
                new Recorded<Notification<char>>(150, Notification.CreateOnCompleted<char>())
            };
            var actual = TestSchedulerEx.ParseMarbles(
                "-------a---b---|",
                new Dictionary<char, char> {{'a', 'A'}, {'b', 'B'}});
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseMarbles_should_parse_marble_string_allowing_spaces_at_the_end()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                new Recorded<Notification<char>>(20, Notification.CreateOnNext('A')),
                new Recorded<Notification<char>>(50, Notification.CreateOnNext('B')),
                new Recorded<Notification<char>>(80, Notification.CreateOnCompleted<char>())
            };
            var actual = TestSchedulerEx.ParseMarbles(
                "--a--b--|    ",
                new Dictionary<char, char> { { 'a', 'A' }, { 'b', 'B' } });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseMarble_should_parse_marble_string_with_subscription_marker()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                new Recorded<Notification<char>>(40, Notification.CreateOnNext('A')),
                new Recorded<Notification<char>>(80, Notification.CreateOnNext('B')),
                new Recorded<Notification<char>>(120, Notification.CreateOnCompleted<char>())
            };
            var actual = TestSchedulerEx.ParseMarbles(
                "---^---a---b---|",
                new Dictionary<char, char> { { 'a', 'A' }, { 'b', 'B' } });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseMarble_should_parse_marble_string_with_error_marker()
        {
            var error = new Exception("omg error!");
            var expected = new List<Recorded<Notification<char>>>
            {
                new Recorded<Notification<char>>(70, Notification.CreateOnNext('A')),
                new Recorded<Notification<char>>(110, Notification.CreateOnNext('B')),
                new Recorded<Notification<char>>(150, Notification.CreateOnError<char>(error))
            };
            var actual = TestSchedulerEx.ParseMarbles(
                "-------a---b---#",
                new Dictionary<char, char> { { 'a', 'A' }, { 'b', 'B' } },
                error);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseMarble_should_default_to_letters_if_no_dictionary_provided()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                new Recorded<Notification<char>>(20, Notification.CreateOnNext('a')),
                new Recorded<Notification<char>>(50, Notification.CreateOnNext('b')),
                new Recorded<Notification<char>>(80, Notification.CreateOnNext('c'))
            };
            var actual = TestSchedulerEx.ParseMarbles("--a--b--c--");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseMarble_should_handle_grouped_values()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                new Recorded<Notification<char>>(30, Notification.CreateOnNext('a')),
                new Recorded<Notification<char>>(30, Notification.CreateOnNext('b')),
                new Recorded<Notification<char>>(30, Notification.CreateOnNext('c'))
            };
            var actual = TestSchedulerEx.ParseMarbles("---(abc)---");
            Assert.Equal(expected, actual);
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