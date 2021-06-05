using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WSBC.ChatBots.Telegram
{
    public class TelegramPing
    {
        public string DisplayName { get; }
        public int UserID { get; }
        public string Link => $"tg://user?id={this.UserID}";

        private readonly ParseMode _parseMode;
        private const ParseMode _defaultParseMode = ParseMode.MarkdownV2;

        public TelegramPing(string displayName, int userID, ParseMode parseMode)
        {
            this.DisplayName = displayName;
            this.UserID = userID;
            this._parseMode = parseMode;
        }

        public TelegramPing(string displayName, int userID)
            : this(displayName, userID, _defaultParseMode) { }

        public TelegramPing(User user, ParseMode parseMode)
            : this(GetUserDisplayName(user), user.Id, parseMode) { }

        public TelegramPing(User user)
            : this(user, _defaultParseMode) { }

        private static string GetUserDisplayName(User user)
        {
            IEnumerable<string> displayNameParts = new string[] { user.FirstName, user.LastName }.Where(x => !string.IsNullOrWhiteSpace(x));
            return string.Join(' ', displayNameParts);
        }

        public override string ToString()
            => this.ToString(this._parseMode);

        public string ToString(ParseMode parseMode)
        {
            if (parseMode == ParseMode.Default)
                return this.Link;
            else if (parseMode == ParseMode.Html)
                return $"<a href=\"{this.Link}\">{this.DisplayName}</a>";
            else
                return $"[{TelegramMardown.FullEscapeV2(this.DisplayName)}]({this.Link})";
        }
    }
}
