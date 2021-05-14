using Discord;
using WSBC.ChatBots.Token;

namespace WSBC.ChatBots.Discord
{
    public interface ITokenDataEmbedBuilder
    {
        Embed Build(TokenData data, IMessage message);
    }
}
