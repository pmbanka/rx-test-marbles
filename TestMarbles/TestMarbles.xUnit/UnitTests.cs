using System;
using System.Diagnostics;
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
            var observable = scheduler.CreateHotObservable<char>("      ---a---b---|");
            observable.Subscribe(
                o => _output.WriteLine($"{o}"),
                ex => _output.WriteLine(ex.Message),
                () => _output.WriteLine("End"));
            scheduler.Start();
            _output.WriteLine("Time " + scheduler.Now.Ticks);
            _output.WriteLine("Cnt " + observable.Messages.Count);
        }

        [Fact]
        public void Cold()
        {
            var scheduler = new TestScheduler();
            var observable = scheduler.CreateColdObservable<char>("---#");
            observable.Subscribe(
                o => _output.WriteLine($"{o}"),
                ex => _output.WriteLine(ex.Message),
                () => _output.WriteLine("End"));
            scheduler.Start();
            _output.WriteLine(scheduler.Now.Ticks.ToString());
        }
    }
}
