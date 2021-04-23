using Discord;
using WSBC.ChatBots.Coin;
using WSBC.ChatBots.Coin.Explorer;
using WSBC.ChatBots.Coin.MiningPoolStats;

namespace WSBC.ChatBots.Discord
{
    public interface ICoinDataEmbedBuilder
    {
        Embed Build(CoinData data, IMessage message);
        Embed Build(ExplorerBlockData data, IMessage message);
        Embed Build(ExplorerTransactionData data, IMessage message);
        Embed Build(MiningPoolStatsData data, IMessage message);
    }
}
