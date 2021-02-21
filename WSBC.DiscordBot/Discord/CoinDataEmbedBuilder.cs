using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Discord;
using Microsoft.Extensions.Options;

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
            EmbedBuilder builder = new EmbedBuilder();
            builder.Title = data.Name;
            builder.Url = this._options.CoinURL;
            builder.AddField("Current Supply", data.Supply.ToString("0", CultureInfo.InvariantCulture), inline: true);
            builder.AddField("Market Cap", $"${data.MarketCap.ToString("0.00", CultureInfo.InvariantCulture)}", inline: true);
            builder.AddField("BTC Value", data.BtcPrice, inline: true);
            builder.AddField("Network", BuildNetworkFieldText(data), inline: false);
            builder.AddField("Last Block", BuildLatestBlockFieldText(data), inline: false);
            builder.WithThumbnailUrl(this._options.CoinIconURL);
            builder.WithFooter("Data provided by WSBC Explorer and TxBit", this._options.CoinIconURL);
            builder.WithCurrentTimestamp();

            // message dependant stuff
            if (message != null)
                builder.WithAuthor(message.Author);

            return builder.Build();
        }

        private static string BuildNetworkFieldText(CoinData data)
        {
            return $"***Hashrate***: {TrimUnits(data.Hashrate, new string[] { "H", "kH", "MH", "GH", "TH", "PH" })}/s\n" +
                $"***Difficulty***: {data.Difficulty.ToString("N0", CultureInfo.InvariantCulture)}\n" +
                $"***Block Height***: {data.BlockHeight}\n" +
                $"***Transactions***: {data.TransactionsCount.ToString("N0", CultureInfo.InvariantCulture)}\n" +
                $"***Block Time***: {data.TargetBlockTime}sec";
        }

        private static string BuildLatestBlockFieldText(CoinData data)
        {
            return $"***Hash***: {data.TopBlockHash}\n" +
                $"***Reward***: {data.BlockReward} {data.Code}\n" +
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
