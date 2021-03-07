using System.Threading;
using System.Threading.Tasks;
using WSBC.DiscordBot.MiningPoolStats;

namespace WSBC.DiscordBot
{
    public interface ICoinDataProvider
    {
        Task<CoinData> GetDataAsync(CancellationToken cancellationToken = default);
        Task<MiningPoolStatsData> GetPoolsDataAsync(CancellationToken cancellationToken = default);
    }
}
