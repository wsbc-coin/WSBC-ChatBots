using System;
using System.Collections.Generic;
using System.Reflection;
using Discord.Commands;

namespace WSBC.ChatBots.Discord.Discord
{
    public class DiscordOptions
    {
        public string BotToken { get; set; }

        #region COMMANDS
        public string Prefix { get; set; } = "!wsbc";
        public bool AcceptMentionPrefix { get; set; } = true;
        public bool AcceptBotMessages { get; set; } = false;
        public bool RequirePublicMessagePrefix { get; set; } = true;
        public bool RequirePrivateMessagePrefix { get; set; } = false;

        public bool CaseSensitive { get; set; } = false;
        public RunMode DefaultRunMode { get; set; } = RunMode.Default;
        public bool IgnoreExtraArgs { get; set; } = true;

        // for loading
        public ICollection<Type> CommandClasses { get; set; } = new List<Type>();
        public ICollection<Assembly> CommandAssemblies { get; set; } = new List<Assembly>() { Assembly.GetEntryAssembly() };
        #endregion
    }
}
