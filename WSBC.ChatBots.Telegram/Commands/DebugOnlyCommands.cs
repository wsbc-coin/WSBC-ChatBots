using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using WSBC.ChatBots.Telegram.Autopost;

namespace WSBC.ChatBots.Telegram.Commands
{
    class DebugOnlyCommands : IDisposable
    {
        private readonly ILogger _log;
        private readonly CancellationTokenSource _cts;
        private readonly IAutopostService _autopost;

        public DebugOnlyCommands(ICommandsHandler handler, IAutopostService autopost, ILogger<DebugOnlyCommands> log)
        {
            if (!Debugger.IsAttached)
                return;

            this._log = log;
            this._autopost = autopost;
            this._cts = new CancellationTokenSource();

            this._log.LogDebug("Debugger attached, registering debug-only commands");
            handler.Register("/autopost_test", this.CmdNextAutopost);
        }

        private async void CmdNextAutopost(CommandContext context)
            => await this._autopost.SendNextAsync(context.ChatID, this._cts.Token).ConfigureAwait(false);

        public void Dispose()
        {
            try { this._cts?.Cancel(); } catch { }
            try { this._cts?.Dispose(); } catch { }
        }
    }
}
