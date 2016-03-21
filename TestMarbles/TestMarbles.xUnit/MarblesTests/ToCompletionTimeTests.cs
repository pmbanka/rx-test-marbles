using System;
using FluentAssertions;
using Xunit;

namespace TestMarbles.xUnit.MarblesTests
{
    public class ToCompletionTimeTests
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