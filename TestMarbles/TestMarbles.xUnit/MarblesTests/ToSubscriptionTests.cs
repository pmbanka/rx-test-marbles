using System;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;

namespace TestMarbles.xUnit.MarblesTests
{
    public class ToSubscriptionTests : ReactiveTest
    {
        [Fact]
        private void ToSubscription_should_parse_subscription_marbles()
        {
            var result = Marbles.ToSubscription("---^---!-");
            result.Subscribe.Should().Be(30);
            result.Unsubscribe.Should().Be(70);
        }

        [Fact]
        // TODO FIX NAME IN JS
        private void ToSubscription_should_parse_marbles_without_unsubscription()
        {
            var result = Marbles.ToSubscription("---^-");
            result.Subscribe.Should().Be(30);
            result.Unsubscribe.Should().Be(Subscription.Infinite);
        }

        [Fact]
        private void ToSubscription_should_parse_marbles_with_instant_unsubscription()
        {
            var result = Marbles.ToSubscription("---(^!)-");
            result.Subscribe.Should().Be(30);
            result.Unsubscribe.Should().Be(30);
        }

        [Fact]
        public void ToSubscription_should_throw_if_second_subscription_detected()
        {
            Action action = () => Marbles.ToSubscription("^^");
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void ToSubscription_should_throw_if_second_unsubscription_detected()
        {
            Action action = () => Marbles.ToSubscription("||");
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void ToSubscription_should_allow_marbles_without_both_subscription_nor_unsubscription()
        {
            var result = Marbles.ToSubscription("-");
            result.Subscribe.Should().Be(Subscription.Infinite);
            result.Unsubscribe.Should().Be(Subscription.Infinite);
        }

        [Fact]
        public void ToSubscription_should_allow_unsubscription_before_subscription()
        {
            var result = Marbles.ToSubscription("!-^");
            result.Subscribe.Should().Be(20);
            result.Unsubscribe.Should().Be(0);
        }
    }
}