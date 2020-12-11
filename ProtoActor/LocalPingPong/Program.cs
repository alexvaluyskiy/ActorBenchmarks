using System;
using System.Diagnostics;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Proto;

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

            var system = new ActorSystem();

            Console.WriteLine("Clients\t\tElapsed\t\tMsg/sec");

            foreach (var clientCount in clientCounts)
            {
                var clients = new PID[clientCount];
                var echos = new PID[clientCount];

                for (var i = 0; i < clientCount; i++)
                {
                    clients[i] = system.Root.Spawn(Props.FromProducer(() => new PingActor(messageCount, batchSize)));
                    echos[i] = system.Root.Spawn(Props.FromProducer(() => new PongActor()));
                }

                var tasks = new Task[clientCount];
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < clientCount; i++)
                {
                    var client = clients[i];
                    var echo = echos[i];

                    tasks[i] = system.Root.RequestAsync<bool>(client, new PingActor.Start(echo));
                }
                Task.WaitAll(tasks);
                sw.Stop();

                var totalMessages = messageCount * 2 * clientCount;
                var x = (int)(totalMessages / (double)sw.ElapsedMilliseconds * 1000.0d);
                Console.WriteLine($"{clientCount}\t\t{sw.ElapsedMilliseconds}\t\t{x}");

                Thread.Sleep(2000);
            }

            Console.ReadLine();
        }
    }
}
