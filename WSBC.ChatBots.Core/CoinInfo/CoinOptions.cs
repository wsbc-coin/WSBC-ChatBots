using System;

namespace WSBC.ChatBots.Coin
{
    public class CoinOptions
    {
        public string CoinName { get; set; } = "WallStreetBets Coin";
        public string CoinTicker { get; set; } = "WSBC";
        public string CoinIconURL { get; set; } = "https://avatars.githubusercontent.com/u/79186640";
        public string CoinURL { get; set; } = "https://wallstreetbetsbros.com/";

        public TimeSpan DataCacheLifetime { get; set; } = TimeSpan.FromSeconds(10);
        public TimeSpan MiningPoolStatsDataCacheLifetime { get; set; } = TimeSpan.FromMinutes(3);
    }
}
