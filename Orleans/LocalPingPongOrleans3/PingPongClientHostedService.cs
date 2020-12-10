using LocalPingPong;
using Microsoft.Extensions.Hosting;
using Orleans;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LocalPingPongOrleans3
{
    public class PingPongClientHostedService : IHostedService
    {
        private readonly IClusterClient _client;

        public PingPongClientHostedService(IClusterClient client)
        {
            this._client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            const int messageCount = 1000000;
            const int batchSize = 100;
            int[] clientCounts = new int[] { 1, 2, 4, 8, 12 };

            Console.WriteLine("Clients\t\tElapsed\t\tMsg/sec");

            foreach (var clientCount in clientCounts)
            {
                var clients = new IPingGrain[clientCount];
                var echos = new IPongGrain[clientCount];
                var results = new Task<bool>[clientCount];
                var observers = new IBenchmarkObserver[clientCount];
                for (var i = 0; i < clientCount; i++)
                {
                    clients[i] = this._client.GetGrain<IPingGrain>(i);
                    echos[i] = this._client.GetGrain<IPongGrain>(i + clientCount);

                    await clients[i].Init(echos[i], messageCount, batchSize);

                    var observer = new BenchmarkObserver();
                    observers[i] = observer;
                    await clients[i].Subscribe(this._client.CreateObjectReference<IBenchmarkObserver>(observer).Result);
                    results[i] = observer.AsTask();
                }

                var sw = Stopwatch.StartNew();
                for (var i = 0; i < clientCount; i++)
                {
                    var client = clients[i];
                    var echo = echos[i];

                    client.Start().Ignore();
                }
                Task.WaitAll(results);

                sw.Stop();
                var totalMessages = messageCount * 2 * clientCount;
                var x = (int)(totalMessages / (double)sw.ElapsedMilliseconds * 1000.0d);
                Console.WriteLine($"{clientCount}\t\t{sw.ElapsedMilliseconds}\t\t{x}");

                Thread.Sleep(2000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
