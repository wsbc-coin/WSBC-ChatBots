using System.Threading;
using System.Threading.Tasks;
using WSBC.ChatBots.Coin.MiningPoolStats;

namespace WSBC.ChatBots.Coin
{
    public interface ICoinDataProvider
    {
        Task<CoinData> GetDataAsync(CancellationToken cancellationToken = default);
        Task<MiningPoolStatsData> GetPoolsDataAsync(CancellationToken cancellationToken = default);
    }
}
