using System.Threading.Tasks;

namespace WSBC.ChatBots.Discord.Discord
{
    interface ICommandHandler
    {
        Task InitializeAsync();
    }
}
