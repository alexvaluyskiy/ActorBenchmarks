using System;
using System.Runtime;
using Proto;

namespace SpawnBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is Server GC {GCSettings.IsServerGC}");
            var system = new ActorSystem();

            var actor = system.Root.Spawn(Props.FromProducer(() => new RootActor()));
            system.Root.Send(actor, new RootActor.Run(5));
            Console.ReadLine();
        }
    }
}