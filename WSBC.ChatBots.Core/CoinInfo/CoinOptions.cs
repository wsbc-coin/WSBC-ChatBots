using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace WSBC.ChatBots.Coin
{
    public class CoinOptions
    {
        public string CoinName { get; set; } = "WallStreetBets Coin";
        public string CoinTicker { get; set; } = "WSBC";
        public string IconURL { get; set; } = "https://raw.githubusercontent.com/wiki/wsbc-coin/wallstreetbets/img/coin-logo-1.png";
        public string CoinURL { get; set; } = "https://wallstreetbetsbros.com/";
        public IEnumerable<ExchangeInfo> Exchanges { get; set; }

        public TimeSpan DataCacheLifetime { get; set; } = TimeSpan.FromSeconds(10);
        public TimeSpan MiningPoolStatsDataCacheLifetime { get; set; } = TimeSpan.FromMinutes(3);
    }

    internal class ConfigureCoinOptions : IPostConfigureOptions<CoinOptions>
    {
        private static readonly ExchangeInfo[] _defaultExchanges = new ExchangeInfo[]
        {
            new ExchangeInfo()
            {
                DisplayName = "TxBit",
                URL = "https://txbit.io/Trade/WSBC/BTC",
                Pairs = new string[] { "BTC" }
            }
        };

        public void PostConfigure(string name, CoinOptions options)
        {
            if (options.Exchanges == null)
                options.Exchanges = _defaultExchanges;
        }
    }
}
