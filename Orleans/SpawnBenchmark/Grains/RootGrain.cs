using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SpawnBenchmark
{
    public interface IRootGrain : IGrainWithStringKey
    {
        Task Run(int number);

        Task Complete(long result);
    }

    public class RootGrain : Grain, IRootGrain
    {
        private int _currentRun;
        private long _elapsedMilliseconds;

        public async Task Run(int number)
        {
            await StartRun(number);
        }

        private Task StartRun(int n)
        {
            if (n == 0)
                return Task.CompletedTask;
                
            _currentRun = n;
            Console.WriteLine($"Start run {n}");

            var childGrain = this.GrainFactory.GetGrain<ISpawnGrain>("rootChild");
            childGrain.Initialize(this);
            _elapsedMilliseconds = Stopwatch.GetTimestamp();
            childGrain.Start(null, 7, 0).Ignore();
            
            return Task.CompletedTask;
        }

        public async Task Complete(long result)
        {
            var diff = TimeSpan.FromTicks(Stopwatch.GetTimestamp() - _elapsedMilliseconds);
            Console.WriteLine($"Run {_currentRun} result: {result} in {diff.TotalMilliseconds} ms");
            await Task.Delay(2000);
            await StartRun(_currentRun - 1);
        }
    }
}
