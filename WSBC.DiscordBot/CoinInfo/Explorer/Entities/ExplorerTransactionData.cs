using System;
using Newtonsoft.Json;
using WSBC.DiscordBot.Serialization;
using WSBC.DiscordBot.Utilities;

namespace WSBC.DiscordBot.Explorer
{
    /// <summary>Deserialized response of Explorer's /transaction/ API endpoint.</summary>
    /// <remarks>This class doesn't cointain all info returned by API - for example, inputs and outputs are currently missing.
    /// These values are currently not needed, and supporting some of them would just add complexity.</remarks>
    /// <seealso href="https://github.com/arqma/explorer-arqma#apitransactiontx_hash"/>
    class ExplorerTransactionData
    {
        /// <summary>Height of the block this transaction is written to.</summary>
        [JsonProperty("block_height")]
        public int BlockHeight { get; init; }
        /// <summary>Height of top block in blockchain.</summary>
        [JsonProperty("current_height")]
        public int TopBlockHeight { get; init; }
        /// <summary>Whether this transaction is a mining reward.</summary>
        [JsonProperty("coinbase")]
        public bool IsCoinbase { get; init; }
        /// <summary>Unique hash of this transaction.</summary>
        [JsonProperty("tx_hash")]
        public string Hash { get; init; }
        /// <summary>Number of confirmations.</summary>
        [JsonProperty("confirmations")]
        public int ConfirmationsCount { get; init; }
        /// <summary>Sum of values of all inputs.</summary>
        [JsonProperty("evox_inputs")]
        [JsonConverter(typeof(TransactionAmountConverter))]
        public decimal InputsSum { get; init; }
        /// <summary>Sum of values of all outputs.</summary>
        [JsonProperty("evox_outputs")]
        [JsonConverter(typeof(TransactionAmountConverter))]
        public decimal OutputsSum { get; init; }
        /// <summary>Extra hash.</summary>
        [JsonProperty("extra")]
        public string Extra { get; init; }
        /// <summary>Timestamp of this transaction.</summary>
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTimeOffset Timestamp { get; init; }
        /// <summary>Transaction fee.</summary>
        [JsonProperty("tx_fee")]
        [JsonConverter(typeof(TransactionAmountConverter))]
        public decimal Fee { get; init; }
        /// <summary>Transaction version.</summary>
        [JsonProperty("tx_version")]
        public int Version { get; init; }
        /// <summary>Block size, in bytes.</summary>
        [JsonProperty("tx_size")]
        public int Size { get; init; }
    }
}
