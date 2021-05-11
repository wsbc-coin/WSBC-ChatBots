using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WSBC.ChatBots.Token;
using WSBC.ChatBots.Token.DexGuru;
using WSBC.ChatBots.Token.DexGuru.Services;
using WSBC.ChatBots.Token.DexTrade;
using WSBC.ChatBots.Token.DexTrade.Services;
using WSBC.ChatBots.Token.PancakeSwap;
using WSBC.ChatBots.Token.Services;
using WSBC.ChatBots.Token.Stex;
using WSBC.ChatBots.Token.Stex.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TokenDataDependencyInjectionExtensions
    {
        public static IServiceCollection AddTokenData(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.Configure<TokenOptions>(_ => { });

            services.AddDexGuruClient();
            services.AddDexTradeClient();
            services.AddStexClient();
            services.TryAddSingleton<ITokenDataProvider, TokenDataProvider>();

            return services;
        }

        public static IServiceCollection AddDexGuruClient(this IServiceCollection services, 
            Action<DexGuruOptions> configureOptions = null, IConfigurationSection configuration = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions != null)
                services.Configure(configureOptions);
            if (configuration != null)
                services.Configure<DexGuruOptions>(configuration);

            services.Configure<TokenOptions>(_ => { });
            services.AddHttpClient();
            services.TryAddTransient<ITokenDataClient<DexGuruData>, DexGuruDataClient>();

            return services;
        }

        public static IServiceCollection AddDexTradeClient(this IServiceCollection services,
            Action<DexTradeOptions> configureOptions = null, IConfigurationSection configuration = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions != null)
                services.Configure(configureOptions);
            if (configuration != null)
                services.Configure<DexTradeOptions>(configuration);

            services.Configure<TokenOptions>(_ => { });
            services.AddHttpClient();
            services.TryAddTransient<ITokenDataClient<DexTradeData>, DexTradeDataClient>();

            return services;
        }

        public static IServiceCollection AddStexClient(this IServiceCollection services,
            Action<StexOptions> configureOptions = null, IConfigurationSection configuration = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions != null)
                services.Configure(configureOptions);
            if (configuration != null)
                services.Configure<StexOptions>(configuration);

            services.Configure<TokenOptions>(_ => { });
            services.AddHttpClient();
            services.TryAddTransient<ITokenDataClient<StexData>, StexDataClient>();

            return services;
        }

        public static IServiceCollection AddPancakeSwapClient(this IServiceCollection services,
            Action<PancakeSwapOptions> configureOptions = null, IConfigurationSection configuration = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions != null)
                services.Configure(configureOptions);
            if (configuration != null)
                services.Configure<PancakeSwapOptions>(configuration);

            services.Configure<TokenOptions>(_ => { });
            services.AddHttpClient();
            services.TryAddTransient<ITokenDataClient<PancakeSwapData>, PancakeSwapDataClient>();

            return services;
        }
    }
}
