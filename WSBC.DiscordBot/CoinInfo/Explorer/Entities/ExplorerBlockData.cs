using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WSBC.DiscordBot.Utilities;

namespace WSBC.DiscordBot.Explorer
{
    /// <summary>Deserialized response of Explorer's /block/ API endpoint.</summary>
    /// <seealso href="https://github.com/arqma/explorer-arqma#apiblockblock_numberblock_hash"/>
    public class ExplorerBlockData
    {
        /// <summary>Height of this block.</summary>
        [JsonProperty("block_height")]
        public int Height { get; init; }
        /// <summary>Height of top block in blockchain.</summary>
        [JsonProperty("current_height")]
        public int TopBlockHeight { get; init; }
        /// <summary>Block difficulty.</summary>
        [JsonProperty("diff")]
        public int Difficulty { get; init; }
        /// <summary>Block's unique hash.</summary>
        [JsonProperty("hash")]
        public string Hash { get; init; }
        /// <summary>Block size, in bytes.</summary>
        [JsonProperty("size")]
        public int Size { get; init; }
        /// <summary>Time when this block was created.</summary>
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTimeOffset Timestamp { get; init; }
        /// <summary>Transactions in this block.</summary>
        /// <remarks>Transactions in this collection will not have all fields populated. Request transaction directly to get all data.</remarks>
        [JsonProperty("txs")]
        public IEnumerable<ExplorerTransactionData> Transactions { get; init; }
    }
}
