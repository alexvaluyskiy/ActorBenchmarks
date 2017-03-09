using Messages;
using Proto;
using Proto.Remote;
using System;
using System.Runtime;
using System.Threading;
using ProtosReflection = Messages.ProtosReflection;

namespace LocalPingPong
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is Server GC {GCSettings.IsServerGC}");

            Serialization.RegisterFileDescriptor(ProtosReflection.Descriptor);
            RemotingSystem.Start("127.0.0.1", 12001);

            var messageCount = 1000000;
            var wg = new AutoResetEvent(false);
            var props = Actor.FromProducer(() => new LocalActor(0, messageCount, wg));

            var pid = Actor.Spawn(props);
            var remote = new PID("127.0.0.1:12000", "remote");
            remote.RequestAsync<Start>(new StartRemote { Sender = pid }).Wait();

            var start = DateTime.Now;
            Console.WriteLine("Starting to send");
            var msg = new Ping();
            for (var i = 0; i < messageCount; i++)
            {
                remote.Tell(msg);
            }
            wg.WaitOne();
            var elapsed = DateTime.Now - start;
            Console.WriteLine("Elapsed {0}", elapsed);

            var t = messageCount * 2.0 / elapsed.TotalMilliseconds * 1000;
            Console.WriteLine("Throughput {0} msg / sec", t);

            Console.ReadLine();
        }
    }
}