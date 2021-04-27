using System.Threading;
using System.Threading.Tasks;

namespace WSBC.ChatBots.Telegram.Autopost
{
    interface IAutopostService
    {
        Task SendNextAsync(long chatID, CancellationToken cancellationToken = default);
    }
}
