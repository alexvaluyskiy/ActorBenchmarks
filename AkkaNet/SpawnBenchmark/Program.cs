using Akka.Actor;
using Akka.Configuration;
using System;
using System.Diagnostics;
using System.Runtime;

namespace SpawnBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is Server GC {GCSettings.IsServerGC}");

            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=on");
            using (var sys = ActorSystem.Create("main", config))
            {
                Console.ReadLine();
                var actor = sys.ActorOf(SkynetFast.Props);
                var sw = Stopwatch.StartNew();
                var res = actor.Ask<long>(new SpawnRequest(div: 10, num: 0, size: 1000000)).GetAwaiter().GetResult();
                sw.Stop();
                Console.WriteLine(res);
                Console.WriteLine(sw.Elapsed);
                Console.ReadLine();
            }
        }
    }
}