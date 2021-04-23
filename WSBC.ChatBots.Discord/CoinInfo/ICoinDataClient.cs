using System.Threading;
using System.Threading.Tasks;

namespace WSBC.ChatBots.Discord
{
    interface ICoinDataClient<T>
    {
        Task<T> GetDataAsync(CancellationToken cancellationToken = default);
    }
}
