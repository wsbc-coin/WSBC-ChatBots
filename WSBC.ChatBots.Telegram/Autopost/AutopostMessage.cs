using Telegram.Bot.Types.Enums;

namespace WSBC.ChatBots.Telegram.Autopost
{
    class AutopostMessage
    {
        public string Content { get; set; }
        public ParseMode? ParsingMode { get; set; }
        public string ImagePath { get; set; }
        public string ImageURL { get; set; }
        public int ReplyTo { get; set; } = 0;
        public bool DisableWebPreview { get; set; } = false;
    }
}
