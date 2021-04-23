using System.Threading;
using System.Threading.Tasks;

namespace WSBC.ChatBots.Coin
{
    public interface ICoinDataClient<T>
    {
        Task<T> GetDataAsync(CancellationToken cancellationToken = default);
    }
}
