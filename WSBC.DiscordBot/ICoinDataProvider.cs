using System.Threading;
using System.Threading.Tasks;

namespace WSBC.DiscordBot
{
    interface ICoinDataProvider
    {
        Task<CoinData> GetDataAsync(CancellationToken cancellationToken = default);
    }
}
