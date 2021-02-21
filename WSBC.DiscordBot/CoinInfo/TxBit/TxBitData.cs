using System;
using Newtonsoft.Json;

namespace WSBC.DiscordBot.TxBit
{
    /// <summary>Represents coin data as presented by TxBit API.</summary>
    /// <seealso href="https://apidocs.txbit.io/#public-getcurrencyinformation"/>
    class TxBitData
    {
        [JsonProperty("CurrencyLong")]
        public string CurrencyName { get; init; }
        [JsonProperty("Currency")]
        public string CurrencyCode { get; init; }
        [JsonProperty("AssetType")]
        public TxBitAssetType AssetType { get; init; }
        [JsonProperty("ConsensusType")]
        public string ConsensusType { get; init; }
        [JsonProperty("Supply")]
        public decimal Supply { get; init; }
        [JsonProperty("MarketCap")]
        public decimal MarketCap { get; init; }
        [JsonProperty("BidPrice")]
        public decimal BidPrice { get; init; }
        [JsonProperty("Algorithm")]
        public string Algorithm { get; init; }
        [JsonProperty("Website")]
        public string WebsiteURL { get; init; }
        [JsonProperty("Explorer")]
        public string ExplorerURL { get; init; }
        [JsonProperty("SourceCode")]
        public string SourceCodeURL { get; init; }
        [JsonProperty("Announcement")]
        public string BitcoinTalkAnnouncementURL { get; init; }
        [JsonProperty("CoinMarketCap")]
        public string CoinMarketCapURL { get; init; }
        [JsonProperty("CoinGecko")]
        public string CoinGeckoURL { get; init; }
        [JsonProperty("Description")]
        public string Description { get; init; }
        [JsonProperty("BlockCount")]
        public int BlockCount { get; init; }
        [JsonProperty("IsAvailable")]
        public bool IsAvailable { get; init; }
        [JsonProperty("LastBlock")]
        public DateTimeOffset? LastBlockTime { get; init; }
        [JsonProperty("MasternodeCount")]
        public int MasterNodeCount { get; init; }

        [JsonConstructor]
        private TxBitData() { }
    }

    enum TxBitAssetType
    {
        COIN, TOKEN
    }
}
