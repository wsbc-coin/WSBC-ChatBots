using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WSBC.ChatBots.Token.DexGuru;

namespace WSBC.ChatBots.Token.Services
{
    internal class TokenDataProvider : ITokenDataProvider
    {
        private readonly ITokenDataClient<DexGuruData> _dexGuruClient;
        private readonly ILogger _log;
        private readonly IOptionsMonitor<TokenOptions> _tokenOptions;
        private readonly SemaphoreSlim _lock;

        // cached data
        private TokenData _cachedTokenData;
        private DateTime _tokenDateCacheTimeUTC;

        public TokenDataProvider(ITokenDataClient<DexGuruData> dexGuruClient, ILogger<TokenDataProvider> log, IOptionsMonitor<TokenOptions> tokenOptions)
        {
            this._dexGuruClient = dexGuruClient;
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
                DexGuruData dexGuruData = await this._dexGuruClient.GetDataAsync(cancellationToken).ConfigureAwait(false);

                // aggregate all data and return
                this._cachedTokenData = new TokenData()
                {
                    LiquidityChange = dexGuruData.LiquidityChange,
                    LiquidityETH = dexGuruData.LiquidityETH,
                    LiquidityUSD = dexGuruData.LiquidityUSD,
                    PriceChange = dexGuruData.PriceChange,
                    PriceETH = dexGuruData.PriceETH,
                    PriceUSD = dexGuruData.PriceUSD,
                    Transactions = dexGuruData.Transactions,
                    TransactionsChange = dexGuruData.TransactionsChange,
                    Volume = dexGuruData.Volume,
                    VolumeChange = dexGuruData.VolumeChange,
                    VolumeETH = dexGuruData.VolumeETH,
                    VolumeUSD = dexGuruData.VolumeUSD
                };
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
