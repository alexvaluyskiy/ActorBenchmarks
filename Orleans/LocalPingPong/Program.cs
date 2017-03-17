using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LocalPingPong
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is Server GC {GCSettings.IsServerGC}");

            var hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
            {
                AppDomainInitializer = InitSilo,
                AppDomainInitializerArguments = args,
            });

            DoClientWork().Wait();
            Console.ReadLine();
        }

        static void InitSilo(string[] args)
        {
            var siloHost = new SiloHost("SpawnSilo");

            siloHost.ConfigFileName = "OrleansConfiguration.xml";
            siloHost.InitializeOrleansSilo();

            var startedok = siloHost.StartOrleansSilo();
            if (!startedok)
                throw new SystemException(String.Format("Failed to start Orleans silo '{0}' as a {1} node", siloHost.Name, siloHost.Type));
        }

        private static async Task DoClientWork()
        {
            const int messageCount = 1000000;
            const int batchSize = 100;
            int[] clientCounts = new int[] { 1, 2, 4, 8 };

            var clientConfig = ClientConfiguration.LocalhostSilo(30000);
            GrainClient.Initialize(clientConfig);

            Console.WriteLine("Clients\t\tElapsed\t\tMsg/sec");

            foreach (var clientCount in clientCounts)
            {
                var clients = new IPingGrain[clientCount];
                var echos = new IPongGrain[clientCount];
                var results = new Task<bool>[clientCount];
                var observers = new IBenchmarkObserver[clientCount];
                for (var i = 0; i < clientCount; i++)
                {
                    clients[i] = GrainClient.GrainFactory.GetGrain<IPingGrain>(i);
                    echos[i] = GrainClient.GrainFactory.GetGrain<IPongGrain>(i + 10);

                    await clients[i].Init(echos[i], messageCount, batchSize);

                    var observer = new BenchmarkObserver();
                    observers[i] = observer;
                    await clients[i].Subscribe(GrainClient.GrainFactory.CreateObjectReference<IBenchmarkObserver>(observer).Result);
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

            Console.ReadLine();
        }
    }
}