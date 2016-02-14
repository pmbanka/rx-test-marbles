using System;
using System.Collections.Generic;
using System.Reactive;
using Microsoft.Reactive.Testing;
using Xunit;

namespace TestMarbles.xUnit
{
    public class ParseMarblesTests
    {
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
        public void ParseMarbles_should_parse_marble_string_with_subscription_marker()
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
        public void ParseMarbles_should_parse_marble_string_with_error_marker()
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
        public void ParseMarbles_should_default_to_letters_if_no_dictionary_provided()
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
        public void ParseMarbles_should_handle_grouped_values()
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
    }
}