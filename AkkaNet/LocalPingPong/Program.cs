using Akka.Actor;
using Akka.Configuration;
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

            const int messageCount = 1000000;
            const int batchSize = 100;
            int[] clientCounts = new int[] { 1, 2, 4, 8, 16 };

            Console.WriteLine("Clients\t\tElapsed\t\tMsg/sec");

            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=on");

            foreach (var clientCount in clientCounts)
            {
                var clients = new IActorRef[clientCount];
                var echos = new IActorRef[clientCount];
                var completions = new TaskCompletionSource<bool>[clientCount];

                using (var sys = ActorSystem.Create("main", config))
                {
                    for (var i = 0; i < clientCount; i++)
                    {
                        var tsc = new TaskCompletionSource<bool>();
                        completions[i] = tsc;
                        var clientProps = PingActor.Props(tsc, messageCount, batchSize);

                        clients[i] = sys.ActorOf(clientProps);
                        echos[i] = sys.ActorOf(PongActor.Props);
                    }

                    var tasks = completions.Select(tsc => tsc.Task).ToArray();
                    var sw = Stopwatch.StartNew();
                    for (var i = 0; i < clientCount; i++)
                    {
                        var client = clients[i];
                        var echo = echos[i];

                        client.Tell(new Start(echo));
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