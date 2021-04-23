using Newtonsoft.Json;
using WSBC.ChatBots.Discord.Serialization;

namespace WSBC.ChatBots.Discord.Explorer
{
    public class ExplorerEmissionData
    {
        /// <summary>Total blocks on the blockchain.</summary>
        [JsonProperty("blk_no")]
        public int BlocksNumber { get; init; }
        /// <summary>Total coins in supply.</summary>
        [JsonProperty("coinbase")]
        [JsonConverter(typeof(TransactionAmountConverter))]
        public decimal CirculatingSupply { get; init; }
        [JsonProperty("fee")]
        [JsonConverter(typeof(TransactionAmountConverter))]
        public decimal Fee { get; init; }

        [JsonConstructor]
        private ExplorerEmissionData() { }
    }
}
