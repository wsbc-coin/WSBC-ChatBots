using Discord;
using WSBC.ChatBots.Discord.Explorer;
using WSBC.ChatBots.Discord.MiningPoolStats;

namespace WSBC.ChatBots.Discord.Discord
{
    public interface ICoinDataEmbedBuilder
    {
        Embed Build(CoinData data, IMessage message);
        Embed Build(ExplorerBlockData data, IMessage message);
        Embed Build(ExplorerTransactionData data, IMessage message);
        Embed Build(MiningPoolStatsData data, IMessage message);
    }
}
