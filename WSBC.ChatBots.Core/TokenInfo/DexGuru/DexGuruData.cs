using System;
using Newtonsoft.Json;
using WSBC.ChatBots.Utilities;

namespace WSBC.ChatBots.Token.DexGuru
{
    public class DexGuruData
    {
        /// <summary>ID of the token.</summary>
        [JsonProperty("id")]
        public string ID { get; init; }
        /// <summary>Symbol of the token.</summary>
        [JsonProperty("symbol")]
        public string Symbol { get; init; }
        /// <summaryName of the token.></summary>
        [JsonProperty("name")]
        public string Name { get; init; }
        /// <summary>Token description.</summary>
        [JsonProperty("description")]
        public string Description { get; init; }
        /// <summary>Number of transactions in last 24 hours.</summary>
        [JsonProperty("txns24h")]
        public int Transactions { get; init; }
        /// <summary>Percentage (-1 to 1) difference of transactions count compared with previous day.</summary>
        [JsonProperty("txns24hChange")]
        public double TransactionsChange { get; init; }
        /// <summary>Is token verified?</summary>
        [JsonProperty("verified")]
        public bool IsVerified { get; init; }
        /// <summary>Token's decimals count.</summary>
        [JsonProperty("decimals")]
        public int DecimalsCount { get; init; }
        /// <summary>Tokens traded in last 24 hours.</summary>
        [JsonProperty("volume24h")]
        public double Volume { get; init; }
        /// <summary>USD value traded in last 24 hours.</summary>
        [JsonProperty("volume24hUSD")]
        public decimal VolumeUSD { get; init; }
        /// <summary>ETH value traded in last 24 hours.</summary>
        [JsonProperty("volume24hETH")]
        public decimal VolumeETH { get; init; }
        /// <summary>Percentage (-1 to 1) difference of trade volume compared with previous day.</summary>
        [JsonProperty("volumeChange24h")]
        public double VolumeChange { get; init; }
        /// <summary>USD value of liquidity pool.</summary>
        [JsonProperty("liquidityUSD")]
        public decimal LiquidityUSD { get; init; }
        /// <summary>ETH value of liquidity pool.</summary>
        [JsonProperty("liquidityETH")]
        public decimal LiquidityETH { get; init; }
        /// <summary>Percentage (-1 to 1) difference of liqidity pool value compared with previous day.</summary>
        [JsonProperty("liquidityChange24h")]
        public double LiquidityChange { get; init; }
        /// <summary>Price of token in USD.</summary>
        [JsonProperty("priceUSD")]
        public decimal PriceUSD { get; init; }
        /// <summary>Price of token in ETH.</summary>
        [JsonProperty("priceETH")]
        public decimal PriceETH { get; init; }
        /// <summary>Percentage (-1 to 1) difference of token price compared with previous day.</summary>
        [JsonProperty("priceChange24h")]
        public double PriceChange { get; init; }
        /// <summary>Time when the data was snapshotted.</summary>
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTimeOffset Timestamp { get; init; }
        /// <summary>Blockchain block number.</summary>
        [JsonProperty("blockNumber")]
        public uint Block { get; init; }
        /// <summary>???</summary>
        [JsonProperty("AMM")]
        public string AMM { get; init; }
        /// <summary>Blockchain network of the token.</summary>
        [JsonProperty("network")]
        public string Network { get; init; }
    }
}
