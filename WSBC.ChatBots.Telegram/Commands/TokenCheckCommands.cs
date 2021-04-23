using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WSBC.ChatBots.Token;

namespace WSBC.ChatBots.Telegram.Commands
{
    class TokenCheckCommands : IDisposable
    {
        private readonly ITokenDataProvider _tokenDataProvider;
        private readonly ICommandsHandler _handler;
        private readonly IOptionsMonitor<TokenOptions> _tokenOptions;
        private readonly ILogger _log;
        private readonly CancellationTokenSource _cts;

        private readonly NumberFormatInfo _priceFormatProvider;
        private const string _priceFormatShort = "#,0.00##";
        private const string _priceFormatLong = "#,0.00####";

        public TokenCheckCommands(ITokenDataProvider tokenDataProvider, ICommandsHandler handler, IOptionsMonitor<TokenOptions> tokenOptions, ILogger<TokenCheckCommands> log)
        {
            this._tokenDataProvider = tokenDataProvider;
            this._handler = handler;
            this._tokenOptions = tokenOptions;
            this._log = log;
            this._cts = new CancellationTokenSource();

            this._priceFormatProvider = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            this._priceFormatProvider.NumberGroupSeparator = " ";
            this._priceFormatProvider.CurrencyGroupSeparator = " ";
            this._priceFormatProvider.PercentGroupSeparator = " ";

            this._handler.Register("/contract", "Gets WSBT token address", CmdAddress);
            this._handler.Register("/price", "Gets current WSBT price (according to Dex.Guru)", CmdPrice);
            this._handler.Register("/volume", "Gets WSBT trading volume (according to Dex.Guru)", CmdVolume);
        }

        private async void CmdAddress(ITelegramBotClient client, Message msg)
        {
            await client.SendTextMessageAsync(msg.Chat.Id, $"<b>WSBT Contract Address</b>: <i>{this._tokenOptions.CurrentValue.ContractAddress}</i>", ParseMode.Html, 
                cancellationToken: this._cts.Token).ConfigureAwait(false);
        }

        private async void CmdPrice(ITelegramBotClient client, Message msg)
        {
            try
            {
                TokenData data = await this._tokenDataProvider.GetDataAsync(this._cts.Token).ConfigureAwait(false);
                if (data == null || data.PriceETH == 0)
                {
                    await SendFailedRetrievingAsync(client, msg).ConfigureAwait(false);
                    return;
                }

                await client.SendTextMessageAsync(msg.Chat.Id, $"1 WSBT = <b>${data.PriceUSD.ToString(_priceFormatShort, _priceFormatProvider)}</b> ({data.PriceETH.ToString(_priceFormatLong, _priceFormatProvider)} ETH)<pre> </pre>24h change: {data.PriceChange:0.##%}",
                    ParseMode.Html, cancellationToken: this._cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.LogAsError(this._log, "Exception occured when retrieving token data"))
            {
                await SendFailedRetrievingAsync(client, msg).ConfigureAwait(false);
            }
        }

        private async void CmdVolume(ITelegramBotClient client, Message msg)
        {
            try
            {
                TokenData data = await this._tokenDataProvider.GetDataAsync(this._cts.Token).ConfigureAwait(false);
                if (data == null || data.Volume == 0)
                {
                    await SendFailedRetrievingAsync(client, msg).ConfigureAwait(false);
                    return;
                }

                await client.SendTextMessageAsync(msg.Chat.Id, @$"WSBT traded in last 24 hours: *${data.VolumeUSD.ToString(_priceFormatShort, _priceFormatProvider)}*
({data.Volume.ToString(_priceFormatShort, _priceFormatProvider)} WSBT / {data.VolumeETH.ToString(_priceFormatShort, _priceFormatProvider)} ETH)

24h change: {data.VolumeChange:0.##%}",
                    ParseMode.Markdown, cancellationToken: this._cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.LogAsError(this._log, "Exception occured when retrieving token data"))
            {
                await SendFailedRetrievingAsync(client, msg).ConfigureAwait(false);
            }
        }

        private Task SendFailedRetrievingAsync(ITelegramBotClient client, Message msg)
            => client.SendTextMessageAsync(msg.Chat.Id, "Failed retrieving token data", cancellationToken: this._cts.Token);

        public void Dispose()
        {
            try { this._cts?.Cancel(); } catch { }
            try { this._cts?.Dispose(); } catch { }
        }
    }
}
