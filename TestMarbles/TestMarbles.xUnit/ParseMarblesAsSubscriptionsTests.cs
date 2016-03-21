using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;

namespace TestMarbles.xUnit
{
    public class ParseMarblesAsSubscriptionsTests : ReactiveTest
    {
        [Fact]
        private void ParseMarblesAsSubscriptions_should_parse_subscription_marbles()
        {
            var result = Marbles.ToSubscription("---^---!-");
            result.Subscribe.Should().Be(30);
            result.Unsubscribe.Should().Be(70);
        }

        [Fact]
        // TODO FIX NAME IN JS
        private void ParseMarblesAsSubscriptions_should_parse_marbles_without_unsubscription()
        {
            var result = Marbles.ToSubscription("---^-");
            result.Subscribe.Should().Be(30);
            result.Unsubscribe.Should().Be(long.MaxValue);
        }

        [Fact]
        private void ParseMarblesAsSubscriptions_should_parse_marbles_with_instant_unsubscription()
        {
            var result = Marbles.ToSubscription("---(^!)-");
            result.Subscribe.Should().Be(30);
            result.Unsubscribe.Should().Be(30);
        }
    }
}