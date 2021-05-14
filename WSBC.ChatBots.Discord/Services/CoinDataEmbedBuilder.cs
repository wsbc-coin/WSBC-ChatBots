using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Discord;
using Microsoft.Extensions.Options;
using WSBC.ChatBots.Coin;
using WSBC.ChatBots.Coin.Explorer;
using WSBC.ChatBots.Coin.MiningPoolStats;

namespace WSBC.ChatBots.Discord.Services
{
    class CoinDataEmbedBuilder : ICoinDataEmbedBuilder
    {
        private readonly MiningPoolStatsOptions _poolStatsOptions;
        private readonly CoinOptions _options;

        public CoinDataEmbedBuilder(IOptionsSnapshot<CoinOptions> options, IOptionsSnapshot<MiningPoolStatsOptions> poolStatsOptions)
        {
            this._options = options.Value;
            this._poolStatsOptions = poolStatsOptions.Value;
        }

        public Embed Build(CoinData data, IMessage message)
        {
            EmbedBuilder builder = this.CreateDefaultEmbed(message);
            builder.Title = this._options.CoinName;
            builder.Url = this._options.CoinURL;
            builder.AddField("Current Supply", data.Supply.ToString("N0", CultureInfo.InvariantCulture), inline: true);
            builder.AddField("Market Cap", $"${data.MarketCap.ToString("0.00", CultureInfo.InvariantCulture)}", inline: true);
            builder.AddField("BTC Value", data.BtcPrice, inline: true);
            builder.AddField("Network", this.BuildNetworkFieldText(data), inline: false);
            builder.AddField("Last Block", this.BuildLatestBlockFieldText(data), inline: false);

            return builder.Build();
        }

        public Embed Build(ExplorerBlockData data, IMessage message)
        {
            EmbedBuilder builder = this.CreateDefaultEmbed(message);
            builder.Title = $"Block {data.Height}";
            builder.Url = $"http://explorer.wallstreetbetsbros.com/block/{data.Height}";
            builder.Description = $"***Height***: {data.Height} ({data.TopBlockHeight - data.Height + 1} blocks ago)\n" +
                $"***Hash***: {data.Hash}\n" +
                $"***Difficulty***: {data.Difficulty.ToString("N0", CultureInfo.InvariantCulture)}\n" +
                $"***Reward***: {data.Transactions.First(tx => tx.IsCoinbase).OutputsSum} {this._options.CoinTicker}\n" +
                $"***Size***: {TrimUnits(data.Size, new string[] { "B", "kB", "MB", "GB", "TB", "PB" })}\n" + 
                $"***Transactions***: {data.Transactions.Count()}\n" +
                $"***Created***: {(DateTimeOffset.UtcNow - data.Timestamp).ToDisplayString()} ago";
            return builder.Build();
        }

        public Embed Build(ExplorerTransactionData data, IMessage message)
        {
            EmbedBuilder builder = this.CreateDefaultEmbed(message);
            builder.Title = $"Transaction {data.Hash}";
            builder.Url = $"http://explorer.wallstreetbetsbros.com/tx/{data.Hash}";
            builder.Description = $"***Hash***: {data.Hash}\n" + 
                $"***Block Height***: {data.BlockHeight} ({data.TopBlockHeight - data.BlockHeight + 1} blocks ago)\n" +
                $"***Fee***: {data.Fee} {this._options.CoinTicker}\n" +
                $"***Is Reward?***: {(data.IsCoinbase ? $"Yes ({data.OutputsSum} {this._options.CoinTicker})" : "No")}\n" +
                $"***Size***: {TrimUnits(data.Size, new string[] { "B", "kB", "MB", "GB", "TB", "PB" })}\n" +
                $"***Confirmations***: {data.ConfirmationsCount}\n" +
                $"***Created***: {(DateTimeOffset.UtcNow - data.Timestamp).ToDisplayString()} ago";
            return builder.Build();
        }

        public Embed Build(MiningPoolStatsData data, IMessage message)
        {
            EmbedBuilder builder = this.CreateDefaultEmbed(message);
            builder.WithFooter("Data provided by PoolMiningStats", this._options.IconURL);
            builder.WithTimestamp(data.Timestamp);
            StringBuilder poolListBuilder = new StringBuilder();
            int i = 0;
            foreach (MiningPoolStatsData.PoolData pool in data.Pools.OrderByDescending(p => p.Hashrate).Take(10))
                poolListBuilder.AppendFormat("{0}. ***[{1}]({2})*** - **{3}**, Luck: *{4}%*, Workers: *{5}*\n",
                    ++i, pool.Name, pool.URL, BuildHashrateString(pool.Hashrate), pool.Luck.ToString("0.#", CultureInfo.InvariantCulture), pool.WorkersCount);
            builder.AddField("Top Pools", poolListBuilder.ToString(), inline: false);
            builder.AddField("Important note",
                "Everyone joining the same pool does this coin no good. To better support this project (and lambos <:emoji_51:808859069779279883>), please consider NOT joining the top pool.\n" +
                "By spreading across numerous pools, you support decentralization - this will make the coin be worth much more! <:emoji_54:808859529071820820>", inline: false);
            builder.AddField("Need more info?",
                $"For more information about mining pools, as well as non-top mining pools, check out [PoolMiningStats Website]({this._poolStatsOptions.WebsiteURL})!", inline: false);
            return builder.Build();
        }

        private EmbedBuilder CreateDefaultEmbed(IMessage message)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithThumbnailUrl(this._options.IconURL);
            builder.WithFooter("Data provided by WSBC Explorer and TxBit", this._options.IconURL);
            builder.WithCurrentTimestamp();

            // message dependant stuff
            if (message != null)
                builder.WithAuthor(message.Author);

            return builder;
        }

        private string BuildNetworkFieldText(CoinData data)
        {
            return $"***Hashrate***: {BuildHashrateString(data.Hashrate)}/s\n" +
                $"***Difficulty***: {data.Difficulty.ToString("N0", CultureInfo.InvariantCulture)}\n" +
                $"***Block Height***: {data.BlockHeight}\n" +
                $"***Transactions***: {data.TransactionsCount.ToString("N0", CultureInfo.InvariantCulture)}\n" +
                $"***Block Time***: {data.TargetBlockTime}sec";
        }

        private string BuildLatestBlockFieldText(CoinData data)
        {
            return $"***Hash***: {data.TopBlockHash}\n" +
                $"***Height***: {data.BlockHeight - 1}\n" +
                $"***Reward***: {data.BlockReward} {this._options.CoinTicker}\n" +
                $"***Created***: {(DateTimeOffset.UtcNow - data.LastBlockTime).Value.ToDisplayString()} ago";
        }

        private static string BuildHashrateString(long hashrate)
            => $"{TrimUnits(hashrate, new string[] { "H", "kH", "MH", "GH", "TH", "PH" })}/s";

        private static string TrimUnits(double value, ICollection<string> units)
        {
            if (units == null || !units.Any())
                throw new ArgumentException("Units are required", nameof(units));

            string resultUnit = units.First();
            for (int i = 1; i < units.Count; i++)
            {
                if (value < 1000)
                    break;
                value /= 1000;
                resultUnit = units.ElementAt(i);
            }
            return $"{value.ToString("0.##", CultureInfo.InvariantCulture)} {resultUnit}";
        }
    }
}
