using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WSBC.Discord
{
    class Program
    {
        static async Task Main(string[] args)
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
                IServiceCollection services = new ServiceCollection()
                    .AddSingleton<ILoggerFactory>(new LoggerFactory()
                        .AddSerilog(Log.Logger, dispose: true));

                await Task.Delay(-1).ConfigureAwait(false);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
