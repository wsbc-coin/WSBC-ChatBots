﻿using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WSBC.ChatBots.Telegram.Autopost
{
    class AutopostService : IDisposable
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
            CancellationToken cancellationToken = this._cts.Token;
            await this._lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
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

                // counter check
                if (!CheckCounter(msg.MessageId, options.MessageRate))
                    return;

                // calculate index - circ back to 0 if used up all messages
                if (this._messageIndex >= options.Messages.Length)
                    this._messageIndex = 0;
                int index = this._messageIndex++;

                // send
                try
                {
                    string text = options.Messages[index];
                    this._log.LogDebug("Sending message index {Index}: {Text}", index, text);
                    await this._client.Client.SendTextMessageAsync(options.ChannelID, text, ParseMode.Html, cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex.LogAsError(this._log, "Exception when sending a message index {Index}", index)) { }
            }
            finally
            {
                this._lock.Release();
            }
        }

        private bool CheckCounter(int msgID, uint rate)
        {
            this._receivedCounter++;
            bool shouldPost = this._receivedCounter >= rate;
            if (shouldPost)
                this._receivedCounter = 0;
            this._log.LogTrace("Received message {ID}, counter is now {Counter}", msgID, this._receivedCounter);
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