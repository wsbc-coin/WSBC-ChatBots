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
            this._handler.Register("/chart", "Gets links to price charts", CmdChart);
            this._handler.Register("/price", "Gets current WSBT price (according to STEX.com)", CmdPrice);
            this._handler.Register("/volume", "Gets WSBT trading volume (according to STEX.com)", CmdVolume);
        }

        private async void CmdAddress(ITelegramBotClient client, Message msg)
        {
            string text = TelegramMardown.EscapeV2($"*WSBT Contract Address*: `{this._tokenOptions.CurrentValue.ContractAddress}`");
            await client.SendTextMessageAsync(msg.Chat.Id, text, ParseMode.MarkdownV2, 
                cancellationToken: this._cts.Token).ConfigureAwait(false);
        }

        private async void CmdPrice(ITelegramBotClient client, Message msg)
        {
            try
            {
                TokenData data = await this._tokenDataProvider.GetDataAsync(this._cts.Token).ConfigureAwait(false);
                if (data == null || data.Price == 0)
                {
                    await SendFailedRetrievingAsync(client, msg).ConfigureAwait(false);
                    return;
                }

                string priceUSD = data.Price.ToString(_priceFormatShort, _priceFormatProvider);
                string change = $"{(data.Change >= 0 ? "+" : string.Empty)}{data.Change:0.##}";
                string text = TelegramMardown.EscapeV2($"In last trade, 1 WSBT = *${priceUSD}* \\(*{change}%*\\)\n" +
                    "_Data provided by [STEX](https://app.stex.com/en/trading/pair/USDT/WSBT/5). For exchange-independent price, visit [LiveCoinWatch](https://www.livecoinwatch.com/price/WallStreetBetsToken-WSBT)_.");
                await client.SendTextMessageAsync(msg.Chat.Id, text, ParseMode.MarkdownV2, 
                    disableWebPagePreview: true, cancellationToken: this._cts.Token).ConfigureAwait(false);
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

                string volumeWSBT = data.Volume.ToString(_priceFormatShort, _priceFormatProvider);
                string volumeUSD = ((decimal)data.Volume * data.Price).ToString(_priceFormatShort, _priceFormatProvider);
                string text = TelegramMardown.EscapeV2($"WSBT traded on STEX in last 24 hours:\n*{volumeWSBT} \\(${volumeUSD}\\)*\n" +
                    "_Data provided by [STEX](https://app.stex.com/en/trading/pair/USDT/WSBT/5). For exchange-independent price, visit [LiveCoinWatch](https://www.livecoinwatch.com/price/WallStreetBetsToken-WSBT)_.");
                await client.SendTextMessageAsync(msg.Chat.Id, text, ParseMode.MarkdownV2, 
                    disableWebPagePreview: true, cancellationToken: this._cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.LogAsError(this._log, "Exception occured when retrieving token data"))
            {
                await SendFailedRetrievingAsync(client, msg).ConfigureAwait(false);
            }
        }

        private async void CmdChart(ITelegramBotClient client, Message msg)
        {
            string text = TelegramMardown.EscapeV2("You can view live price chart on [LiveCoinWatch](https://www.livecoinwatch.com/price/WallStreetBetsToken-WSBT)!");
            await client.SendTextMessageAsync(msg.Chat.Id, text, ParseMode.MarkdownV2, 
                disableWebPagePreview: false, disableNotification: true, replyToMessageId: msg.MessageId, cancellationToken: this._cts.Token).ConfigureAwait(false);
        }

        private Task SendFailedRetrievingAsync(ITelegramBotClient client, Message msg)
            => client.SendTextMessageAsync(msg.Chat.Id, "\u274C Failed retrieving token data", cancellationToken: this._cts.Token);

        public void Dispose()
        {
            try { this._cts?.Cancel(); } catch { }
            try { this._cts?.Dispose(); } catch { }
        }
    }
}
