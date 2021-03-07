using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WSBC.DiscordBot.Explorer;
using WSBC.DiscordBot.MiningPoolStats;
using WSBC.DiscordBot.TxBit;

namespace WSBC.DiscordBot.Services
{
    class CoinDataProvider : ICoinDataProvider, IDisposable
    {
        private readonly ICoinDataClient<TxBitData> _txbitClient;
        private readonly ICoinDataClient<MiningPoolStatsData> _poolStatsClient;
        private readonly IExplorerDataClient _explorerClient;
        private readonly ILogger _log;
        private readonly IOptionsMonitor<CachingOptions> _cachingOptions;
        private readonly SemaphoreSlim _coinLock;
        private readonly SemaphoreSlim _poolsLock;

        // cached data
        private CoinData _cachedCoinData;
        private DateTime _coinDateCacheTimeUTC;
        private MiningPoolStatsData _cachedPoolStatsData;
        private DateTime _poolStatsDataCacheTimeUTC;

        public CoinDataProvider(ICoinDataClient<TxBitData> txbitClient, ICoinDataClient<MiningPoolStatsData> poolStatsClient, IExplorerDataClient explorerClient,
            ILogger<CoinDataProvider> log, IOptionsMonitor<CachingOptions> cachingOptions)
        {
            this._txbitClient = txbitClient;
            this._explorerClient = explorerClient;
            this._poolStatsClient = poolStatsClient;
            this._log = log;
            this._cachingOptions = cachingOptions;
            this._coinLock = new SemaphoreSlim(1, 1);
            this._poolsLock = new SemaphoreSlim(1, 1);
        }

        public async Task<CoinData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            await this._coinLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // attempt to get cached first to avoid hammering APIs
                if (_cachedCoinData != null && DateTime.UtcNow < this._coinDateCacheTimeUTC + this._cachingOptions.CurrentValue.DataCacheLifetime)
                {
                    this._log.LogTrace("Found valid cached coin data, skipping APIs request");
                    return _cachedCoinData;
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
                this._cachedCoinData = new CoinData(txbitData.CurrencyName, txbitData.CurrencyCode)
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
                this._coinDateCacheTimeUTC = DateTime.UtcNow;
                return this._cachedCoinData;
            }
            finally
            {
                try { this._coinLock.Release(); } catch { }
            }
        }

        public async Task<MiningPoolStatsData> GetPoolsDataAsync(CancellationToken cancellationToken = default)
        {
            await this._poolsLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // attempt to get cached first to avoid hammering APIs
                if (_cachedPoolStatsData != null && DateTime.UtcNow < this._poolStatsDataCacheTimeUTC + this._cachingOptions.CurrentValue.MiningPoolStatsDataCacheLifetime)
                {
                    this._log.LogTrace("Found valid cached pools data, skipping APIs request");
                    return _cachedPoolStatsData;
                }

                this._log.LogInformation("Downloading all pools data");
                this._cachedPoolStatsData = await _poolStatsClient.GetDataAsync(cancellationToken).ConfigureAwait(false);
                this._poolStatsDataCacheTimeUTC = DateTime.UtcNow;
                return this._cachedPoolStatsData;
            }
            finally
            {
                try { this._poolsLock.Release(); } catch { }
            }
        }

        public void Dispose()
        {
            try { this._coinLock?.Dispose(); } catch { }
        }
    }
}
