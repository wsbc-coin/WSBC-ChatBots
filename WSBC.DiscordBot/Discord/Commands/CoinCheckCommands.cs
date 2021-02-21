using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace WSBC.DiscordBot.Discord.Commands
{
    public class CoinCheckCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ICoinDataProvider _dataProvider;
        private readonly ICoinDataEmbedBuilder _embedBuilder;
        private readonly ILogger _log;

        public CoinCheckCommands(ICoinDataProvider dataProvider, ICoinDataEmbedBuilder embedBuilder, ILogger<CoinCheckCommands> log)
        {
            this._dataProvider = dataProvider;
            this._embedBuilder = embedBuilder;
            this._log = log;
        }

        [Command("coin")]
        [Summary("Shows current currency data")]
        public async Task CoinDataAsync()
        {
            using IDisposable logScope = this._log.BeginCommandScope(base.Context, this);

            CoinData data = await this._dataProvider.GetDataAsync().ConfigureAwait(false);
            Embed embed = this._embedBuilder.Build(data, base.Context.Message);
            await base.Context.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }
    }
}
