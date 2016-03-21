using System;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;

namespace TestMarbles.xUnit
{
    public class CreateTimeTests
    {
        [Fact]
        public void ToCompletionTime_should_parse_simple_marbles_to_time()
        {
            var actual = Marbles.ToCompletionTime("-----|");
            actual.Should().Be(50);
        }

        [Fact]
        public void ToCompletionTime_should_throw_if_input_without_end_marker()
        {
            Action action = () => Marbles.ToCompletionTime("--a--b-#");
            action.ShouldThrow<ArgumentException>();
        }
    }
}