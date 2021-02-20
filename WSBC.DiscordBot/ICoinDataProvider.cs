using System.Threading;
using System.Threading.Tasks;

namespace WSBC.Discord
{
    interface ICoinDataProvider
    {
        Task<CoinData> GetDataAsync(CancellationToken cancellationToken = default);
    }
}
