using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using WSBC.ChatBots.Coin;
using WSBC.ChatBots.Coin.Explorer;
using WSBC.ChatBots.Token;

namespace WSBC.ChatBots.Discord.Commands
{
    [Name("Token Info")]
    public class TokenCheckCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ITokenDataProvider _dataProvider;
        private readonly ITokenDataEmbedBuilder _embedBuilder;
        private readonly ILogger _log;

        public TokenCheckCommands(ITokenDataProvider dataProvider, ITokenDataEmbedBuilder embedBuilder,
            ILogger<TokenCheckCommands> log)
        {
            this._dataProvider = dataProvider;
            this._embedBuilder = embedBuilder;
            this._log = log;
        }

        [Command("token")]
        [Summary("Shows current WSBT data.")]
        [Priority(40)]
        public async Task CmdTokenDataAsync()
        {
            using IDisposable logScope = this._log.BeginCommandScope(base.Context, this);
            IUserMessage stateMessage = null;
            TokenData data;

            Task SendErrorAsync()
               => this.EditOrSendAsync(stateMessage, $"\u274C Couldn't retrieve token data.");

            try
            {
                Task<TokenData> dataTask = this._dataProvider.GetDataAsync();
                if (!dataTask.IsCompleted)
                    stateMessage = await base.ReplyAsync("Fetching token data, please wait...").ConfigureAwait(false);
                data = await dataTask.ConfigureAwait(false);
                if (data == null)
                {
                    await SendErrorAsync().ConfigureAwait(false);
                    return;
                }
            }
            catch
            {
                await SendErrorAsync().ConfigureAwait(false);
                throw;
            }
            Embed embed = this._embedBuilder.Build(data, base.Context.Message);
            await this.EditOrSendAsync(stateMessage, null, embed: embed).ConfigureAwait(false);
        }

        private Task EditOrSendAsync(IUserMessage message, string text = null, Embed embed = null)
        {
            if (message == null)
                return base.ReplyAsync(text, false, embed);
            else
                return message.ModifyAsync(msg =>
                {
                    msg.Content = text;
                    msg.Embed = embed;
                });
        }
    }
}
