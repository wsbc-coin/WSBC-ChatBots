﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using WSBC.ChatBots.Discord.Services;
using WSBC.ChatBots.Coin.Explorer;
using WSBC.ChatBots.Coin.Explorer.Services;
using WSBC.ChatBots.Memes;
using WSBC.ChatBots.Memes.Services;
using WSBC.ChatBots.Coin.MiningPoolStats;
using WSBC.ChatBots.Coin.MiningPoolStats.Services;
using WSBC.ChatBots.Coin.Services;
using WSBC.ChatBots.Coin.TxBit;
using WSBC.ChatBots.Coin.TxBit.Services;
using WSBC.ChatBots.Coin;

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
            services.Configure<WsbcOptions>(configuration);

            // data aggregator
            services.AddSingleton<ICoinDataProvider, CoinDataProvider>()
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
                // - Embed builder
                .AddTransient<ICoinDataEmbedBuilder, CoinDataEmbedBuilder>()
                // - Config
                .Configure<DiscordOptions>(configuration.GetSection("Discord"));

            // Clients
            services.AddHttpClient();
            // - TxBit
            services.AddTransient<ICoinDataClient<TxBitData>, TxBitDataClient>()
                .Configure<TxBitOptions>(configuration.GetSection("TxBit"));
            // - Explorer
            services.AddTransient<IExplorerDataClient, ExplorerDataClient>()
                .Configure<ExplorerOptions>(configuration.GetSection("Explorer"));
            // - MiningPoolStats
            services.AddTransient<ICoinDataClient<MiningPoolStatsData>, MiningPoolStatsDataClient>()
                .Configure<MiningPoolStatsOptions>(configuration.GetSection("MiningPoolStats"));

            // Memes feature
            services.AddTransient<IRandomFilePicker, RandomFilePicker>()
                .Configure<MemesOptions>(configuration.GetSection("Memes"));

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