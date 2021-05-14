using System.Collections.Generic;

namespace WSBC.ChatBots
{
    public class ExchangeInfo
    {
        public string DisplayName { get; set; }
        public string URL { get; set; }
        public IEnumerable<string> Pairs { get; set; }
    }
}
