using Discord;
using Microsoft.Extensions.Options;

namespace WSBC.DiscordBot.Discord
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
            builder.AddField("Current Supply", data.Supply.ToString("0"), inline: true);
            builder.AddField("Market Cap", $"${data.MarketCap:0.00}", inline: true);
            builder.AddField("Block Height", data.BlockHeight, inline: false);
            builder.AddField("BTC Value", data.BtcPrice, inline: true);
            builder.WithThumbnailUrl(this._options.CoinIconURL);
            builder.WithFooter(null, this._options.CoinIconURL);
            builder.WithCurrentTimestamp();

            // message dependant stuff
            if (message != null)
                builder.WithAuthor(message.Author);

            return builder.Build();
        }
    }
}
