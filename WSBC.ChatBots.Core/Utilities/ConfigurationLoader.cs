using Microsoft.Extensions.Configuration;

namespace WSBC.ChatBots.Utilities
{
    public static class ConfigurationLoader
    {
        public static IConfigurationBuilder LoadDefaults(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsecrets.json", optional: true, reloadOnChange: true);
            if (args != null)
                builder.AddCommandLine(args);
            return builder;
        }

        public static IConfiguration BuildDefault(string[] args)
            => LoadDefaults(args).Build();
    }
}
