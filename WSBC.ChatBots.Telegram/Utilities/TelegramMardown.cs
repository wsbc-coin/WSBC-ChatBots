﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSBC.ChatBots.Telegram
{
    public static class TelegramMardown
    {
        private static readonly HashSet<char> _unescapedCharacters = new HashSet<char>()
        {
            ' ', '\\', '\n', '*', '_', '~', '[', ']', '(', ')', '`', ','
        };

        public static IReadOnlyCollection<char> UnescapedCharacters => _unescapedCharacters.ToArray();

        public static string EscapeV2(string text, ICollection<char> ignoredCharacters)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            // telegram uses some own, absolutely fucked up "markdown" syntax
            // so we need to escape any 'normal' special character not on exclusions list
            StringBuilder builder = new StringBuilder(text);
            int i = 0;
            while (i < builder.Length)
            {
                if (ShouldEscape(builder[i]))
                {
                    builder.Insert(i, '\\');
                    i++;    // need to do one more skip to not check the same character again (since it has moved)
                }
                i++;
            }
            return builder.ToString();

            bool ShouldEscape(char c)
            {
                // skip all letters and digits
                if (char.IsDigit(c) || char.IsLetter(c))
                    return false;
                // check for non-ANSI
                if (c > 255)
                    return false;
                // skip elements on exclusion list
                if (ignoredCharacters?.Contains(c) == true)
                    return false;
                return true;
            }
        }

        public static string EscapeV2(string text)
            => EscapeV2(text, _unescapedCharacters);

        public static string FullEscapeV2(string text)
            => EscapeV2(text, null);
    }
}
