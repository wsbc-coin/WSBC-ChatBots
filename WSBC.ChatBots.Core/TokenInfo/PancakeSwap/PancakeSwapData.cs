using System;
using Newtonsoft.Json;
using WSBC.ChatBots.Utilities;

namespace WSBC.ChatBots.Token.PancakeSwap
{
    public class PancakeSwapData
    {
        /// <summary>Name of the token.</summary>
        [JsonProperty("name")]
        public string CurrencyName { get; init; }
        /// <summary>Symbol of the token.</summary>
        [JsonProperty("symbol")]
        public string CurrencySymbol { get; init; }
        /// <summary>Price of the token in USD.</summary>
        [JsonProperty("price")]
        public decimal PriceUSD { get; init; }
        /// <summary>Price of the token in BNB.</summary>
        [JsonProperty("price_BNB")]
        public decimal PriceBNB { get; init; }

        /// <summary>Time when the data was snapshotted.</summary>
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(MillisecondsUnixTimestampConverter))]
        public DateTimeOffset Timestamp { get; init; }
    }
}
