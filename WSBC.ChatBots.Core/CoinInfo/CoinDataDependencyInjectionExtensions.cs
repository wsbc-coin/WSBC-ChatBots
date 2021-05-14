using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using WSBC.ChatBots.Coin;
using WSBC.ChatBots.Coin.Explorer;
using WSBC.ChatBots.Coin.Explorer.Services;
using WSBC.ChatBots.Coin.MiningPoolStats;
using WSBC.ChatBots.Coin.MiningPoolStats.Services;
using WSBC.ChatBots.Coin.Services;
using WSBC.ChatBots.Coin.TxBit;
using WSBC.ChatBots.Coin.TxBit.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CoinDataDependencyInjectionExtensions
    {
        public static IServiceCollection AddCoinData(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.Configure<CoinOptions>(_ => { });
            services.AddSingleton<IPostConfigureOptions<CoinOptions>, ConfigureCoinOptions>();

            services.AddTxBitClient();
            services.AddBlockchainExplorerClient();
            services.AddMiningPoolStatsClient();
            services.TryAddSingleton<ICoinDataProvider, CoinDataProvider>();

            return services;
        }

        public static IServiceCollection AddTxBitClient(this IServiceCollection services, 
            Action<TxBitOptions> configureOptions = null, IConfigurationSection configuration = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions != null)
                services.Configure(configureOptions);
            if (configuration != null)
                services.Configure<TxBitOptions>(configuration);

            services.AddHttpClient();
            services.TryAddTransient<ICoinDataClient<TxBitData>, TxBitDataClient>();

            return services;
        }

        public static IServiceCollection AddMiningPoolStatsClient(this IServiceCollection services,
            Action<MiningPoolStatsOptions> configureOptions = null, IConfigurationSection configuration = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions != null)
                services.Configure(configureOptions);
            if (configuration != null)
                services.Configure<MiningPoolStatsOptions>(configuration);

            services.AddHttpClient();
            services.TryAddTransient<ICoinDataClient<MiningPoolStatsData>, MiningPoolStatsDataClient>();

            return services;
        }

        public static IServiceCollection AddBlockchainExplorerClient(this IServiceCollection services,
            Action<ExplorerOptions> configureOptions = null, IConfigurationSection configuration = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions != null)
                services.Configure(configureOptions);
            if (configuration != null)
                services.Configure<ExplorerOptions>(configuration);

            services.AddHttpClient();
            services.TryAddTransient<IExplorerDataClient, ExplorerDataClient>();

            return services;
        }
    }
}
