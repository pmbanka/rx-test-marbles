using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;

namespace TestMarbles.xUnit
{
    public class CreateObservableTests
    {
        [Fact]
        public void CreateColdObservable_should_create_cold_observable()
        {
            var expected = new List<char> {'A', 'B'};
            var scheduler = new TestScheduler();
            var source = scheduler.CreateColdObservable(
                "--a---b--|",
                new Dictionary<char, char> {{'a', 'A'}, {'b', 'B'}});
            source.Subscribe(p =>
            {
                p.Should().Be(expected[0]);
                expected.RemoveAt(0);
            });
            scheduler.Start();
            expected.Should().BeEmpty();
        }

        [Fact]
        public void CreateHotObservable_should_create_hot_observable()
        {
            var expected = new List<char> {'A', 'B'};
            var scheduler = new TestScheduler();
            var source = scheduler.CreateHotObservable(
                "--a---b--|",
                new Dictionary<char, char> {{'a', 'A'}, {'b', 'B'}});
            source.Subscribe(p =>
            {
                p.Should().Be(expected[0]);
                expected.RemoveAt(0);
            });
            scheduler.Start();
            expected.Should().BeEmpty();
        }
    }
}