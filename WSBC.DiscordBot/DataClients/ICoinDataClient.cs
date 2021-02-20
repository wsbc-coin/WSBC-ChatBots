using System.Threading;
using System.Threading.Tasks;

namespace WSBC.Discord
{
    interface ICoinDataClient<T>
    {
        Task<T> GetDataAsync(CancellationToken cancellationToken = default);
    }
}
