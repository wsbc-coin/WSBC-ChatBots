using System;
using Newtonsoft.Json;
using WSBC.DiscordBot.Utilities;

namespace WSBC.DiscordBot.Explorer
{
    /// <summary>Deserialized response of Explorer's /networkinfo API endpoint.</summary>
    /// <seealso href="https://github.com/arqma/explorer-arqma#apinetworkinfo"/>
    class ExplorerNetworkData
    {
        /// <summary>Count of alt blocks.</summary>
        [JsonProperty("alt_blocks_count")]
        public int AltBlocksCount { get; init; }
        /// <summary>Max size of a block.</summary>
        [JsonProperty("block_size_limit")]
        public int BlockSizeLimit { get; init; }
        /// <summary>Cumulative difficulty.</summary>
        [JsonProperty("cumulative_difficulty")]
        public long CumulativeDifficulty { get; init; }
        /// <summary>Network difficulty.</summary>
        [JsonProperty("difficulty")]
        public long Difficulty { get; init; }
        /// <summary>Fee per kilobyte.</summary>
        [JsonProperty("fee_per_kb")]
        public decimal FeePerKilobyte { get; init; }
        /// <summary>Network hashrate.</summary>
        [JsonProperty("hash_rate")]
        public int Hashrate { get; init; }
        /// <summary>Block height.</summary>
        [JsonProperty("height")]
        public int BlockHeight { get; init; }
        /// <summary>Count of incoming connections.</summary>
        [JsonProperty("incoming_connections_count")]
        public int IncomingConnectionsCount { get; init; }
        /// <summary>Count of outgoign connections.</summary>
        [JsonProperty("outgoing_connections_count")]
        public int OutgoingConnectionsCount { get; init; }
        /// <summary>Time when the blockchain started.</summary>
        [JsonProperty("start_time")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTimeOffset StartTime { get; init; }
        /// <summary>Blockchain status.</summary>
        [JsonProperty("status")]
        public string Status { get; init; }
        /// <summary>Target block time, in seconds.</summary>
        [JsonProperty("target")]
        public int TargetBlockTime { get; init; }
        /// <summary>Target block height.</summary>
        [JsonProperty("target_height")]
        public int TargetBlockHeight { get; init; }
        /// <summary>Is test network?</summary>
        [JsonProperty("testnet")]
        public bool IsTestnet { get; init; }
        /// <summary>Hash of the latest block.</summary>
        [JsonProperty("top_block_hash")]
        public string TopBlockHash { get; init; }
        /// <summary>Transactions count.</summary>
        [JsonProperty("tx_count")]
        public int TransactionsCount { get; init; }
        /// <summary>Size of transactions pool.</summary>
        [JsonProperty("tx_pool_size")]
        public int TransactionPoolSize { get; init; }
        [JsonProperty("grey_peerlist_size")]
        public int GreyPeerListSize { get; init; }
        [JsonProperty("white_peerlist_size")]
        public int WhitePeerListSize { get; init; }

        [JsonConstructor]
        private ExplorerNetworkData() { }
    }
}
