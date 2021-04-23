using System.Threading;
using System.Threading.Tasks;

namespace WSBC.ChatBots.Coin
{
    interface ICoinDataClient<T>
    {
        Task<T> GetDataAsync(CancellationToken cancellationToken = default);
    }
}
