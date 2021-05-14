using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace WSBC.ChatBots.Token
{
    public class TokenOptions
    {
        public string IconURL { get; set; } = "https://lcw.nyc3.cdn.digitaloceanspaces.com/production/currencies/64/wsbt.webp";
        public string ContractAddress { get; set; } = "0x8244609023097aef71c702ccbaefc0bde5b48694";
        public TimeSpan DataCacheLifetime { get; set; } = TimeSpan.FromSeconds(60);
        public IEnumerable<ExchangeInfo> Exchanges { get; set; }
    }

    internal class ConfigureTokenOptions : IPostConfigureOptions<TokenOptions>
    {
        private static readonly ExchangeInfo[] _defaultExchanges = new ExchangeInfo[]
        {
            new ExchangeInfo()
            {
                DisplayName = "WhiteBit",
                URL = "https://whitebit.com/trade/WSBT_BTC",
                Pairs = new string[] { "BTC" }
            },
            new ExchangeInfo()
            {
                DisplayName = "STEX",
                URL = "https://app.stex.com/en/basic-trade/pair/USDT/WSBT/1D",
                Pairs = new string[] { "USDT", "BTC" }
            },
            new ExchangeInfo()
            {
                DisplayName = "Dex-Trade",
                URL = "https://dex-trade.com/spot/trading/WSBTUSDT",
                Pairs = new string[] { "USDT", "BTC", "BNB" }
            },
            new ExchangeInfo()
            {
                DisplayName = "PancakeSwap",
                URL = "https://exchange.pancakeswap.finance/#/swap?outputCurrency=0x8244609023097AeF71C702cCbaEFC0bde5b48694",
                Pairs = new string[] { "BNB" }
            }
        };

        public void PostConfigure(string name, TokenOptions options)
        {
            if (options.Exchanges == null)
                options.Exchanges = _defaultExchanges;
        }
    }
}
