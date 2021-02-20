using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
                // other startup logic goes here

                await Task.Delay(-1).ConfigureAwait(false);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
