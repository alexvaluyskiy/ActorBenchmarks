using Microsoft.Extensions.Hosting;
using Orleans;
using System.Threading;
using System.Threading.Tasks;

namespace SpawnBenchmark
{
    public class SpawnClientHostedService : IHostedService
    {
        private readonly IClusterClient _client;

        public SpawnClientHostedService(IClusterClient client)
        {
            _client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var root = _client.GetGrain<IRootGrain>("root");

            await root.Run(1);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
