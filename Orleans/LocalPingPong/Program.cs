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

namespace LocalPingPong
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Is Server GC {GCSettings.IsServerGC}");
            DoClientWork().Wait();
            Console.ReadLine();
        }

        private static void InitSilo()
        {
            var siloHost = new SiloHost("PingPongTest");

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
            int[] clientCounts = new int[] { 1, 2, 4, 8, 16 };

            InitSilo();

            var clientConfig = ClientConfiguration.LocalhostSilo(3000);
            GrainClient.Initialize(clientConfig);

            Console.WriteLine("Clients\t\tElapsed\t\tMsg/sec");

            foreach (var clientCount in clientCounts)
            {
                var clients = new IPingGrain[clientCount];
                var echos = new IPongGrain[clientCount];
                var completions = new TaskCompletionSource<bool>[clientCount];

                for (var i = 0; i < clientCount; i++)
                {
                    var tsc = new TaskCompletionSource<bool>();
                    completions[i] = tsc;

                    clients[i] = GrainClient.GrainFactory.GetGrain<IPingGrain>(Guid.NewGuid());
                    echos[i] = GrainClient.GrainFactory.GetGrain<IPongGrain>(Guid.NewGuid());
                }

                var tasks = completions.Select(tsc => tsc.Task).ToArray();
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < clientCount; i++)
                {
                    var client = clients[i];
                    var echo = echos[i];

                    await client.Initialize(echo, 1);

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

        private static void InitializeWithRetries(ClientConfiguration config, int initializeAttemptsBeforeFailing)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    GrainClient.Initialize(config);
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (SiloUnavailableException)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
        }
    }
}