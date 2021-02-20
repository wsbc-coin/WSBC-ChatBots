using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using WSBC.Discord.DataClients;
using WSBC.Discord.TxBit;

namespace WSBC.Discord
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            // build configuration
            IConfiguration config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsecrets.json", optional: true)
                .AddCommandLine(args)
                .Build();

            // enable logging
            Logging.ConfigureLogging(config);

            try
            {
                // prepare DI container
                IServiceCollection serviceCollection = ConfigureServices(config);
                IServiceProvider services = serviceCollection.BuildServiceProvider();

                await Task.Delay(-1).ConfigureAwait(false);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IServiceCollection ConfigureServices(IConfiguration configuration)
        {
            IServiceCollection services = new ServiceCollection();

            // global coin options
            services.Configure<WsbcOptions>(configuration);

            // Logging
            services.AddSingleton<ILoggerFactory>(new LoggerFactory()
                        .AddSerilog(Log.Logger, dispose: true));

            // Clients
            services.AddHttpClient();
            // - TxBit
            services.AddTransient<ICoinDataClient<TxBitData>, TxBitDataClient>()
                .Configure<TxBitOptions>(configuration.GetSection("TxBit"));

            return services;
        }
    }
}
