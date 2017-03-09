using Proto;
using System;
using System.Diagnostics;
using System.Runtime;
using System.Threading.Tasks;

namespace SpawnBenchmark
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine($"Is Server GC {GCSettings.IsServerGC}");

            var pid = Actor.Spawn(SpawnActor.Props);
            var sw = Stopwatch.StartNew();
            var t = pid.RequestAsync<long>(new SpawnRequest(div: 10, num: 0, size: 1000000));
            t.ConfigureAwait(false);
            var res = t.Result;
            Console.WriteLine(sw.Elapsed);
            Console.WriteLine(res);
            Console.ReadLine();
        }
    }
}