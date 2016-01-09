using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Xunit;
using Xunit.Abstractions;

namespace TestMarbles.xUnit
{
    public class HotObservableUnitTests
    {
        private readonly ITestOutputHelper _output;

        public HotObservableUnitTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void EmptySequenceGivesObsNever()
        {
            var scheduler = new TestScheduler();
            var observable = scheduler.CreateHotObservable<int>("-");
            observable.Subscribe(
                o => _output.WriteLine($"{o}"),
                ex => _output.WriteLine(ex.Message),
                () => _output.WriteLine("End"));
            scheduler.Start();
            _output.WriteLine(scheduler.Now.ToString());
        }
    }
}
