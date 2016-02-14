using FluentAssertions;
using Xunit;

namespace TestMarbles.xUnit
{
    public class ParseMarblesAsSubscriptionsTests
    {
        [Fact]
        private void ParseMarblesAsSubscriptions_should_parse_subscription_marbles()
        {
            var result = TestSchedulerEx.ParseMarblesAsSubscriptions("---^---!-");
            result.SubscribedFrame.Should().Be(30);
            result.UnsubscribedFrame.Should().Be(70);
        }

        [Fact]
        // TODO FIX NAME IN JS
        private void ParseMarblesAsSubscriptions_should_parse_marbles_without_unsubscription()
        {
            var result = TestSchedulerEx.ParseMarblesAsSubscriptions("---^-");
            result.SubscribedFrame.Should().Be(30);
            result.UnsubscribedFrame.Should().Be(long.MaxValue);
        }

        [Fact]
        private void ParseMarblesAsSubscriptions_should_parse_marbles_with_instant_unsubscription()
        {
            var result = TestSchedulerEx.ParseMarblesAsSubscriptions("---(^!)-");
            result.SubscribedFrame.Should().Be(30);
            result.UnsubscribedFrame.Should().Be(30);
        }
    }
}