using System.Globalization;
using System.Linq;
using System.Text;
using Discord;
using Microsoft.Extensions.Options;
using WSBC.ChatBots.Token;

namespace WSBC.ChatBots.Discord.Services
{
    class TokenDataEmbedBuilder : ITokenDataEmbedBuilder
    {
        private readonly TokenOptions _options;
        private readonly NumberFormatInfo _priceFormatProvider;
        private const string _priceFormatShort = "#,0.00##";
        private const string _priceFormatLong = "#,0.00####";

        public TokenDataEmbedBuilder(IOptionsSnapshot<TokenOptions> options)
        {
            this._options = options.Value;

            this._priceFormatProvider = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            this._priceFormatProvider.NumberGroupSeparator = " ";
            this._priceFormatProvider.CurrencyGroupSeparator = " ";
            this._priceFormatProvider.PercentGroupSeparator = " ";
        }

        public Embed Build(TokenData data, IMessage message)
        {
            EmbedBuilder builder = this.CreateDefaultEmbed(message);
            builder.Title = "WallStreetBets Token";
            builder.Url = "https://bscscan.com/token/0x8244609023097aef71c702ccbaefc0bde5b48694";
            string change = $"{(data.Change >= 0 ? "+" : string.Empty)}{data.Change:0.##}";
            string priceUSD = data.Price.ToString(_priceFormatShort, _priceFormatProvider);
            string volumeWSBT = data.Volume.ToString(_priceFormatShort, _priceFormatProvider);
            string volumeUSD = ((decimal)data.Volume * data.Price).ToString(_priceFormatShort, _priceFormatProvider);
            builder.AddField("Contract Address", this._options.ContractAddress, inline: false);
            builder.AddField("USD Value", $"${priceUSD} ({change}%)", inline: true);
            builder.AddField("Volume", $"{volumeWSBT} (${volumeUSD})", inline: true);
            builder.AddField("Price Chart", $"[LiveCoinWatch](https://www.livecoinwatch.com/price/WallStreetBetsToken-WSBT)", inline: false);
            builder.AddField("Exchanges", this.BuildExchangesFieldText(), inline: false);

            return builder.Build();
        }

        private EmbedBuilder CreateDefaultEmbed(IMessage message)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithThumbnailUrl(this._options.IconURL);
            builder.WithFooter("Data provided by STEX exchange", this._options.IconURL);
            builder.WithCurrentTimestamp();

            // message dependant stuff
            if (message != null)
                builder.WithAuthor(message.Author);

            return builder;
        }

        private string BuildExchangesFieldText()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ExchangeInfo exchange in this._options.Exchanges)
            {
                builder.Append($"[{exchange.DisplayName}]({exchange.URL})");
                string pairs = string.Join(", ", exchange.Pairs.Select(p => $"WSBT/{p}"));
                if (!string.IsNullOrWhiteSpace(pairs))
                    builder.Append($" - {pairs}");
                builder.Append('\n');
            }
            return builder.ToString();
        }
    }
}
