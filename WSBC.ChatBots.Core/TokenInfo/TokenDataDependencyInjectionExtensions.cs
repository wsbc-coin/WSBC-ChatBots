using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WSBC.ChatBots.Token;
using WSBC.ChatBots.Token.DexGuru;
using WSBC.ChatBots.Token.DexGuru.Services;
using WSBC.ChatBots.Token.Services;

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
    }
}
