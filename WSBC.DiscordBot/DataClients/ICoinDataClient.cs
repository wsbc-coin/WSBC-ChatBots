using System.Threading;
using System.Threading.Tasks;

namespace WSBC.DiscordBot
{
    interface ICoinDataClient<T>
    {
        Task<T> GetDataAsync(CancellationToken cancellationToken = default);
    }
}
