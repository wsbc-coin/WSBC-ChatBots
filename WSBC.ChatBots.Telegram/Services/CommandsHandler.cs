using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
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
        private readonly IDictionary<string, Action<ITelegramBotClient, Message>> _commands;

        public CommandsHandler(ITelegramClient client, ILogger<CommandsHandler> log)
        {
            this._client = client;
            this._log = log;
            this._commands = new Dictionary<string, Action<ITelegramBotClient, Message>>();

            this._client.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            Message msg = e.Message;
            if (msg.Type != MessageType.Text || string.IsNullOrWhiteSpace(msg.Text) || msg.Text[0] != '/')
                return;
            string cmd = msg.Text.Remove(msg.Text.IndexOf(' '));
            if (!this._commands.TryGetValue(cmd, out Action<ITelegramBotClient, Message> callback))
                return;
            callback?.Invoke(this._client.Client, msg);
        }

        public void Register(string command, Action<ITelegramBotClient, Message> callback)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));
            if (command.Contains(' '))
                throw new ArgumentException("Commands cannot contain any spaces", nameof(command));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            if (command[0] != '/')
                command = "/" + command;

            this._log.LogDebug("Registering command {Command}", command);
            this._commands[command] = callback;
        }

        public void Dispose()
        {
            try { this._client.MessageReceived -= OnMessageReceived; } catch { }
            this._commands.Clear();
        }
    }
}
