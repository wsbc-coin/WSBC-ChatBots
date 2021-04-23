using System.Threading;
using System.Threading.Tasks;
using WSBC.ChatBots.Discord.MiningPoolStats;

namespace WSBC.ChatBots.Discord
{
    public interface ICoinDataProvider
    {
        Task<CoinData> GetDataAsync(CancellationToken cancellationToken = default);
        Task<MiningPoolStatsData> GetPoolsDataAsync(CancellationToken cancellationToken = default);
    }
}
