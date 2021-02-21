using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WSBC.DiscordBot.Explorer;
using WSBC.DiscordBot.TxBit;

namespace WSBC.DiscordBot.Services
{
    class CoinDataProvider : ICoinDataProvider, IDisposable
    {
        private readonly ICoinDataClient<TxBitData> _txbitClient;
        private readonly IExplorerDataClient _explorerClient;
        private readonly ILogger _log;
        private readonly IOptionsMonitor<CachingOptions> _cachingOptions;
        private readonly SemaphoreSlim _lock;

        // cached data
        private CoinData _cachedResult;
        private DateTime _lastDownloadTimeUTC;

        public CoinDataProvider(ICoinDataClient<TxBitData> txbitClient, IExplorerDataClient explorerClient,
            ILogger<CoinDataProvider> log, IOptionsMonitor<CachingOptions> cachingOptions)
        {
            this._txbitClient = txbitClient;
            this._explorerClient = explorerClient;
            this._log = log;
            this._cachingOptions = cachingOptions;
            this._lock = new SemaphoreSlim(1, 1);
        }

        public async Task<CoinData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            await this._lock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // attempt to get cached first to avoid hammering APIs
                if (_cachedResult != null && DateTime.UtcNow < this._lastDownloadTimeUTC + this._cachingOptions.CurrentValue.DataCacheLifetime)
                {
                    this._log.LogTrace("Found valid cached coin data, skipping APIs request");
                    return _cachedResult;
                }

                this._log.LogInformation("Downloading all coin data");

                // start downloading
                Task<ExplorerNetworkData> explorerNetworkTask = this._explorerClient.GetNetworkDataAsync(cancellationToken);
                Task<ExplorerEmissionData> explorerEmissionTask = this._explorerClient.GetEmissionDataAsync(cancellationToken);
                Task<TxBitData> txbitTask = this._txbitClient.GetDataAsync(cancellationToken);
                // use explorer result to get last block data
                ExplorerNetworkData explorerNetworkData = await explorerNetworkTask.ConfigureAwait(false);
                Task<ExplorerBlockData> explorerBlockTask = this._explorerClient.GetBlockDataAsync(explorerNetworkData.TopBlockHash, cancellationToken);
                // await all remaining data
                TxBitData txbitData = await txbitTask.ConfigureAwait(false);
                ExplorerBlockData explorerBlockData = await explorerBlockTask.ConfigureAwait(false);
                ExplorerEmissionData explorerEmissionData = await explorerEmissionTask.ConfigureAwait(false);

                // aggregate all data and return
                this._cachedResult = new CoinData(txbitData.CurrencyName, txbitData.CurrencyCode)
                {
                    Supply = explorerEmissionData.CirculatingSupply,
                    MarketCap = txbitData.MarketCap,
                    BtcPrice = txbitData.BidPrice,
                    BlockHeight = explorerNetworkData.BlockHeight,
                    BlockReward = explorerBlockData.Transactions.First(tx => tx.IsCoinbase).OutputsSum,
                    Difficulty = explorerNetworkData.Difficulty,
                    Hashrate = explorerNetworkData.Hashrate,
                    LastBlockTime = explorerBlockData.Timestamp,
                    TargetBlockTime = explorerNetworkData.TargetBlockTime,
                    TopBlockHash = explorerBlockData.Hash,
                    TransactionsCount = explorerNetworkData.TransactionsCount
                };
                this._lastDownloadTimeUTC = DateTime.UtcNow;
                return this._cachedResult;
            }
            finally
            {
                try { this._lock.Release(); } catch { }
            }
        }

        public void Dispose()
        {
            try { this._lock?.Dispose(); } catch { }
        }
    }
}
