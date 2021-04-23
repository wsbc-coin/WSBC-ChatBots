using System.Threading.Tasks;

namespace WSBC.ChatBots.Discord
{
    interface ICommandHandler
    {
        Task InitializeAsync();
    }
}
