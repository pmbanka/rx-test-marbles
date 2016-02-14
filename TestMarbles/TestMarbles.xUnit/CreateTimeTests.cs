using System;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;

namespace TestMarbles.xUnit
{
    public class CreateTimeTests
    {
        public CreateTimeTests()
        {
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
    }
}