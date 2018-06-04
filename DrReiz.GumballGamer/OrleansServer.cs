using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Orleans.Configuration;
using Orleans.Hosting;


namespace DrReiz.GumballGamer
{
    class OrleansServer
    {
        public static async Task Execute()
        {
            var host = await StartSilo();
            Console.WriteLine("Press any key to terminate...");
            Console.ReadKey();

            await host.StopAsync();


        }
        public static async Task<ISiloHost> StartSilo()
        {
            var builder = new SiloHostBuilder()
                  // Use localhost clustering for a single local silo
                  .UseLocalhostClustering()
                  // Configure ClusterId and ServiceId
                  .Configure<ClusterOptions>(options =>
                  {
                      options.ClusterId = "dev";
                      options.ServiceId = "MyAwesomeService";
                  })
              // Configure connectivity
              .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback);
                  // Configure logging with any logging framework that supports Microsoft.Extensions.Logging.
                  // In this particular case it logs using the Microsoft.Extensions.Logging.Console package.
                  //.ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }

    class OrleansGrain: Orleans.Grain, IGumballPing
    {
        public Task<string> SayPing(string msg)
        {
            return Task.FromResult($"Gumball ping response: {DateTime.UtcNow.Ticks}");
        }
    }

    public interface IGumballPing : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayPing(string msg);
    }
}
