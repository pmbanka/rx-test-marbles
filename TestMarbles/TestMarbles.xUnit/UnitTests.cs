using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Xunit;
using Xunit.Abstractions;

namespace TestMarbles.xUnit
{
    public class UnitTests
    {
        private readonly ITestOutputHelper _output;

        public UnitTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Hot()
        {
            var scheduler = new TestScheduler();
            var hot = scheduler.CreateHotObservable("---a---b---|");
            hot.Subscribe(
                o => _output.WriteLine($"{o}"),
                ex => _output.WriteLine(ex.Message),
                () => _output.WriteLine("End"));
            scheduler.Start();
            _output.WriteLine("Time " + scheduler.Now.Ticks);
            _output.WriteLine("Cnt " + hot.Messages.Count);
        }

        [Fact]
        public void HotDict()
        {
            var s = new TestScheduler();
            var dict = new Dictionary<char, int>
            {
                {'a', 1 },
                {'b', 2 }
            };
            var hot = s.CreateHotObservable("---a---b---|", dict);
            hot.Subscribe(
                o => _output.WriteLine($"{o}"),
                ex => _output.WriteLine(ex.Message),
                () => _output.WriteLine("End"));
            s.Start();
        }

        [Fact]
        public void Cold()
        {
            var scheduler = new TestScheduler();
            var cold = scheduler.CreateColdObservable("---#");
            var n = Notification.CreateOnNext(cold.Messages);
            cold.Subscribe(
                o => _output.WriteLine($"{o}"),
                ex => _output.WriteLine(ex.Message),
                () => _output.WriteLine("End"));
            scheduler.Start();

            _output.WriteLine(scheduler.Now.Ticks.ToString());
        }
    }
}
