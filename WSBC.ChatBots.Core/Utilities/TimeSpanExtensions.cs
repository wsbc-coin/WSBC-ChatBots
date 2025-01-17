﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSBC.ChatBots
{
    public static class TimeSpanExtensions
    {
        [ThreadStatic]
        private static List<string> _components;
        [ThreadStatic]
        private static StringBuilder _builder;

        public static string ToDisplayString(this TimeSpan timespan)
        {
            if (_components == null)
                _components = new List<string>(4);
            if (_builder == null)
                _builder = new StringBuilder();

            try
            {
                AddComponent(timespan.Days, "day");
                AddComponent(timespan.Hours, "hour");
                AddComponent(timespan.Minutes, "minute");
                AddComponent(timespan.Seconds, "second", _components.Count == 0);

                if (_components.Count > 1)
                {
                    _builder.Append(string.Join(", ", _components.Take(_components.Count - 1)));
                    _builder.Append(" and ");
                }
                if (_components.Count != 0)
                    _builder.Append(_components.Last());
                return _builder.ToString();
            }
            finally
            {
                _components.Clear();
                _builder.Clear();
            }
        }

        private static void AddComponent(int value, string text, bool skipZeroCheck = false)
        {
            if (skipZeroCheck || value != 0)
                _components.Add($"{value} {text}{(value != 1 ? "s" : string.Empty)}");
        }
    }
}
