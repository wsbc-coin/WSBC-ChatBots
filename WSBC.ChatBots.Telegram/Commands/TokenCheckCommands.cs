using System;
using System.Threading;
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

        public TokenCheckCommands(ITokenDataProvider tokenDataProvider, ICommandsHandler handler, IOptionsMonitor<TokenOptions> tokenOptions,  ILogger<TokenCheckCommands> log)
        {
            this._tokenDataProvider = tokenDataProvider;
            this._handler = handler;
            this._tokenOptions = tokenOptions;
            this._log = log;
            this._cts = new CancellationTokenSource();

            this._handler.Register("/contract", "Gets WSBT token address", CmdAddress);
        }

        private async void CmdAddress(ITelegramBotClient client, Message msg)
        {
            await client.SendTextMessageAsync(msg.Chat.Id, $"<b>WSBT Contract Address</b>: <i>{this._tokenOptions.CurrentValue.ContractAddress}</i>", ParseMode.Html, 
                cancellationToken: this._cts.Token).ConfigureAwait(false);
        }

        public void Dispose()
        {
            try { this._cts?.Cancel(); } catch { }
            try { this._cts?.Dispose(); } catch { }
        }
    }
}
