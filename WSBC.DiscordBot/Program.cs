using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using WSBC.DiscordBot.DataClients;
using WSBC.DiscordBot.Discord;
using WSBC.DiscordBot.TxBit;

namespace WSBC.DiscordBot
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

                // start Discord.NET client and commands handler
                ICommandHandler handler = services.GetRequiredService<ICommandHandler>();
                await handler.InitializeAsync().ConfigureAwait(false);
                WsbcDiscordClient client = services.GetRequiredService<WsbcDiscordClient>();
                await client.StartClientAsync().ConfigureAwait(false);

                // wait forever to prevent window closing
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

            // data aggregator
            services.AddTransient<ICoinDataProvider, CoinDataProvider>()
                .Configure<CachingOptions>(configuration.GetSection("Caching"));

            // Logging
            services.AddSingleton<ILoggerFactory>(new LoggerFactory()
                        .AddSerilog(Log.Logger, dispose: true));

            // Discord.NET
            services
                // - Client wrapper
                .AddSingleton<WsbcDiscordClient>()
                .AddSingleton<DiscordSocketClient>(s => s.GetRequiredService<WsbcDiscordClient>().Client)
                .AddSingleton<IDiscordClient>(s => s.GetRequiredService<DiscordSocketClient>())
                // - Command service
                .AddSingleton<ICommandHandler, SimpleCommandHandler>()
                // - Config
                .Configure<DiscordOptions>(configuration.GetSection("Discord"));

            // Clients
            services.AddHttpClient();
            // - TxBit
            services.AddTransient<ICoinDataClient<TxBitData>, TxBitDataClient>()
                .Configure<TxBitOptions>(configuration.GetSection("TxBit"));

            return services;
        }
    }
}
