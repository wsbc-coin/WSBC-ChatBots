using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using WSBC.ChatBots.Memes;
using File = System.IO.File;

namespace WSBC.ChatBots.Telegram.Commands
{
    class MemesCommands : IDisposable
    {
        private readonly IRandomFilePicker _randomFile;
        private readonly ICommandsHandler _handler;
        private readonly IOptionsMonitor<MemesOptions> _memesOptions;
        private readonly ILogger _log;
        private readonly CancellationTokenSource _cts;

        public MemesCommands(IRandomFilePicker randomFile, ICommandsHandler handler, IOptionsMonitor<MemesOptions> memesOptions, ILogger<TokenCheckCommands> log)
        {
            this._randomFile = randomFile;
            this._handler = handler;
            this._memesOptions = memesOptions;
            this._log = log;
            this._cts = new CancellationTokenSource();


            this._handler.Register("/lambo", "Shows the Lambo you'll buy with WSBT! \uD83D\uDE0E", CmdLambo);
        }

        private void CmdLambo(CommandContext context)
            => _ = this.SendRandomFileAsync(_memesOptions.CurrentValue.LamboPath, $"Lambo for you, {new TelegramPing(context.Message.From)}! \uD83D\uDE0E", context);

        private async Task SendRandomFileAsync(string path, string text, CommandContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentNullException(nameof(path));

                string file = _randomFile.Pick(path);
                using FileStream stream = File.OpenRead(file);

                await context.Client.SendPhotoAsync(context.ChatID, new InputOnlineFile(stream), TelegramMardown.EscapeV2(text), ParseMode.MarkdownV2, cancellationToken: this._cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.LogAsError(this._log, "Exception when picking a random meme from path '{Path}'", path))
            {
                await context.Client.SendTextMessageAsync(context.ChatID, "\u274C Failed sending meme, please contact the developers", cancellationToken: this._cts.Token).ConfigureAwait(false);
                return;
            }
        }

        public void Dispose()
        {
            try { this._cts?.Cancel(); } catch { }
            try { this._cts?.Dispose(); } catch { }
        }
    }
}
