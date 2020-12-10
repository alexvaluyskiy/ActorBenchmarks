using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
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

            string serializer = args.FirstOrDefault(arg => arg.StartsWith("--serializer="))?.Replace("--serializer=", "");

            const int messageCount = 1000000;
            const int batchSize = 100;
            var clientCounts = new HashSet<int>() {1, 2, 4, 8, 12};

            Console.WriteLine("Clients\t\tElapsed\t\tMsg/sec");

            var config = ConfigurationFactory.ParseString(@"akka.suppress-json-serializer-warning=on");

            if (!string.IsNullOrEmpty(serializer))
            {
                Console.WriteLine($"Used {serializer} serializer");
                config = config.WithFallback(ConfigurationFactory.ParseString(@"
                    akka.actor.serialize-messages = on
                    akka.actor.serializers.protobuf = ""LocalPingPong.Serializers.ProtobufSerializer, LocalPingPong""
                    akka.actor.serializers.msgpack = ""LocalPingPong.Serializers.MsgPackSerializer, LocalPingPong""
                    akka.actor.serializers.hyperion = ""LocalPingPong.Serializers.HyperionSerializer, LocalPingPong""
                    akka.actor.serializers.bond = ""LocalPingPong.Serializers.BondSerializer, LocalPingPong""
                "));

                if (serializer.Equals("protobuf")) {
                    config = config.WithFallback(ConfigurationFactory.ParseString(@"akka.actor.serialization-bindings.""LocalPingPong.Msg, LocalPingPong"" = protobuf"));
                }
                else if (serializer.Equals("msgpack")) {
                    config = config.WithFallback(ConfigurationFactory.ParseString(@"akka.actor.serialization-bindings.""LocalPingPong.Msg, LocalPingPong"" = msgpack"));
                }
                else if (serializer.Equals("bond")) {
                    config = config.WithFallback(ConfigurationFactory.ParseString(@"akka.actor.serialization-bindings.""LocalPingPong.Msg, LocalPingPong"" = bond"));
                }
            }

            foreach (var clientCount in clientCounts.OrderBy(x => x))
            {
                var clients = new IActorRef[clientCount];
                var echos = new IActorRef[clientCount];

                using (var sys = ActorSystem.Create("main", config))
                {
                    for (var i = 0; i < clientCount; i++)
                    {
                        clients[i] = sys.ActorOf(PingActor.Props(messageCount, batchSize));
                        echos[i] = sys.ActorOf(PongActor.Props);
                    }

                    var tasks = new Task[clientCount];
                    var sw = Stopwatch.StartNew();
                    for (var i = 0; i < clientCount; i++)
                    {
                        var client = clients[i];
                        var echo = echos[i];

                        tasks[i] = client.Ask<bool>(new Start(echo));
                    }
                    Task.WaitAll(tasks);
                    sw.Stop();

                    var totalMessages = messageCount * 2 * clientCount;
                    var x = (int)(totalMessages / (double)sw.ElapsedMilliseconds * 1000.0d);
                    Console.WriteLine($"{clientCount}\t\t{sw.ElapsedMilliseconds}\t\t{x}");
                }

                Thread.Sleep(2000);
            }

            Console.ReadLine();
        }
    }
}