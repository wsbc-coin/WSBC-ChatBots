using System;
using Newtonsoft.Json;

namespace WSBC.ChatBots.Coin.TxBit
{
    /// <summary>Represents coin data as presented by TxBit API.</summary>
    /// <seealso href="https://apidocs.txbit.io/#public-getcurrencyinformation"/>
    public class TxBitData
    {
        /// <summary>Currency full name.</summary>
        [JsonProperty("CurrencyLong")]
        public string CurrencyName { get; init; }
        /// <summary>Currency short name (code).</summary>
        [JsonProperty("Currency")]
        public string CurrencyCode { get; init; }
        /// <summary>Currency type.</summary>
        [JsonProperty("AssetType")]
        public TxBitAssetType AssetType { get; init; }
        /// <summary>Currency consensus type.</summary>
        /// <remarks>Examples: "POW", "POS" etc.</remarks>
        [JsonProperty("ConsensusType")]
        public string ConsensusType { get; init; }
        /// <summary>Count of coins currently in supply.</summary>
        [JsonProperty("Supply")]
        public decimal Supply { get; init; }
        /// <summary>Total market capitalization, in USD.</summary>
        [JsonProperty("MarketCap")]
        public decimal MarketCap { get; init; }
        /// <summary>Price at which the currency was last sold, in BTC.</summary>
        [JsonProperty("BidPrice")]
        public decimal BidPrice { get; init; }
        /// <summary>Algorithm used by the currency.</summary>
        [JsonProperty("Algorithm")]
        public string Algorithm { get; init; }
        /// <summary>Official currency website URL.</summary>
        [JsonProperty("Website")]
        public string WebsiteURL { get; init; }
        /// <summary>Official blockchain explorer URL.</summary>
        [JsonProperty("Explorer")]
        public string ExplorerURL { get; init; }
        /// <summary>Source code repository URL.</summary>
        [JsonProperty("SourceCode")]
        public string SourceCodeURL { get; init; }
        /// <summary>BitcoinTalk forum announcement thread URL.</summary>
        [JsonProperty("Announcement")]
        public string BitcoinTalkAnnouncementURL { get; init; }
        /// <summary>Currency's CoinMarketCap URL.</summary>
        [JsonProperty("CoinMarketCap")]
        public string CoinMarketCapURL { get; init; }
        /// <summary>Currency's CoinGecko URL.</summary>
        [JsonProperty("CoinGecko")]
        public string CoinGeckoURL { get; init; }
        /// <summary>Currency description.</summary>
        [JsonProperty("Description")]
        public string Description { get; init; }
        /// <summary>Currency block count.</summary>
        [JsonProperty("BlockCount")]
        public int BlockCount { get; init; }
        /// <summary>Is wallet available?</summary>
        [JsonProperty("IsAvailable")]
        public bool IsWalletAvailable { get; init; }
        /// <summary>Time when last block was mined.</summary>
        [JsonProperty("LastBlock")]
        public DateTimeOffset? LastBlockTime { get; init; }
        /// <summary>Count of live master nodes.</summary>
        [JsonProperty("MasternodeCount")]
        public int MasterNodeCount { get; init; }

        [JsonConstructor]
        private TxBitData() { }
    }

    public enum TxBitAssetType
    {
        /// <summary>Currency is a coin with own blockchain.</summary>
        COIN, 
        /// <summary>Currency is a token on other currency's blockchain.</summary>
        TOKEN
    }
}
