using System.Threading;
using System.Threading.Tasks;

namespace WSBC.ChatBots.Telegram
{
    interface ICommandsHandler
    {
        void Register(TelegramCommand command);
        Task SubmitCommandsAsync(CancellationToken cancellationToken = default);
    }
}
