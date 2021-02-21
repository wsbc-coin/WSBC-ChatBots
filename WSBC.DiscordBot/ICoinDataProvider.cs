using System.Threading;
using System.Threading.Tasks;

namespace WSBC.DiscordBot
{
    public interface ICoinDataProvider
    {
        Task<CoinData> GetDataAsync(CancellationToken cancellationToken = default);
    }
}
