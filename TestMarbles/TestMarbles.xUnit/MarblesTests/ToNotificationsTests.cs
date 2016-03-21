using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Microsoft.Reactive.Testing;
using Xunit;

namespace TestMarbles.xUnit.MarblesTests
{
    public class ToNotificationsTests : ReactiveTest
    {
        [Fact]
        public void ToNotifications_should_parse_observable_empty()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                OnCompleted<char>(0),
            };
            var actual = Marbles.ToNotifications("|");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToNotifications_should_parse_observable_never()
        {
            var expected = Enumerable.Empty<Recorded<Notification<char>>>();
            var actual = Marbles.ToNotifications("");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToNotifications_should_parse_observable_return()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                OnNext(0, 'a'),
                OnCompleted<char>(0)
            };
            var actual = Marbles.ToNotifications("(a|)");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToNotifications_should_parse_observable_throw()
        {
            var ex = new Exception();
            var expected = new List<Recorded<Notification<char>>>
            {
                OnError<char>(0, ex),
            };
            var actual = Marbles.ToNotifications("#", ex);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToNotifications_should_parse_marble_string_into_series_of_notifications()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                OnNext(70, 'A'),
                OnNext(110, 'B'),
                OnCompleted<char>(150)
            };
            var actual = Marbles.ToNotifications(
                "-------a---b---|",
                new Dictionary<char, char> {{'a', 'A'}, {'b', 'B'}});
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToNotifications_should_parse_marble_string_allowing_spaces_at_the_end()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                OnNext(20, 'A'),
                OnNext(50, 'B'),
                OnCompleted<char>(80)
            };
            var actual = Marbles.ToNotifications(
                "--a--b--|    ",
                new Dictionary<char, char> { { 'a', 'A' }, { 'b', 'B' } });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToNotifications_should_parse_marble_string_with_subscription_marker()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                OnNext(40, 'A'),
                OnNext(80, 'B'),
                OnCompleted<char>(120)
            };
            var actual = Marbles.ToNotifications(
                "---^---a---b---|",
                new Dictionary<char, char> { { 'a', 'A' }, { 'b', 'B' } });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToNotifications_should_parse_marble_string_with_error_marker()
        {
            var error = new Exception("omg error!");
            var expected = new List<Recorded<Notification<char>>>
            {
                OnNext(70, 'A'),
                OnNext(110, 'B'),
                OnError<char>(150,error)
            };
            var actual = Marbles.ToNotifications(
                "-------a---b---#",
                new Dictionary<char, char> { { 'a', 'A' }, { 'b', 'B' } },
                error);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToNotifications_should_default_to_letters_if_no_dictionary_provided()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                OnNext(20, 'a'),
                OnNext(50, 'b'),
                OnNext(80, 'c')
            };
            var actual = Marbles.ToNotifications("--a--b--c--");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToNotifications_should_handle_grouped_values()
        {
            var expected = new List<Recorded<Notification<char>>>
            {
                OnNext(30, 'a'),
                OnNext(30, 'b'),
                OnNext(30, 'c')
            };
            var actual = Marbles.ToNotifications("---(abc)---");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToNotifications_allows_setting_symbols_with_a_dictionary()
        {
            var expected = new List<Recorded<Notification<int>>>
            {
                OnNext(0, 99),
                OnNext(30, 1),
                OnNext(50, 2),
                OnNext(50, 1),
                OnCompleted<int>(90)
            };
            var dict = new Dictionary<char, int>
            {
                { '1', 1 },
                { '2', 2 },
                { 'X', 99 }
            };
            var actual = Marbles.ToNotifications("X--1-(21)|", dict);
            Assert.Equal(expected, actual);
        }
    }
}