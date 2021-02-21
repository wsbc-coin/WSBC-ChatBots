using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WSBC.DiscordBot.Discord.Services
{
    /// <summary>Wrapper for Discord.NET's client to make it work nicely with DI.</summary>
    class WsbcDiscordClient : IDisposable
    {
        public DiscordSocketClient Client { get; }

        private readonly ILogger _log;
        private readonly IOptionsMonitor<DiscordOptions> _discordOptions;

        public WsbcDiscordClient(IOptionsMonitor<DiscordOptions> discordOptions, ILogger<WsbcDiscordClient> log)
        {
            this._discordOptions = discordOptions;
            this._log = log;

            DiscordSocketConfig clientConfig = new DiscordSocketConfig();
            clientConfig.LogLevel = LogSeverity.Verbose;
            this.Client = new DiscordSocketClient(clientConfig);
            this.Client.Log += OnClientLog;

            this._discordOptions.OnChange(async _ =>
            {
                if (Client.ConnectionState == ConnectionState.Connected || Client.ConnectionState == ConnectionState.Connecting)
                {
                    _log.LogInformation("Options changed, reconnecting client");
                    await StopClientAsync().ConfigureAwait(false);
                    await StartClientAsync().ConfigureAwait(false);
                }
            });
        }

        public async Task StartClientAsync()
        {
            await this.Client.LoginAsync(TokenType.Bot, _discordOptions.CurrentValue.BotToken).ConfigureAwait(false);
            await this.Client.StartAsync().ConfigureAwait(false);
        }

        public async Task StopClientAsync()
        {
            if (this.Client.LoginState == LoginState.LoggedIn || this.Client.LoginState == LoginState.LoggingIn)
                await this.Client.LogoutAsync().ConfigureAwait(false);
            if (this.Client.ConnectionState == ConnectionState.Connected || this.Client.ConnectionState == ConnectionState.Connecting)
                await this.Client.StopAsync().ConfigureAwait(false);
        }

        private Task OnClientLog(LogMessage message)
        {
            this._log.Log(message);
            return Task.CompletedTask;
        }

        public static implicit operator DiscordSocketClient(WsbcDiscordClient client)
            => client.Client;

        public void Dispose()
        {
            try { this.Client.Log -= OnClientLog; } catch { }
            try { this.Client?.Dispose(); } catch { }
        }
    }
}
