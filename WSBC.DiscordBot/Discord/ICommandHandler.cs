using System.Threading.Tasks;

namespace WSBC.DiscordBot.Discord
{
    interface ICommandHandler
    {
        Task InitializeAsync();
    }
}
