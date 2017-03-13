using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;

namespace SpawnBenchmark
{
    class Program
    {

        static void Main(string[] args)
        {
            var hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
            {
                AppDomainInitializer = InitSilo,
                AppDomainInitializerArguments = args,
            });

            InitClient().Wait();
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

        private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim(false);

        static async Task InitClient() {
            var clientConfig = ClientConfiguration.LocalhostSilo(30000);
            GrainClient.Initialize(clientConfig);

            var root = GrainClient.GrainFactory.GetGrain<IRootGrain>("root");
            Console.WriteLine("Please ENTER to start a benchmark");
            Console.ReadLine();
            await root.Run(1);

            Console.ReadLine();
        }
    }
}