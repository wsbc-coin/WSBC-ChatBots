using System;

namespace WSBC.ChatBots.Discord
{
    /// <summary>Represents aggregated data for displaying in Discord.</summary>
    public class CoinData
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
        /// <summary>Network difficulty.</summary>
        public long Difficulty { get; init; }
        /// <summary>Network hashrate.</summary>
        public int Hashrate { get; init; }
        /// <summary>Total transactions count.</summary>
        public int TransactionsCount { get; init; }
        /// <summary>Hash of the top block.</summary>
        public string TopBlockHash { get; init; }
        /// <summary>Target block time, in seconds.</summary>
        public int TargetBlockTime { get; init; }
        /// <summary>Last block's reward.</summary>
        public decimal BlockReward { get; init; }
        /// <summary>Time when the last block was found.</summary>
        public DateTimeOffset? LastBlockTime { get; init; }

        public CoinData(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }
    }
}
