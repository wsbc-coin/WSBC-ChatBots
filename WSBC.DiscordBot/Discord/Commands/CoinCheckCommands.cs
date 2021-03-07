using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using WSBC.DiscordBot.Explorer;
using WSBC.DiscordBot.MiningPoolStats;

namespace WSBC.DiscordBot.Discord.Commands
{
    [Name("Coin Info")]
    public class CoinCheckCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ICoinDataProvider _dataProvider;
        private readonly ICoinDataEmbedBuilder _embedBuilder;
        private readonly IExplorerDataClient _explorerClient;
        private readonly ILogger _log;

        public CoinCheckCommands(ICoinDataProvider dataProvider, ICoinDataEmbedBuilder embedBuilder, IExplorerDataClient explorerClient,
            ILogger<CoinCheckCommands> log)
        {
            this._dataProvider = dataProvider;
            this._embedBuilder = embedBuilder;
            this._explorerClient = explorerClient;
            this._log = log;
        }

        [Command("coin")]
        [Alias("info")]
        [Summary("Shows current currency and blockchain data.")]
        [Priority(10)]
        public async Task CmdCoinDataAsync()
        {
            using IDisposable logScope = this._log.BeginCommandScope(base.Context, this);
            IUserMessage stateMessage = null;
            CoinData data;

            Task SendErrorAsync()
               => this.EditOrSendAsync(stateMessage, $"\u274C Couldn't retrieve coin data.");

            try
            {
                Task<CoinData> dataTask = this._dataProvider.GetDataAsync();
                if (!dataTask.IsCompleted)
                    stateMessage = await base.ReplyAsync("Fetching coin data, please wait...").ConfigureAwait(false);
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

        [Command("block")]
        [Summary("Gets block data.")]
        [Priority(9)]
        public async Task CmdBlockDataAsync([Summary("Block height, or block unique hash.")]string block)
        {
            using IDisposable logScope = this._log.BeginCommandScope(base.Context, this);
            IUserMessage stateMessage = null;
            ExplorerBlockData data;

            Task SendErrorAsync()
                => this.EditOrSendAsync(stateMessage, $"\u274C Couldn't retrieve data about block `{block}`.");

            try
            {
                Task<ExplorerBlockData> dataTask = this._explorerClient.GetBlockDataAsync(block);
                if (!dataTask.IsCompleted)
                    stateMessage = await base.ReplyAsync("Fetching block data, please wait...").ConfigureAwait(false);
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
            await this.EditOrSendAsync(stateMessage, embed: embed).ConfigureAwait(false);
        }

        [Command("transaction")]
        [Alias("tx")]
        [Summary("Gets transaction data.")]
        [Priority(8)]
        public async Task CmdTransactionDataAsync([Summary("Unique hash of the transaction")]string hash)
        {
            using IDisposable logScope = this._log.BeginCommandScope(base.Context, this);
            IUserMessage stateMessage = null;
            ExplorerTransactionData data;

            Task SendErrorAsync()
                => this.EditOrSendAsync(stateMessage, $"\u274C Couldn't retrieve data about transaction `{hash}`.");

            try
            {
                Task<ExplorerTransactionData> dataTask = this._explorerClient.GetTransactionDataAsync(hash);
                if (!dataTask.IsCompleted)
                    stateMessage = await base.ReplyAsync("Fetching transaction data, please wait...").ConfigureAwait(false);
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
            await this.EditOrSendAsync(stateMessage, embed: embed).ConfigureAwait(false);
        }

        [Command("pools")]
        [Summary("Gets mining pools data.")]
        [Priority(7)]
        public async Task CmdPoolsDataAsync()
        {
            using IDisposable logScope = this._log.BeginCommandScope(base.Context, this);
            IUserMessage stateMessage = null;
            MiningPoolStatsData data;

            Task SendErrorAsync()
                => this.EditOrSendAsync(stateMessage, $"\u274C Couldn't retrieve data about mining pools.");
            try
            {
                Task<MiningPoolStatsData> dataTask = this._dataProvider.GetPoolsDataAsync();
                if (!dataTask.IsCompleted)
                {
                    stateMessage = await base.ReplyAsync("Fetching mining pools data, please wait...").ConfigureAwait(false);
                }
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
            await this.EditOrSendAsync(stateMessage, embed: embed).ConfigureAwait(false);
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
