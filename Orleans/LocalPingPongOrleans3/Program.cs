using Orleans;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Runtime;
using LocalPingPongOrleans3;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LocalPingPong
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"Is Server GC {GCSettings.IsServerGC}");

            var host = new HostBuilder()
                .ConfigureHostConfiguration(config => config
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
                .UseOrleans(builder =>
                {
                    builder
                        .UseLocalhostClustering()
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "HelloWorldApp";
                        })
                        .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                        .AddMemoryGrainStorage(name: "ArchiveStorage");
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<PingPongClientHostedService>();

                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging(builder =>
                {
                    //builder.AddConsole();
                });

            await host.RunConsoleAsync();

            Console.ReadLine();
        }
    }
}