using System;
using Microsoft.Reactive.Testing;
using Xunit;

namespace TestMarbles.xUnit.MarbleSchedulerTests
{
    public class StartTests
    {
        [Fact]
        public void Start_can_be_called_at_most_once()
        {
            Action a = () =>
            {
                var s = new MarbleScheduler();
                s.Start();
                s.Start();
            };
            Assert.Throws<InvalidOperationException>(a);
        }

        [Fact]
        public void Dispose_can_be_called_even_if_start_was_called()
        {
            using (var s = new MarbleScheduler())
            {
                s.Start();
            }
        }

        [Fact]
        public void Dispose_allows_for_not_calling_start()
        {
            using (var s = new MarbleScheduler())
            {
                s.ExpectSubscription(new Subscription()).ToBe("(^!)");
            }
        }
    }
}