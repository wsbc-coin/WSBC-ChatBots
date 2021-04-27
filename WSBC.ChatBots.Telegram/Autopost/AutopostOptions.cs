using Telegram.Bot.Types.Enums;

namespace WSBC.ChatBots.Telegram.Autopost
{
    class AutopostOptions
    {
        public uint MessageRate { get; set; } = 35;
        public long ChannelID { get; set; }
        public ParseMode ParsingMode { get; set; } = ParseMode.Html;

        public string[] Messages { get; set; }
    }
}
