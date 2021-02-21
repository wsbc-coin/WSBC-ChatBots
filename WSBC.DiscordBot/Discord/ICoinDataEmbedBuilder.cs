using Discord;

namespace WSBC.DiscordBot.Discord
{
    public interface ICoinDataEmbedBuilder
    {
        Embed Build(CoinData data, IMessage message);
    }
}
