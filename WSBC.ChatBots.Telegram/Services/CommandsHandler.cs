using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WSBC.ChatBots.Telegram.Services
{
    /// <summary>A simple commands handler.</summary>
    // This is a very simple entry for commands processing.
    // It's not a fully fledged commands system - that might be added later, depending on if it's really needed and if I can be bothered.
    class CommandsHandler : ICommandsHandler, IDisposable
    {
        private readonly ITelegramClient _client;
        private readonly ILogger _log;
        private readonly IDictionary<string, TelegramCommand> _commands;
        private User _currentUser;

        public CommandsHandler(ITelegramClient client, ILogger<CommandsHandler> log)
        {
            this._client = client;
            this._log = log;
            this._commands = new Dictionary<string, TelegramCommand>(StringComparer.OrdinalIgnoreCase);

            this._client.MessageReceived += OnMessageReceived;
        }

        private async void OnMessageReceived(object sender, MessageEventArgs e)
        {
            Message msg = e.Message;
            if (msg.Type != MessageType.Text || string.IsNullOrWhiteSpace(msg.Text) || msg.Text[0] != '/')
                return;
            int spaceIndex = msg.Text.IndexOf(' ');
            string cmd = spaceIndex != -1 ? msg.Text.Remove(spaceIndex) : msg.Text;
            if (cmd.Contains('@'))
            {
                if (this._currentUser == null)
                    this._currentUser = await this._client.Client.GetMeAsync().ConfigureAwait(false);
                string username = "@" + this._currentUser.Username;
                if (cmd.EndsWith(username, StringComparison.OrdinalIgnoreCase))
                    cmd = cmd.Remove(cmd.Length - username.Length, username.Length);
            }
            if (!this._commands.TryGetValue(cmd, out TelegramCommand command))
                return;
            command?.Invoke(this._client.Client, msg);
        }

        public void Register(TelegramCommand command)
        {
            this._log.LogDebug("Registering command {Command}", command);
            this._commands[command.Command] = command;
        }

        public Task SubmitCommandsAsync(CancellationToken cancellationToken = default)
            => this._client.Client.SetMyCommandsAsync(this._commands.Values.Where(cmd => cmd.IsListed).Select(cmd => (BotCommand)cmd), cancellationToken);

        public void Dispose()
        {
            try { this._client.MessageReceived -= OnMessageReceived; } catch { }
            this._commands.Clear();
        }
    }
}
