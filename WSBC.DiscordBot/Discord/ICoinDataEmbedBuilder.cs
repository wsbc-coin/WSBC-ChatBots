using Discord;
using WSBC.DiscordBot.Explorer;

namespace WSBC.DiscordBot.Discord
{
    public interface ICoinDataEmbedBuilder
    {
        Embed Build(CoinData data, IMessage message);
        Embed Build(ExplorerBlockData data, IMessage message);
        Embed Build(ExplorerTransactionData data, IMessage message);
    }
}
