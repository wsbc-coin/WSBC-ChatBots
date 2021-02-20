using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WSBC.Discord.TxBit;

namespace WSBC.Discord
{
    class CoinDataProvider : ICoinDataProvider
    {
        private readonly ICoinDataClient<TxBitData> _txbitClient;
        private readonly ILogger _log;

        public CoinDataProvider(ICoinDataClient<TxBitData> txbitClient, ILogger<CoinDataProvider> log)
        {
            this._txbitClient = txbitClient;
            this._log = log;
        }

        public async Task<CoinData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            this._log.LogInformation("Downloading all coin data");

            // start downloading: TxBit
            Task<TxBitData> txbitTask = this._txbitClient.GetDataAsync(cancellationToken);

            // await all results
            TxBitData txbitData = await txbitTask.ConfigureAwait(false);

            // aggregate all data and return
            return new CoinData(txbitData.CurrencyName, txbitData.CurrencyCode)
            {
                Supply = txbitData.Supply,
                MarketCap = txbitData.MarketCap,
                BtcPrice = txbitData.BidPrice,
                BlockHeight = txbitData.BlockCount
            };
        }
    }
}
