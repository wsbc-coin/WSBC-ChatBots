using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Discord;
using Microsoft.Extensions.Options;
using WSBC.DiscordBot.Explorer;

namespace WSBC.DiscordBot.Discord.Services
{
    class CoinDataEmbedBuilder : ICoinDataEmbedBuilder
    {
        private readonly WsbcOptions _options;

        public CoinDataEmbedBuilder(IOptionsSnapshot<WsbcOptions> options)
        {
            this._options = options.Value;
        }

        public Embed Build(CoinData data, IMessage message)
        {
            EmbedBuilder builder = this.CreateDefaultEmbed(message);
            builder.Title = this._options.CoinName;
            builder.Url = this._options.CoinURL;
            builder.AddField("Current Supply", data.Supply.ToString("0", CultureInfo.InvariantCulture), inline: true);
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
                $"***Reward***: {data.Transactions.First(tx => tx.IsCoinbase).OutputsSum} {this._options.CoinCode}\n" +
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
                $"***Fee***: {data.Fee} {this._options.CoinCode}\n" +
                $"***Is Reward?***: {(data.IsCoinbase ? $"Yes ({data.OutputsSum} {this._options.CoinCode})" : "No")}\n" +
                $"***Size***: {TrimUnits(data.Size, new string[] { "B", "kB", "MB", "GB", "TB", "PB" })}\n" +
                $"***Confirmations***: {data.ConfirmationsCount}\n" +
                $"***Created***: {(DateTimeOffset.UtcNow - data.Timestamp).ToDisplayString()} ago";
            return builder.Build();
        }

        private EmbedBuilder CreateDefaultEmbed(IMessage message)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithThumbnailUrl(this._options.CoinIconURL);
            builder.WithFooter("Data provided by WSBC Explorer and TxBit", this._options.CoinIconURL);
            builder.WithCurrentTimestamp();

            // message dependant stuff
            if (message != null)
                builder.WithAuthor(message.Author);

            return builder;
        }

        private string BuildNetworkFieldText(CoinData data)
        {
            return $"***Hashrate***: {TrimUnits(data.Hashrate, new string[] { "H", "kH", "MH", "GH", "TH", "PH" })}/s\n" +
                $"***Difficulty***: {data.Difficulty.ToString("N0", CultureInfo.InvariantCulture)}\n" +
                $"***Block Height***: {data.BlockHeight}\n" +
                $"***Transactions***: {data.TransactionsCount.ToString("N0", CultureInfo.InvariantCulture)}\n" +
                $"***Block Time***: {data.TargetBlockTime}sec";
        }

        private string BuildLatestBlockFieldText(CoinData data)
        {
            return $"***Hash***: {data.TopBlockHash}\n" +
                $"***Height***: {data.BlockHeight - 1}\n" +
                $"***Reward***: {data.BlockReward} {this._options.CoinCode}\n" +
                $"***Created***: {(DateTimeOffset.UtcNow - data.LastBlockTime).Value.ToDisplayString()} ago";
        }

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
