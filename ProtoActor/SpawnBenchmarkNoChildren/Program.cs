using Proto;
using System;
using System.Diagnostics;
using System.Runtime;

namespace SpawnBenchmarkNoChildren
{
    public class Program
    {
        private static void Main()
        {
            Console.WriteLine($"Is Server GC {GCSettings.IsServerGC}");
            var sw = Stopwatch.StartNew();

            for (int n = 5; n > 0; n--)
            {
                Console.WriteLine($"Start run {n}");
                var start = sw.ElapsedMilliseconds;

                var pid = Actor.Spawn(SpawnActor.Props);
                var t = pid.RequestAsync<long>(new SpawnRequest(div: 10, num: 0, size: 1000000));
                t.ConfigureAwait(false);
                var x = t.Result;

                var diff = sw.ElapsedMilliseconds - start;
                Console.WriteLine($"Run {n} result: {x} in {diff} ms");
            }

            Console.ReadLine();
        }
    }
}