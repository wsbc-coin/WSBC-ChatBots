using System;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace WSBC.ChatBots.Telegram
{
    public interface ITelegramClient
    {
        ITelegramBotClient Client { get; }
        event EventHandler<MessageEventArgs> MessageReceived;
        void Start();
        void Stop();
    }
}
