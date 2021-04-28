using Newtonsoft.Json;

namespace WSBC.ChatBots.Token.DexTrade
{
    public class DexTradeData
    {
        /// <summary>ID of the token.</summary>
        [JsonProperty("id")]
        public int ID { get; init; }
        /// <summary>Symbol of the trading paid.</summary>
        [JsonProperty("pair")]
        public string PairSymbol { get; init; }
        /// <summary>Tokens traded in last 24 hours.</summary>
        [JsonProperty("volume_24H")]
        public double Volume { get; init; }
        /// <summary>Price at which token was last traded.</summary>
        [JsonProperty("last")]
        public decimal LastPrice { get; init; }
        /// <summary>Open price.</summary>
        [JsonProperty("open")]
        public decimal OpenPrice { get; init; }
        /// <summary>Close price.</summary>
        [JsonProperty("close")]
        public decimal ClosePrice { get; init; }
        /// <summary>High price.</summary>
        [JsonProperty("high")]
        public decimal HighPrice { get; init; }
        /// <summary>Low price.</summary>
        [JsonProperty("low")]
        public decimal LowPrice { get; init; }
    }
}
