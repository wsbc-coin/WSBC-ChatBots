using System;

namespace WSBC.ChatBots.Token
{
    public class TokenOptions
    {
        public string ContractAddress { get; set; } = "0x8244609023097aef71c702ccbaefc0bde5b48694";
        public TimeSpan DataCacheLifetime { get; set; } = TimeSpan.FromSeconds(60);
    }
}
