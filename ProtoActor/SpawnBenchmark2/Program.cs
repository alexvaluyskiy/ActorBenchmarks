using System;
using System.Diagnostics;
using System.Runtime;
using Proto;

namespace SpawnBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is Server GC {GCSettings.IsServerGC}");

            var actor = Actor.Spawn(RootActor.Props);
            actor.Tell(new RootActor.Run(5));
            Console.ReadLine();
        }
    }
}