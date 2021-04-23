using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WSBC.ChatBots.Telegram
{
    class TelegramCommand
    {
        public string Command { get; }
        public string Description { get; }
        public Action<ITelegramBotClient, Message> Callback { get; }

        public bool IsListed => !string.IsNullOrWhiteSpace(this.Description);

        public TelegramCommand(string command, string description, Action<ITelegramBotClient, Message> callback)
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

        public TelegramCommand(string command, Action<ITelegramBotClient, Message> callback)
            : this(command, null, callback) { }

        public void Invoke(ITelegramBotClient client, Message message)
            => this.Callback.Invoke(client, message);

        public static explicit operator BotCommand(TelegramCommand command)
            => new BotCommand() { Command = command.Command, Description = command.Description };

        public override string ToString()
            => this.Command;
    }
}
