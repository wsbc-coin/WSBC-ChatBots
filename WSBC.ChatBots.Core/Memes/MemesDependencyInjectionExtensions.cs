using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WSBC.ChatBots.Memes;
using WSBC.ChatBots.Memes.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MemesDependencyInjectionExtensions
    {
        public static IServiceCollection AddMemes(this IServiceCollection services,
            Action<MemesOptions> configureOptions = null, IConfigurationSection configuration = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions != null)
                services.Configure(configureOptions);
            if (configuration != null)
                services.Configure<MemesOptions>(configuration);

            services.TryAddTransient<IRandomFilePicker, RandomFilePicker>();

            return services;
        }
    }
}
