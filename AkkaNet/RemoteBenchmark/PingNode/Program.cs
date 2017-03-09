using Akka.Actor;
using Akka.Configuration;
using PongNode.Actors;
using PongNode.Messages;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace LocalPingPong
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is Server GC {GCSettings.IsServerGC}");

            var config = ConfigurationFactory.ParseString(@"
                akka {  
                    log-config-on-start = on
                    suppress-json-serializer-warning=on

                    actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    }
                    remote {
                        helios.tcp {
		                    port = 12001
		                    hostname = localhost
                        }
                    }
                }
            ");

            using (var system = ActorSystem.Create("RemoteBenchmark", config))
            {
                var messageCount = 1000000;
                var wg = new AutoResetEvent(false);

                var props = Props.Create(() => new PingActor(0, messageCount, wg));
                var localActor = system.ActorOf(props, "local");

                var remoteAddress = Address.Parse("akka.tcp://RemoteBenchmark@localhost:12000");
                var remoteProps = Props.Create<PongActor>()
                    .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress)));
                var remoteActor = system.ActorOf(remoteProps, "remoteecho");

                remoteActor.Ask<Start>(new StartRemote(localActor)).Wait();

                Console.ReadLine();

                var start = DateTime.Now;
                Console.WriteLine("Starting to send");
                var msg = new Ping();
                for (var i = 0; i < messageCount; i++)
                {
                    remoteActor.Tell(msg);
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
}

