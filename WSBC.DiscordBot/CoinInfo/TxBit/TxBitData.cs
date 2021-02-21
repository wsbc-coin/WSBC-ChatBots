using System;
using Newtonsoft.Json;

namespace WSBC.DiscordBot.TxBit
{
    /// <summary>Represents coin data as presented by TxBit API.</summary>
    /// <seealso href="https://apidocs.txbit.io/#public-getcurrencyinformation"/>
    class TxBitData
    {
        [JsonProperty("CurrencyLong")]
        public string CurrencyName { get; private set; }
        [JsonProperty("Currency")]
        public string CurrencyCode { get; private set; }
        [JsonProperty("AssetType")]
        public TxBitAssetType AssetType { get; private set; }
        [JsonProperty("ConsensusType")]
        public string ConsensusType { get; private set; }
        [JsonProperty("Supply")]
        public decimal Supply { get; private set; }
        [JsonProperty("MarketCap")]
        public decimal MarketCap { get; private set; }
        [JsonProperty("BidPrice")]
        public decimal BidPrice { get; private set; }
        [JsonProperty("Algorithm")]
        public string Algorithm { get; private set; }
        [JsonProperty("Website")]
        public string WebsiteURL { get; private set; }
        [JsonProperty("Explorer")]
        public string ExplorerURL { get; private set; }
        [JsonProperty("SourceCode")]
        public string SourceCodeURL { get; private set; }
        [JsonProperty("Announcement")]
        public string BitcoinTalkAnnouncementURL { get; private set; }
        [JsonProperty("CoinMarketCap")]
        public string CoinMarketCapURL { get; private set; }
        [JsonProperty("CoinGecko")]
        public string CoinGeckoURL { get; private set; }
        [JsonProperty("Description")]
        public string Description { get; private set; }
        [JsonProperty("BlockCount")]
        public int BlockCount { get; private set; }
        [JsonProperty("IsAvailable")]
        public bool IsAvailable { get; private set; }
        [JsonProperty("LastBlock")]
        public DateTimeOffset? LastBlockTime { get; private set; }
        [JsonProperty("MasternodeCount")]
        public int MasterNodeCount { get; private set; }

        [JsonConstructor]
        private TxBitData() { }
    }

    enum TxBitAssetType
    {
        COIN, TOKEN
    }
}
