using System;
using System.Diagnostics;
using System.Linq;
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

            Console.WriteLine("Clients\t\tElapsed\t\tMsg/sec");

            foreach (var clientCount in clientCounts)
            {
                var clients = new PID[clientCount];
                var echos = new PID[clientCount];

                for (var i = 0; i < clientCount; i++)
                {
                    clients[i] = Actor.Spawn(PingActor.Props(messageCount, batchSize));
                    echos[i] = Actor.Spawn(PongActor.Props);
                }

                var tasks = new Task[clientCount];
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < clientCount; i++)
                {
                    var client = clients[i];
                    var echo = echos[i];

                    tasks[i] = client.RequestAsync<bool>(new PingActor.Start(echo));
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
