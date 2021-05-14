using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WSBC.ChatBots.Telegram
{
    class CommandContext
    {
        public ITelegramBotClient Client { get; }
        public Message Message { get; }
        public string Arguments { get; }

        public string Text => this.Message.Text;
        public long ChatID => this.Message.Chat.Id;
        public int MessageID => this.Message.MessageId;

        public CommandContext(ITelegramBotClient client, Message message, string args)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            this.Client = client;
            this.Message = message;
            this.Arguments = args;
        }

        public CommandContext(ITelegramBotClient client, Message message)
            : this(client, message, null) { }
    }
}
