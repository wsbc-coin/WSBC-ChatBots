using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WSBC.ChatBots.Telegram
{
    interface ICommandsHandler
    {
        void Register(string command, Action<ITelegramBotClient, Message> callback);
    }
}
