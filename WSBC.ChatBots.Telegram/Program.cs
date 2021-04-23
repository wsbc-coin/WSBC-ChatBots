using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
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

                // TODO: start Telegram.Bot client and commands handler

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

            // Logging
            services.AddSerilogLogging();

            // TODO: Telegram.Bot

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
