using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WSBC.ChatBots.Telegram
{
    internal static class CommandsHandlerExtensions
    {
        public static void Register(this ICommandsHandler handler, string command, string description, Action<ITelegramBotClient, Message> callback)
            => handler.Register(new TelegramCommand(command, description, callback));

        public static void Register(this ICommandsHandler handler, string command, Action<ITelegramBotClient, Message> callback)
            => Register(handler, command, null, callback);
    }
}
