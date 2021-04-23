namespace WSBC.ChatBots.Token
{
    public class TokenData
    {
        /// <summary>Number of transactions in last 24 hours.</summary>
        public int Transactions { get; init; }
        /// <summary>Percentage (-1 to 1) difference of transactions count compared with previous day.</summary>
        public double TransactionsChange { get; init; }
        /// <summary>Tokens traded in last 24 hours.</summary>
        public double Volume { get; init; }
        /// <summary>USD value traded in last 24 hours.</summary>
        public decimal VolumeUSD { get; init; }
        /// <summary>ETH value traded in last 24 hours.</summary>
        public decimal VolumeETH { get; init; }
        /// <summary>Percentage (-1 to 1) difference of trade volume compared with previous day.</summary>
        public double VolumeChange { get; init; }
        /// <summary>USD value of liquidity pool.</summary>
        public decimal LiquidityUSD { get; init; }
        /// <summary>ETH value of liquidity pool.</summary>
        public decimal LiquidityETH { get; init; }
        /// <summary>Percentage (-1 to 1) difference of liqidity pool value compared with previous day.</summary>
        public double LiquidityChange { get; init; }
        /// <summary>Price of token in USD.</summary>
        public decimal PriceUSD { get; init; }
        /// <summary>Price of token in ETH.</summary>
        public decimal PriceETH { get; init; }
        /// <summary>Percentage (-1 to 1) difference of token price compared with previous day.</summary>
        public double PriceChange { get; init; }
    }
}
