using System;
using Proto;
using Proto.Remote;
using ProtosReflection = Messages.ProtosReflection;
using System.Runtime;

namespace PongNode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is Server GC {GCSettings.IsServerGC}");

            Serialization.RegisterFileDescriptor(ProtosReflection.Descriptor);
            RemotingSystem.Start("127.0.0.1", 12000);
            Actor.SpawnNamed(Actor.FromProducer(() => new PongActor()), "remote");
            Console.ReadLine();
        }
    }
}