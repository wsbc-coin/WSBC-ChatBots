namespace WSBC.DiscordBot
{
    /// <summary>Represents aggregated data for displaying in Discord.</summary>
    class CoinData
    {
        /// <summary>Full name of the currency.</summary>
        public string Name { get; }
        /// <summary>Short name of the currency.</summary>
        public string Code { get; }
        /// <summary>Current price in BTC.</summary>
        public decimal BtcPrice { get; init; }
        /// <summary>Count of coins currently in supply.</summary>
        public decimal Supply { get; init; }
        /// <summary>Total market capitalization, in USD.</summary>
        public decimal MarketCap { get; init; }
        /// <summary>Current block height.</summary>
        public int BlockHeight { get; init; }

        public CoinData(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }
    }
}
