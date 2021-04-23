using System.Threading;
using System.Threading.Tasks;

namespace WSBC.ChatBots.Token
{
    public interface ITokenDataProvider
    {
        Task<TokenData> GetDataAsync(CancellationToken cancellationToken = default);
    }
}
