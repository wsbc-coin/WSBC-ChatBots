using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WSBC.ChatBots.Discord.Services;
using WSBC.ChatBots.Coin;
using WSBC.ChatBots.Utilities;
using WSBC.ChatBots.Token;

namespace WSBC.ChatBots.Discord
{
    class Program
    {
        private static IServiceProvider _services;

        private static async Task Main(string[] args)
        {
            // handle exiting
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => OnExit();

            // build configuration
            IConfiguration config = ConfigurationLoader.BuildDefault(args);

            // enable logging
            Logging.ConfigureLogging(config);

            try
            {
                // prepare DI container
                IServiceCollection serviceCollection = ConfigureServices(config);
                _services = serviceCollection.BuildServiceProvider();

                // start Discord.NET client and commands handler
                ICommandHandler handler = _services.GetRequiredService<ICommandHandler>();
                await handler.InitializeAsync().ConfigureAwait(false);
                WsbcDiscordClient client = _services.GetRequiredService<WsbcDiscordClient>();
                await client.StartClientAsync().ConfigureAwait(false);

                // wait forever to prevent window closing
                await Task.Delay(-1).ConfigureAwait(false);
            }
            finally
            {
                OnExit();
            }
        }

        private static IServiceCollection ConfigureServices(IConfiguration configuration)
        {
            IServiceCollection services = new ServiceCollection();

            // global coin options
            IConfigurationSection coinSection = configuration.GetSection("Coin");
            services.Configure<CoinOptions>(coinSection);
            IConfigurationSection tokenSection = configuration.GetSection("Token");
            services.Configure<TokenOptions>(tokenSection);

            // Logging
            services.AddSerilogLogging();

            // Discord.NET
            services
                // - Client wrapper
                .AddSingleton<WsbcDiscordClient>()
                .AddSingleton<DiscordSocketClient>(s => s.GetRequiredService<WsbcDiscordClient>().Client)
                .AddSingleton<IDiscordClient>(s => s.GetRequiredService<DiscordSocketClient>())
                // - Command service
                .AddSingleton<ICommandHandler, SimpleCommandHandler>()
                // - Embed builder
                .AddTransient<ICoinDataEmbedBuilder, CoinDataEmbedBuilder>()
                // - Config
                .Configure<DiscordOptions>(configuration.GetSection("Discord"));

            // Token Data
            services
                .AddDexGuruClient(configuration: tokenSection.GetSection("DexGuru"))
                .AddDexTradeClient(configuration: tokenSection.GetSection("DexTrade"))
                .AddTokenData();

            // Coin Data
            services
                .AddTxBitClient(configuration: coinSection.GetSection("TxBit"))
                .AddMiningPoolStatsClient(configuration: coinSection.GetSection("MiningPoolStats"))
                .AddBlockchainExplorerClient(configuration: coinSection.GetSection("Explorer"))
                .AddCoinData();

            // Memes feature
            services.AddMemes(configuration: configuration.GetSection("Memes"));

            return services;
        }

        private static void OnExit()
        {
            try { Log.CloseAndFlush(); } catch { }
            try
            {
                if (_services is IDisposable disposableServices)
                    disposableServices.Dispose();
            }
            catch { }
        }
    }
}
