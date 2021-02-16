using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
        }
    }
}
