using System;
using System.Linq;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using Xunit;
using Xunit.Abstractions;

namespace TestMarbles.xUnit.MarbleSchedulerTests
{
    public class ExpectSubscriptionTests
    {
        [Fact]
        public void ExpectSubscriptions_should_work_for_empty_subscription()
        {
            using (var s = new MarbleScheduler())
            {
                s.ExpectSubscriptions(new Subscription(Subscription.Infinite)).ToBe("");
            }
        }

        [Fact]
        public void ExpectSubscriptions_should_work_for_subscription_at_zero()
        {
            using (var s = new MarbleScheduler())
            {
                s.ExpectSubscriptions(new Subscription(0)).ToBe("^");
            }
        }

        [Fact]
        public void ExpectSubscriptions_should_work_for_subscription_not_at_zero()
        {
            using (var s = new MarbleScheduler())
            {
                s.ExpectSubscriptions(new Subscription(20)).ToBe("--^");
            }
        }

        [Fact]
        public void ExpectSubscriptions_should_work_for_subscription_and_unsubscription()
        {
            using (var s = new MarbleScheduler())
            {
                s.ExpectSubscriptions(new Subscription(20, 40)).ToBe("--^-!");
            }
        }

        [Fact]
        public void ExpectSubscriptions_should_work_for_grouped_subscription_and_unsubscription()
        {
            using (var s = new MarbleScheduler())
            {
                s.ExpectSubscriptions(new Subscription(20, 20)).ToBe("--(^!)");
            }
        }

        [Fact]
        public void ExpectSubscriptions_should_throw_on_wrong_markers()
        {
            Action a = () =>
            {
                using (var s = new MarbleScheduler())
                {
                    s.ExpectSubscriptions(new Subscription(20, 20)).ToBe("a-(^!)");
                }
            };
            Assert.Throws<ArgumentException>(a);
        }

        [Fact]
        public void ExpectSubscriptions_can_be_called_multiple_times_in_single_test()
        {
            using (var s = new MarbleScheduler())
            {
                s.ExpectSubscriptions(new Subscription(20, 20)).ToBe("--(^!)");
                s.ExpectSubscriptions(new Subscription(0, 0)).ToBe("(^!)");
                s.ExpectSubscriptions(new Subscription(0, 10)).ToBe("^!");
            }
        }

        [Fact]
        public void incorrect_marbles_should_result_in_test_failure()
        {
            Action a = () =>
            {
                using (var s = new MarbleScheduler())
                {
                    s.ExpectSubscriptions(new Subscription(20, 40)).ToBe("^---------!");
                }
            };
            Assert.Throws<ExpectSubscriptionToBeFailedException>(a);
        }

        [Fact]
        public void FactMethodName()
        {
            using (var s = new MarbleScheduler())
            {
                var source = s.Cold("---a---b-|");
                var subs =          "^--!";
                var d = source.Subscribe();
                s.ScheduleAbsolute(30, () => d.Dispose());
                s.ExpectSubscriptions(source.Subscriptions).ToBe(subs);
            }
        }
    }
}