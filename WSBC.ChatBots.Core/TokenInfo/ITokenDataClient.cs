using System.Threading;
using System.Threading.Tasks;

namespace WSBC.ChatBots.Token
{
    public interface ITokenDataClient<T>
    {
        Task<T> GetDataAsync(CancellationToken cancellationToken = default);
    }
}
