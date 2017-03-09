using Akka.Actor;
using Akka.Configuration;
using PongNode.Actors;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace PongNode
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
		                    port = 12000
		                    hostname = localhost
                        }
                    }
                }
            ");

            using (var system = ActorSystem.Create("RemoteBenchmark", config))
            {
                Console.ReadLine();
            }
        }
    }
}