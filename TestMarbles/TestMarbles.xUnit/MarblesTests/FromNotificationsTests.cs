using System;
using System.Collections.Generic;
using System.Reactive;
using Microsoft.Reactive.Testing;
using Xunit;

namespace TestMarbles.xUnit.MarblesTests
{
    public class FromNotificationsTests : ReactiveTest
    {
        [Fact]
        public void empty_is_parsed_correctly()
        {
            var n = new List<Recorded<Notification<char>>>();
            var actual = Marbles.FromNotifications(n);
            Assert.Equal("", actual);
        }

        [Fact]
        public void OnNext_is_parsed_correctly()
        {
            var n = new List<Recorded<Notification<char>>>
            {
                OnNext(0, 'a')
            };
            var actual = Marbles.FromNotifications(n);
            Assert.Equal("a", actual);
        }

        [Fact]
        public void OnError_is_parsed_correctly()
        {
            var n = new List<Recorded<Notification<char>>>
            {
                OnError<char>(0, new Exception())
            };
            var actual = Marbles.FromNotifications(n);
            Assert.Equal("#", actual);
        }

        [Fact]
        public void OnCompleted_is_parsed_correctly()
        {
            var n = new List<Recorded<Notification<char>>>
            {
                OnCompleted<char>(0)
            };
            var actual = Marbles.FromNotifications(n);
            Assert.Equal("|", actual);
        }

        [Fact]
        public void sequence_is_parsed_correctly()
        {
            var n = new List<Recorded<Notification<char>>>
            {
                OnNext(10, 'X'),
                OnNext(50, 'Y'),
                OnCompleted<char>(60)
            };
            var actual = Marbles.FromNotifications(n);
            Assert.Equal("-X---Y|", actual);
        }

        [Fact]
        public void groups_are_parsed_correctly_when_groups_contains_end()
        {
            var n = new List<Recorded<Notification<char>>>
            {
                OnNext(10, 'X'),
                OnNext(50, 'Y'),
                OnCompleted<char>(50)
            };
            var actual = Marbles.FromNotifications(n);
            Assert.Equal("-X---(Y|)", actual);
        }

        [Fact]
        public void undisplayable_overlapping_groups_throw()
        {
            var n = new List<Recorded<Notification<char>>>
            {
                OnNext(10, 'X'),
                OnNext(10, 'Y'),
                OnNext(20, 'K'),
                OnNext(50, 'Z'),
                OnCompleted<char>(50)
            };
            Action action = () => Marbles.FromNotifications(n);
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void groups_are_parsed_correctly()
        {
            var n = new List<Recorded<Notification<char>>>
            {
                OnNext(30, 'b'),
                OnNext(30, 'e'),
                OnNext(110, 'c'),
                OnNext(130, 'f'),
                OnCompleted<char>(190)
            };
            var actual = Marbles.FromNotifications(n);
            Assert.Equal("---(be)----c-f-----|", actual);
        }

        [Fact]
        public void parsing_notifications_with_unexpected_timestamps_throws()
        {
            var n = new List<Recorded<Notification<char>>>
            {
                OnNext(1, 'X')
            };
            Action action = () => Marbles.FromNotifications(n);
            Assert.Throws<ArgumentException>(action);
        }
    }
}
