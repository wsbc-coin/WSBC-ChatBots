using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WSBC.ChatBots.Utilities;

namespace WSBC.ChatBots.Coin.MiningPoolStats
{
    /// <summary>Represents coin data as presented by MiningPoolStats API.</summary>
    public class MiningPoolStatsData
    {
        /// <summary>Network hashrate.</summary>
        [JsonProperty("hashrate")]
        public ulong NetworkHashrate { get; init; }
        /// <summary>Highest hashrate in one pool.</summary>
        [JsonProperty("maxhash")]
        public ulong HighestHashrate { get; init; }
        /// <summary>Sum of all pools' hashrates.</summary>
        [JsonProperty("poolshash")]
        public ulong TotalHashrate { get; init; }
        /// <summary>Total miners across all pools.</summary>
        [JsonProperty("poolsminers")]
        public uint TotalMiners { get; init; }
        /// <summary>Known pools.</summary>
        [JsonProperty("data")]
        public IEnumerable<PoolData> Pools { get; init; }

        /// <summary>Current network difficulty.</summary>
        [JsonProperty("difficulty")]
        public ulong Difficulty { get; init; }

        /// <summary>Block height.</summary>
        [JsonProperty("height")]
        public int BlockHeight { get; init; }
        /// <summary>Is block height OK?</summary>
        [JsonProperty("block_height_ok")]
        public bool IsBlockHeightValid { get; init; }
        /// <summary>Target block time, in seconds.</summary>
        [JsonProperty("block_time_target")]
        public int TargetBlockTime { get; init; }
        /// <summary>Average block time, in seconds.</summary>
        [JsonProperty("block_time_average")]
        public double AverageBlockTime { get; init; }

        /// <summary>Coin symbol.</summary>
        [JsonProperty("symbol")]
        public string CoinSymbol { get; init; }
        /// <summary>Coin symbol.</summary>
        [JsonProperty("algo")]
        public string Algorithm { get; init; }
        /// <summary>Time when the data was snapshotted.</summary>
        [JsonProperty("time")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTimeOffset Timestamp { get; init; }
        /// <summary>Info about release.</summary>
        [JsonProperty("latest_release")]
        public ReleaseData ReleaseInfo { get; init; }


        [JsonConstructor]
        private MiningPoolStatsData() { }

        public class ReleaseData
        {
            /// <summary>Release tag (usually version).</summary>
            [JsonProperty("tag")]
            public string Tag { get; init; }
            /// <summary>Release name.</summary>
            [JsonProperty("name")]
            public string Name { get; init; }
            /// <summary>Release timestamp.</summary>
            [JsonProperty("t")]
            [JsonConverter(typeof(UnixTimestampConverter))]
            public DateTimeOffset Timestamp { get; init; }

            [JsonConstructor]
            private ReleaseData() { }
        }

        // there's a lot data not needed and often ambiguous in this, so only parse stuff that's actually useful
        public class PoolData
        {
            /// <summary>Numeric ID of the pool.</summary>
            [JsonProperty("id")]
            public uint ID { get; init; }
            /// <summary>Name ID of the pool.</summary>
            [JsonProperty("pool_id")]
            public string Name { get; init; }
            /// <summary>URL of the pool.</summary>
            [JsonProperty("url")]
            public string URL { get; init; }
            /// <summary>Minimum payout.</summary>
            [JsonProperty("minpay")]
            public decimal MinimumPayout { get; init; }

            /// <summary>Current height.</summary>
            [JsonProperty("height")]
            public int BlockHeight { get; init; }
            /// <summary>Last found block.</summary>
            [JsonProperty("lastblock")]
            public int LastBlock { get; init; }
            /// <summary>Pool hashrate.</summary>
            [JsonProperty("hashrate")]
            public long Hashrate { get; init; }
            /// <summary>Pool solo hashrate.</summary>
            [JsonProperty("hashrate_solo")]
            public long SoloHashrate { get; init; }
            /// <summary>Pool luck (in %).</summary>
            [JsonProperty("luck")]
            public decimal Luck { get; init; }

            /// <summary>Count of active miners.</summary>
            [JsonProperty("miners")]
            public int MinersCount { get; init; }
            /// <summary>Count of active workers.</summary>
            [JsonProperty("workers")]
            public int WorkersCount { get; init; }

            [JsonConstructor]
            private PoolData() { }
        }
    }
}
