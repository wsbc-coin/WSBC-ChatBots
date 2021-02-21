using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WSBC.DiscordBot.Discord
{
    /// <summary>Handler that allows use of Discord.NET's default commands system.</summary>
    // taken from https://github.com/TehGM/EinherjiBot 
    // and added some modifications since Einherji uses Hosting extensions, while this doesn't
    class SimpleCommandHandler : IDisposable, ICommandHandler
    {
        private readonly DiscordSocketClient _client;
        private CommandService _commands;
        private readonly IOptionsMonitor<DiscordOptions> _options;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _log;
        private readonly IDisposable _optionsChangeRegistration;

        public SimpleCommandHandler(IServiceProvider serviceProvider, DiscordSocketClient client, IOptionsMonitor<DiscordOptions> options, ILogger<SimpleCommandHandler> log)
        {
            this._client = client;
            this._options = options;
            this._serviceProvider = serviceProvider;
            this._log = log;

            this._optionsChangeRegistration = this._options.OnChange(async _ => await InitializeAsync());
            this._client.MessageReceived += HandleCommandAsync;
        }

        public async Task InitializeAsync()
        {
            _log.LogDebug("Initializing CommandService");

            try { (_commands as IDisposable)?.Dispose(); } catch { }

            DiscordOptions options = this._options.CurrentValue;
            CommandServiceConfig config = new CommandServiceConfig();
            config.CaseSensitiveCommands = options.CaseSensitive;
            if (options.DefaultRunMode != RunMode.Default)
                config.DefaultRunMode = options.DefaultRunMode;
            config.IgnoreExtraArgs = options.IgnoreExtraArgs;
            this._commands = new CommandService(config);
            foreach (Assembly asm in options.CommandAssemblies)
                await this._commands.AddModulesAsync(asm, _serviceProvider).ConfigureAwait(false);
            foreach (Type t in options.CommandClasses)
                await this._commands.AddModuleAsync(t, _serviceProvider).ConfigureAwait(false);
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            // most of the implementation here taken from https://discord.foxbot.me/docs/guides/commands/intro.html
            // with my own pinch of customizations - TehGM

            // Don't process the command if it was a system message
            if (!(msg is SocketUserMessage message))
                return;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            DiscordOptions options = this._options.CurrentValue;
            // only execute if not a bot message
            if (!options.AcceptBotMessages && message.Author.IsBot)
                return;
            // get prefix and argPos
            int argPos = 0;
            bool requirePrefix = msg.Channel is SocketGuildChannel ? options.RequirePublicMessagePrefix : options.RequirePrivateMessagePrefix;
            bool hasStringPrefix = message.HasStringPrefix(options.Prefix, ref argPos);
            bool hasMentionPrefix = false;
            if (hasStringPrefix)
                argPos++;   // for string prefix, move pos by 1 so it works with space
            else
                hasMentionPrefix = message.HasMentionPrefix(_client.CurrentUser, ref argPos);

            // if prefix not found but is required, return
            if (requirePrefix && (!string.IsNullOrWhiteSpace(options.Prefix) && !hasStringPrefix) && (options.AcceptMentionPrefix && !hasMentionPrefix))
                return;

            // Create a WebSocket-based command context based on the message
            SocketCommandContext context = new SocketCommandContext(this._client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.

            // Keep in mind that result does not indicate a return value
            // rather an object stating if the command executed successfully.
            using IServiceScope scope = this._serviceProvider.CreateScope();
            IResult result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: scope.ServiceProvider)
                .ConfigureAwait(false);
            if (!result.IsSuccess && result is ExecuteResult executeResult && executeResult.Exception != null && !(executeResult.Exception is OperationCanceledException))
                this._log.LogError(executeResult.Exception, "Unhandled Exception when executing a basic command");
        }

        public void Dispose()
        {
            try { this._optionsChangeRegistration?.Dispose(); } catch { }
            try { this._client.MessageReceived -= HandleCommandAsync; } catch { }
        }
    }
}
