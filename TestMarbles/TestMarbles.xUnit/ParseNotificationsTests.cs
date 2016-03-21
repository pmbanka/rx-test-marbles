using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Xunit;

namespace TestMarbles.xUnit
{
    public class ParseNotificationsTests : ReactiveTest
    {
        [Fact]
        public void empty_is_parsed_correctly()
        {
            var n = new List<Recorded<Notification<char>>>
            {  
            };
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
        public void mixed_is_parsed_correctly()
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
        public void groups_are_parsed_correctly()
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
    }
}
