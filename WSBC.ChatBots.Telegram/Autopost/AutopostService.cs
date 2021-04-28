using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using File = System.IO.File;

namespace WSBC.ChatBots.Telegram.Autopost
{
    class AutopostService : IAutopostService, IDisposable
    {
        private readonly ITelegramClient _client;
        private readonly ILogger _log;
        private readonly IOptionsMonitor<AutopostOptions> _options;
        private readonly SemaphoreSlim _lock;
        private readonly CancellationTokenSource _cts;

        private int _messageIndex;
        private int _receivedCounter;

        public AutopostService(ITelegramClient client, ILogger<AutopostService> log, IOptionsMonitor<AutopostOptions> options)
        {
            this._client = client;
            this._log = log;
            this._options = options;
            this._lock = new SemaphoreSlim(1, 1);
            this._cts = new CancellationTokenSource();

            this._client.MessageReceived += OnMessageReceived;
        }

        private async void OnMessageReceived(object sender, MessageEventArgs e)
        {
            this._log.LogTrace("Checking message {ID}", e.Message.MessageId);
            Message msg = e.Message;
            AutopostOptions options = this._options.CurrentValue;
            // run checks
            if (options.ChannelID == 0 || options.MessageRate == 0 || options.Messages?.Any() != true)
                return;
            if (msg.Type is not MessageType.Text or MessageType.Sticker or MessageType.Photo or MessageType.Video && msg.Animation == null)
                return;
            if (msg.Chat?.Id != options.ChannelID)
                return;
            await SendNextInternalAsync(msg.Chat.Id, false, this._cts.Token).ConfigureAwait(false);
        }

        public Task SendNextAsync(long chatID, CancellationToken cancellationToken = default)
            => this.SendNextInternalAsync(chatID, true, cancellationToken);

        private async Task SendNextInternalAsync(long chatID, bool force, CancellationToken cancellationToken)
        {
            await this._lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                AutopostOptions options = this._options.CurrentValue;
                // counter check
                if (!force && !CheckCounter(options.MessageRate))
                    return;

                // calculate index - circ back to 0 if used up all messages
                if (this._messageIndex >= options.Messages.Length)
                    this._messageIndex = 0;
                int index = this._messageIndex++;

                // send
                try
                {
                    AutopostMessage message = options.Messages[index];
                    InputOnlineFile file = GetMessageImage(message);
                    string content = message.Content;
                    if (string.IsNullOrWhiteSpace(content) && file == null)
                    {
                        this._log.LogError("Cannot send message index {Index} - no content or file", index);
                        return;
                    }
                    ParseMode parseMode = message.ParsingMode ?? options.DefaultParsingMode;
                    if (parseMode == ParseMode.MarkdownV2)
                        content = TelegramMardown.EscapeV2(content);
                    this._log.LogDebug("Sending message index {Index}: {Text}", index, content);
                    if (file == null)
                    {
                        await this._client.Client.SendTextMessageAsync(chatID, content, parseMode,
                            disableWebPagePreview: message.DisableWebPreview,
                            disableNotification: true,
                            replyToMessageId: message.ReplyTo = 0,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        await this._client.Client.SendPhotoAsync(chatID, file, content, parseMode,
                            disableNotification: true,
                            replyToMessageId: message.ReplyTo = 0,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (Exception ex) when (ex.LogAsError(this._log, "Exception when sending a message index {Index}", index)) { }
            }
            finally
            {
                this._lock.Release();
            }
        }

        private InputOnlineFile GetMessageImage(AutopostMessage message)
        {
            if (!string.IsNullOrWhiteSpace(message.ImagePath))
            {
                this._log.LogDebug("Attaching local file {File} to autopost message", message.ImagePath);
                try
                {
                    using FileStream stream = File.OpenRead(message.ImagePath);
                    return new InputOnlineFile(stream);
                }
                catch (Exception ex) when (ex.LogAsError(this._log, "Exception when opening file {File}", message.ImagePath))
                {
                    return null;
                }
            }
            if (!string.IsNullOrWhiteSpace(message.ImageURL))
                return new InputOnlineFile(message.ImageURL);
            return null;
        }

        private bool CheckCounter(uint rate)
        {
            this._receivedCounter++;
            bool shouldPost = this._receivedCounter >= rate;
            if (shouldPost)
                this._receivedCounter = 0;
            this._log.LogTrace("Received message, counter is now {Counter}", this._receivedCounter);
            return shouldPost;
        }

        public void Dispose()
        {
            try { this._client.MessageReceived -= OnMessageReceived; } catch { }
            try { this._cts?.Cancel(); } catch { }
            try { this._cts?.Dispose(); } catch { }
            try { this._lock?.Dispose(); } catch { }
        }
    }
}
