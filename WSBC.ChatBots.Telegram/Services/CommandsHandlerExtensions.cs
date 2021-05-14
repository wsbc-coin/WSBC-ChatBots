using System;

namespace WSBC.ChatBots.Telegram
{
    internal static class CommandsHandlerExtensions
    {
        public static void Register(this ICommandsHandler handler, string command, string description, Action<CommandContext> callback)
            => handler.Register(new TelegramCommand(command, description, callback));

        public static void Register(this ICommandsHandler handler, string command, Action<CommandContext> callback)
            => Register(handler, command, null, callback);
    }
}
