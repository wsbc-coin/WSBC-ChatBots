﻿using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types.Enums;
using WSBC.ChatBots.Token;
using WSBC.ChatBots.Utilities;

namespace WSBC.ChatBots.Telegram.Commands
{
    class TokenCheckCommands : IDisposable
    {
        private readonly ITokenDataProvider _tokenDataProvider;
        private readonly ICommandsHandler _handler;
        private readonly IOptionsMonitor<TokenOptions> _tokenOptions;
        private readonly ILogger _log;
        private readonly CancellationTokenSource _cts;
        private readonly PriceFormatProvider _priceFormat;

        public TokenCheckCommands(ITokenDataProvider tokenDataProvider, ICommandsHandler handler, IOptionsMonitor<TokenOptions> tokenOptions, ILogger<TokenCheckCommands> log, PriceFormatProvider priceFormatProvider)
        {
            this._tokenDataProvider = tokenDataProvider;
            this._priceFormat = priceFormatProvider;
            this._handler = handler;
            this._tokenOptions = tokenOptions;
            this._log = log;
            this._cts = new CancellationTokenSource();

            this._handler.Register("/contract", "Gets WSBT token address", CmdAddress);
            this._handler.Register("/chart", "Gets links to price charts", CmdChart);
            this._handler.Register("/price", "Gets current WSBT price (according to STEX.com)", CmdPrice);
            this._handler.Register("/volume", "Gets WSBT trading volume (according to STEX.com)", CmdVolume);
            this._handler.Register("/exchanges", "Gets list of WSBT exchanges", CmdExchanges);
            this._handler.Register("/buy", null, CmdExchanges);
        }

        private async void CmdAddress(CommandContext context)
        {
            string text = TelegramMardown.EscapeV2($"*WSBT Contract Address*: `{this._tokenOptions.CurrentValue.ContractAddress}`");
            await context.Client.SendTextMessageAsync(context.ChatID, text, ParseMode.MarkdownV2, 
                cancellationToken: this._cts.Token).ConfigureAwait(false);
        }

        private async void CmdPrice(CommandContext context)
        {
            try
            {
                TokenData data = await this._tokenDataProvider.GetDataAsync(this._cts.Token).ConfigureAwait(false);
                if (data == null || data.Price == 0)
                {
                    await SendFailedRetrievingAsync(context).ConfigureAwait(false);
                    return;
                }

                string disclaimer = "_Data provided by [STEX](https://app.stex.com/en/trading/pair/USDT/WSBT/5). For exchange-independent price, visit [LiveCoinWatch](https://www.livecoinwatch.com/price/WallStreetBetsToken-WSBT)_.";

                string change = $"{(data.Change >= 0 ? "+" : string.Empty)}{data.Change:0.##}";
                string text = null;

                if (context.Arguments != null)
                {
                    string amountArg = context.Arguments.Split(' ').First();
                    if (!decimal.TryParse(amountArg, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
                    {
                        await context.Client.SendTextMessageAsync(context.ChatID, "\u274C Invalid amount value provided.", ParseMode.Default,
                            disableWebPagePreview: true, replyToMessageId: context.MessageID, cancellationToken: this._cts.Token).ConfigureAwait(false);
                        return;
                    }

                    string priceUSD = this._priceFormat.FormatNormal(data.Price * amount);
                    text = TelegramMardown.EscapeV2($"In last trade, {amount.ToString("#,0.####", _priceFormat)} WSBT = *${priceUSD}* \\(*{change}%*\\)\n{disclaimer}");
                }
                else
                {
                    string priceUSD = this._priceFormat.FormatNormal(data.Price);
                    text = TelegramMardown.EscapeV2($"In last trade, 1 WSBT = *${priceUSD}* \\(*{change}%*\\)\n{disclaimer}");
                }

                await context.Client.SendTextMessageAsync(context.ChatID, text, ParseMode.MarkdownV2, 
                    disableWebPagePreview: true, cancellationToken: this._cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.LogAsError(this._log, "Exception occured when retrieving token data"))
            {
                await SendFailedRetrievingAsync(context).ConfigureAwait(false);
            }
        }

        private async void CmdVolume(CommandContext context)
        {
            try
            {
                TokenData data = await this._tokenDataProvider.GetDataAsync(this._cts.Token).ConfigureAwait(false);
                if (data == null || data.Volume == 0)
                {
                    await SendFailedRetrievingAsync(context).ConfigureAwait(false);
                    return;
                }

                string volumeWSBT = this._priceFormat.FormatNormal(data.Volume);
                string volumeUSD = this._priceFormat.FormatNormal((decimal)data.Volume * data.Price);
                string text = TelegramMardown.EscapeV2($"WSBT traded on STEX in last 24 hours:\n*{volumeWSBT} \\(${volumeUSD}\\)*\n" +
                    "_Data provided by [STEX](https://app.stex.com/en/trading/pair/USDT/WSBT/5). For exchange-independent price, visit [LiveCoinWatch](https://www.livecoinwatch.com/price/WallStreetBetsToken-WSBT)_.");
                await context.Client.SendTextMessageAsync(context.ChatID, text, ParseMode.MarkdownV2, 
                    disableWebPagePreview: true, cancellationToken: this._cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.LogAsError(this._log, "Exception occured when retrieving token data"))
            {
                await SendFailedRetrievingAsync(context).ConfigureAwait(false);
            }
        }

        private async void CmdChart(CommandContext context)
        {
            string text = TelegramMardown.EscapeV2("You can view live price chart on [LiveCoinWatch](https://www.livecoinwatch.com/price/WallStreetBetsToken-WSBT)!");
            await context.Client.SendTextMessageAsync(context.ChatID, text, ParseMode.MarkdownV2, 
                disableWebPagePreview: false, disableNotification: true, replyToMessageId: context.MessageID, cancellationToken: this._cts.Token).ConfigureAwait(false);
        }

        private async void CmdExchanges(CommandContext context)
        {
            TokenOptions options = this._tokenOptions.CurrentValue;
            if (options?.Exchanges?.Any() != true)
            {
                await context.Client.SendTextMessageAsync(context.ChatID, TelegramMardown.EscapeV2("No exchanges configured, please contact admin"), ParseMode.MarkdownV2,
                    disableWebPagePreview: true, disableNotification: true, replyToMessageId: context.MessageID, cancellationToken: this._cts.Token).ConfigureAwait(false);
                return;
            }

            StringBuilder builder = new StringBuilder("WSBT exchanges:\n");
            foreach (ExchangeInfo exchange in options.Exchanges)
            {
                builder.Append($"[{exchange.DisplayName}]({exchange.URL})");
                string pairs = string.Join(", ", exchange.Pairs.Select(p => $"WSBT/{p}"));
                if (!string.IsNullOrWhiteSpace(pairs))
                    builder.Append($" - {pairs}");
                builder.Append('\n');
            }
            await context.Client.SendTextMessageAsync(context.ChatID, TelegramMardown.EscapeV2(builder.ToString()), ParseMode.MarkdownV2,
                disableWebPagePreview: true, disableNotification: true, replyToMessageId: context.MessageID, cancellationToken: this._cts.Token).ConfigureAwait(false);
        }

        private Task SendFailedRetrievingAsync(CommandContext context)
            => context.Client.SendTextMessageAsync(context.ChatID, "\u274C Failed retrieving token data", cancellationToken: this._cts.Token);

        public void Dispose()
        {
            try { this._cts?.Cancel(); } catch { }
            try { this._cts?.Dispose(); } catch { }
        }
    }
}
