using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WSBC.ChatBots.Telegram
{
    class TelegramCommand
    {
        public string Command { get; }
        public string Description { get; }
        public Action<CommandContext> Callback { get; }

        public bool IsListed => !string.IsNullOrWhiteSpace(this.Description);

        public TelegramCommand(string command, string description, Action<CommandContext> callback)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));
            if (command.Contains(' '))
                throw new ArgumentException("Commands cannot contain any spaces", nameof(command));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            this.Command = command;
            if (this.Command[0] != '/')
                this.Command = "/" + this.Command;
            this.Description = description;
            this.Callback = callback;
        }

        public TelegramCommand(string command, Action<CommandContext> callback)
            : this(command, null, callback) { }

        public void Invoke(CommandContext context)
            => this.Callback.Invoke(context);

        public static explicit operator BotCommand(TelegramCommand command)
            => new BotCommand() { Command = command.Command, Description = command.Description };

        public override string ToString()
            => this.Command;
    }
}
