using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WSBC.ChatBots.Utilities;

namespace WSBC.ChatBots.Token.Stex
{
    public class StexData
    {
        /// <summary>ID of the pair.</summary>
        [JsonProperty("id")]
        public int ID { get; init; }
        /// <summary>Amount multiplier. (?)</summary>
        [JsonProperty("amount_multiplier")]
        public double Multiplier { get; init; }
        /// <summary>Code of the token.</summary>
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; init; }
        /// <summary>Name of the token.</summary>
        [JsonProperty("currency_name")]
        public string CurrencyName { get; init; }
        /// <summary>Code of the trade currency.</summary>
        [JsonProperty("maret_code")]
        public string TradeCurrencyCode { get; init; }
        /// <summary>Name of the trade currency.</summary>
        [JsonProperty("market_name")]
        public string TradeCurrencyName { get; init; }
        /// <summary>Name of the group.</summary>
        [JsonProperty("group_name")]
        public string GroupName { get; init; }
        /// <summary>ID of the group.</summary>
        [JsonProperty("group_id")]
        public int GroupID { get; init; }
        /// <summary>Symbol of the trading paid.</summary>
        [JsonProperty("symbol")]
        public string PairSymbol { get; init; }


        /// <summary>Price at which token is currently sold.</summary>
        [JsonProperty("ask")]
        public decimal AskPrice { get; init; }
        /// <summary>Price at which token is currently bought.</summary>
        [JsonProperty("bid")]
        public decimal BidPrice { get; init; }
        /// <summary>Price at which token was last traded.</summary>
        [JsonProperty("last")]
        public decimal LastPrice { get; init; }
        /// <summary>Trade price 24 hours ago.</summary>
        [JsonProperty("open")]
        public decimal OpenPrice { get; init; }
        /// <summary>Max trade price of the last 24 hours.</summary>
        [JsonProperty("high")]
        public decimal HighPrice { get; init; }
        /// <summary>Min trade price of the last 24 hours.</summary>
        [JsonProperty("low")]
        public decimal LowPrice { get; init; }
        /// <summary>Tokens traded in last 24 hours.</summary>
        [JsonProperty("volume")]
        public double Volume { get; init; }
        /// <summary>Trading volume in currency of the last 24 hours.</summary>
        [JsonProperty("volumeQuote")]
        public double VolumQuotee { get; init; }
        /// <summary>Trades count.</summary>
        [JsonProperty("count")]
        public int TradeCount { get; init; }
        /// <summary>Change versus last 24 hours.</summary>
        [JsonProperty("change")]
        public double Change { get; init; }

        /// <summary>Index price.</summary>
        [JsonProperty("index_price")]
        public decimal IndexPrice { get; init; }
        /// <summary>Rates for fiat currencies calculations.</summary>
        [JsonProperty("fiatsRate")]
        public IDictionary<string, decimal> FiatRates { get; init; }


        /// <summary>Time when the data was snapshotted.</summary>
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTimeOffset Timestamp { get; init; }
    }
}
