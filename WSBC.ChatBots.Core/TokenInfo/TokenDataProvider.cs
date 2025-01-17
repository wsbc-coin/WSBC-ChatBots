﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WSBC.ChatBots.Token.Stex;

namespace WSBC.ChatBots.Token.Services
{
    internal class TokenDataProvider : ITokenDataProvider
    {
        private readonly ITokenDataClient<StexData> _dataClient;
        private readonly ILogger _log;
        private readonly IOptionsMonitor<TokenOptions> _tokenOptions;
        private readonly SemaphoreSlim _lock;

        // cached data
        private TokenData _cachedTokenData;
        private DateTime _tokenDateCacheTimeUTC;

        public TokenDataProvider(ITokenDataClient<StexData> dataClient, ILogger<TokenDataProvider> log, IOptionsMonitor<TokenOptions> tokenOptions)
        {
            this._dataClient = dataClient;
            this._log = log;
            this._tokenOptions = tokenOptions;
            this._lock = new SemaphoreSlim(1, 1);
        }

        public async Task<TokenData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            await this._lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // attempt to get cached first to avoid hammering APIs
                if (_cachedTokenData != null && DateTime.UtcNow < this._tokenDateCacheTimeUTC + this._tokenOptions.CurrentValue.DataCacheLifetime)
                {
                    this._log.LogTrace("Found valid cached token data, skipping APIs request");
                    return _cachedTokenData;
                }

                this._log.LogInformation("Downloading all token data");

                // download data
                StexData data = await this._dataClient.GetDataAsync(cancellationToken).ConfigureAwait(false);

                // aggregate all data and return
                this._cachedTokenData = new TokenData()
                {
                    Price = data?.LastPrice ?? default,
                    Volume = data?.Volume ?? default,
                    Change = data?.Change ?? default
                };
                this._log.LogDebug("Token data retrieved. Price is {Price}", this._cachedTokenData.Price);
                this._tokenDateCacheTimeUTC = DateTime.UtcNow;
                return this._cachedTokenData;
            }
            finally
            {
                try { this._lock.Release(); } catch { }
            }
        }
    }
}
