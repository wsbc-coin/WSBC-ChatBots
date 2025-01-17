﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WSBC.ChatBots.Coin;
using WSBC.ChatBots.Telegram.Autopost;
using WSBC.ChatBots.Telegram.Services;
using WSBC.ChatBots.Token;
using WSBC.ChatBots.Utilities;

namespace WSBC.ChatBots.Telegram
{
    class Program
    {
        private static IServiceProvider _services;

        static async Task Main(string[] args)
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

                // start Telegram.Bot client
                ITelegramClient telegramClient = _services.GetRequiredService<ITelegramClient>();
                telegramClient.Start();

                // init autoposting
                _services.GetRequiredService<IAutopostService>();

                // init all commands - until a proper commands system is in place each handler needs to be triggered manually
                _services.GetRequiredService<Commands.TokenCheckCommands>();
                _services.GetRequiredService<Commands.MemesCommands>();
                _services.GetRequiredService<Commands.DebugOnlyCommands>();

                // submit commands so they're listed
                ICommandsHandler commandsHandler = _services.GetRequiredService<ICommandsHandler>();
                await commandsHandler.SubmitCommandsAsync().ConfigureAwait(false);

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

            // Telegram.Bot
            services.AddSingleton<ITelegramClient, WsbcTelegramClient>()
                .AddSingleton<ICommandsHandler, CommandsHandler>()
                .Configure<TelegramOptions>(configuration.GetSection("Telegram"))
                // commands
                .AddTransient<Commands.TokenCheckCommands>()
                .AddTransient<Commands.MemesCommands>()
                .AddTransient<Commands.DebugOnlyCommands>();

            // Token Data
            services
                .AddDexGuruClient(configuration: tokenSection.GetSection("DexGuru"))
                .AddDexTradeClient(configuration: tokenSection.GetSection("DexTrade"))
                .AddStexClient(configuration: tokenSection.GetSection("Stex"))
                .AddPancakeSwapClient(configuration: tokenSection.GetSection("PancakeSwap"))
                .AddTokenData();

            // Coin Data
            services
                .AddTxBitClient(configuration: coinSection.GetSection("TxBit"))
                .AddMiningPoolStatsClient(configuration: coinSection.GetSection("MiningPoolStats"))
                .AddBlockchainExplorerClient(configuration: coinSection.GetSection("Explorer"))
                .AddCoinData();

            // Memes feature
            services.AddMemes(configuration: configuration.GetSection("Memes"));

            // Autopost feature
            services.AddSingleton<IAutopostService, AutopostService>()
                .Configure<AutopostOptions>(configuration.GetSection("Autopost"));

            // Price formatting
            services.AddTransient<PriceFormatProvider>();

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
